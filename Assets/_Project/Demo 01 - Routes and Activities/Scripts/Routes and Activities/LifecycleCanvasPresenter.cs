using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

namespace FirstGame.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class LifecycleCanvasPresenter : MonoBehaviour
    {
        private const string MissingIdentity = "<MISSING IDENTITY>";
        private const string MissingScene = "<NO SCENE>";

        [Header("On-Demand Lifecycle Check")]
        [SerializeField]
        private TMP_Text reportValue;

        [Header("Console Diagnostics")]
        [SerializeField]
        private bool writeConsoleLog = true;

        [SerializeField]
        private bool includeFrame = true;

        [SerializeField]
        private bool includeUnscaledTime = true;

        [SerializeField]
        private bool includeSourcePath = true;

        private readonly Dictionary<SourceKey, SourceState> observedSources =
            new Dictionary<SourceKey, SourceState>();

        private bool initializationAttempted;
        private bool isReady;
        private bool hasLastEvent;
        private int sequence;
        private LifecycleEventRecord lastEvent;

        public bool IsReady => isReady;
        public int Sequence => sequence;
        public int ObservedSourceCount => observedSources.Count;
        public string RenderedReport => reportValue != null ? reportValue.text : string.Empty;

        private void Awake()
        {
            Initialize();
        }

        public void RecordEvent(
            LifecycleCanvasScope scope,
            LifecycleCanvasEventKind eventKind,
            string identity,
            Component source)
        {
            if (!isReady && !Initialize())
            {
                return;
            }

            if (!IsSupportedPair(scope, eventKind))
            {
                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE] Unsupported lifecycle scope/event pair. " +
                    $"scope='{scope}' event='{eventKind}' identity='{Escape(identity)}'.",
                    source != null ? source : this);
                return;
            }

            string normalizedIdentity = NormalizeIdentity(identity, scope, eventKind, source);
            GameObject sourceObject = source != null ? source.gameObject : gameObject;
            string sourcePath = BuildTransformPath(sourceObject.transform);
            string sourceScene = ResolveSceneName(sourceObject);
            EntityId sourceEntityId = source != null ? source.GetEntityId() : GetEntityId();

            sequence++;

            var record = new LifecycleEventRecord(
                sequence,
                Time.frameCount,
                Time.unscaledTimeAsDouble,
                scope,
                eventKind,
                normalizedIdentity,
                sourcePath,
                sourceScene);

            var key = new SourceKey(sourceEntityId, scope);
            if (!observedSources.TryGetValue(key, out SourceState sourceState))
            {
                sourceState = new SourceState(
                    sequence,
                    scope,
                    normalizedIdentity,
                    sourcePath,
                    sourceScene);
                observedSources.Add(key, sourceState);
            }
            else
            {
                sourceState.RefreshIdentity(normalizedIdentity, sourcePath, sourceScene);
            }

            sourceState.Observe(record);
            lastEvent = record;
            hasLastEvent = true;

            RenderReport();

            if (writeConsoleLog)
            {
                Debug.Log(FormatConsoleLog(record, sourceState), sourceObject);
            }
        }

        [ContextMenu("Reset On-Demand Lifecycle Check")]
        public void ResetReport()
        {
            if (!isReady && !Initialize())
            {
                return;
            }

            sequence = 0;
            hasLastEvent = false;
            lastEvent = default;
            observedSources.Clear();
            RenderReport();
        }

        private bool Initialize()
        {
            if (isReady)
            {
                return true;
            }

            if (initializationAttempted)
            {
                return false;
            }

            initializationAttempted = true;

            if (reportValue == null)
            {
                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE] Lifecycle Canvas Presenter is invalid. " +
                    $"Missing required reference '{nameof(reportValue)}'. " +
                    $"object='{BuildTransformPath(transform)}'.",
                    this);
                return false;
            }

            isReady = true;
            RenderReport();
            return true;
        }

        private void RenderReport()
        {
            if (!isReady || reportValue == null)
            {
                return;
            }

            if (observedSources.Count == 0)
            {
                reportValue.text =
                    "<b>WAITING FOR LIFECYCLE CALLBACK</b>\n" +
                    "A Route, Activity or Scene will appear only after its callback is received.";
                return;
            }

            var orderedSources = new List<SourceState>(observedSources.Values);
            orderedSources.Sort((left, right) => left.FirstSequence.CompareTo(right.FirstSequence));

            var builder = new StringBuilder(512);
            builder.Append("<b>EVENTS RECEIVED: ")
                .Append(sequence)
                .AppendLine("</b>")
                .AppendLine();

            foreach (SourceState source in orderedSources)
            {
                AppendSourceStatus(builder, source);
            }

            builder.AppendLine();
            builder.Append("<b>LAST:</b> ");

            if (!hasLastEvent)
            {
                builder.Append("NONE");
            }
            else
            {
                builder.Append(lastEvent.Scope.ToString().ToUpperInvariant())
                    .Append(" | ")
                    .Append(lastEvent.Identity)
                    .Append(" | ")
                    .Append(ToDisplayName(lastEvent.EventKind));
            }

            reportValue.text = builder.ToString();
        }

        private static void AppendSourceStatus(StringBuilder builder, SourceState source)
        {
            builder.Append("<b>")
                .Append(source.Scope.ToString().ToUpperInvariant())
                .Append("</b> | ")
                .Append(source.Identity)
                .Append(" | ");

            if (source.Scope == LifecycleCanvasScope.Scene)
            {
                AppendEventStatus(builder, LifecycleCanvasEventKind.Available, source.AvailableCount);
                builder.Append(" | ");
                AppendEventStatus(builder, LifecycleCanvasEventKind.Releasing, source.ReleasingCount);
            }
            else
            {
                AppendEventStatus(builder, LifecycleCanvasEventKind.Entered, source.EnteredCount);
                builder.Append(" | ");
                AppendEventStatus(builder, LifecycleCanvasEventKind.Exited, source.ExitedCount);
            }

            builder.AppendLine();
        }

        private static void AppendEventStatus(
            StringBuilder builder,
            LifecycleCanvasEventKind eventKind,
            int observedCount)
        {
            builder.Append(ToDisplayName(eventKind)).Append(": ");

            if (observedCount <= 0)
            {
                builder.Append("WAITING");
                return;
            }

            builder.Append("CALLED");
            if (observedCount > 1)
            {
                builder.Append(" x").Append(observedCount);
            }
        }

        private string FormatConsoleLog(LifecycleEventRecord record, SourceState sourceState)
        {
            var builder = new StringBuilder(512);
            builder.Append("[FIRSTGAME_LIFECYCLE]")
                .Append(" sequence='").Append(record.Sequence).Append('\'')
                .Append(" senderType='").Append(record.Scope).Append('\'')
                .Append(" sender='").Append(Escape(record.Identity)).Append('\'')
                .Append(" event='").Append(record.EventKind).Append('\'')
                .Append(" callbackObserved='True'")
                .Append(" sourceEventCount='").Append(sourceState.GetCount(record.EventKind)).Append('\'')
                .Append(" observedSources='").Append(ObservedSourceCount).Append('\'')
                .Append(" scene='").Append(Escape(record.SourceScene)).Append('\'');

            if (includeSourcePath)
            {
                builder.Append(" source='").Append(Escape(record.SourcePath)).Append('\'');
            }

            if (includeFrame)
            {
                builder.Append(" frame='").Append(record.Frame).Append('\'');
            }

            if (includeUnscaledTime)
            {
                builder.Append(" time='")
                    .Append(record.UnscaledTime.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append('\'');
            }

            return builder.ToString();
        }

        private static string NormalizeIdentity(
            string identity,
            LifecycleCanvasScope scope,
            LifecycleCanvasEventKind eventKind,
            Component source)
        {
            string normalized = identity?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(normalized))
            {
                return normalized;
            }

            Debug.LogError(
                "[FIRSTGAME_LIFECYCLE] Lifecycle callback reporter has an empty identity. " +
                $"scope='{scope}' event='{eventKind}'.",
                source);
            return MissingIdentity;
        }

        private static bool IsSupportedPair(
            LifecycleCanvasScope scope,
            LifecycleCanvasEventKind eventKind)
        {
            if (scope == LifecycleCanvasScope.Scene)
            {
                return eventKind == LifecycleCanvasEventKind.Available ||
                       eventKind == LifecycleCanvasEventKind.Releasing;
            }

            if (scope == LifecycleCanvasScope.Route || scope == LifecycleCanvasScope.Activity)
            {
                return eventKind == LifecycleCanvasEventKind.Entered ||
                       eventKind == LifecycleCanvasEventKind.Exited;
            }

            return false;
        }

        private static string ToDisplayName(LifecycleCanvasEventKind eventKind)
        {
            switch (eventKind)
            {
                case LifecycleCanvasEventKind.Available:
                    return "AVAILABLE";
                case LifecycleCanvasEventKind.Releasing:
                    return "RELEASING";
                case LifecycleCanvasEventKind.Entered:
                    return "ENTERED";
                case LifecycleCanvasEventKind.Exited:
                    return "EXITED";
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventKind), eventKind, null);
            }
        }

        private static string ResolveSceneName(GameObject sourceObject)
        {
            if (sourceObject == null || !sourceObject.scene.IsValid())
            {
                return MissingScene;
            }

            return string.IsNullOrWhiteSpace(sourceObject.scene.name)
                ? MissingScene
                : sourceObject.scene.name;
        }

        private static string BuildTransformPath(Transform current)
        {
            if (current == null)
            {
                return "<missing>";
            }

            string path = current.name;
            while (current.parent != null)
            {
                current = current.parent;
                path = current.name + "/" + path;
            }

            return path;
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private readonly struct SourceKey : IEquatable<SourceKey>
        {
            public SourceKey(EntityId sourceEntityId, LifecycleCanvasScope scope)
            {
                SourceEntityId = sourceEntityId;
                Scope = scope;
            }

            private EntityId SourceEntityId { get; }
            private LifecycleCanvasScope Scope { get; }

            public bool Equals(SourceKey other)
            {
                return SourceEntityId.Equals(other.SourceEntityId) && Scope == other.Scope;
            }

            public override bool Equals(object obj)
            {
                return obj is SourceKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (SourceEntityId.GetHashCode() * 397) ^ (int)Scope;
                }
            }
        }

        private sealed class SourceState
        {
            public SourceState(
                int firstSequence,
                LifecycleCanvasScope scope,
                string identity,
                string sourcePath,
                string sourceScene)
            {
                FirstSequence = firstSequence;
                Scope = scope;
                Identity = identity;
                SourcePath = sourcePath;
                SourceScene = sourceScene;
            }

            public int FirstSequence { get; }
            public LifecycleCanvasScope Scope { get; }
            public string Identity { get; private set; }
            public string SourcePath { get; private set; }
            public string SourceScene { get; private set; }
            public int AvailableCount { get; private set; }
            public int ReleasingCount { get; private set; }
            public int EnteredCount { get; private set; }
            public int ExitedCount { get; private set; }

            public void RefreshIdentity(string identity, string sourcePath, string sourceScene)
            {
                Identity = identity;
                SourcePath = sourcePath;
                SourceScene = sourceScene;
            }

            public void Observe(LifecycleEventRecord record)
            {
                switch (record.EventKind)
                {
                    case LifecycleCanvasEventKind.Available:
                        AvailableCount++;
                        break;
                    case LifecycleCanvasEventKind.Releasing:
                        ReleasingCount++;
                        break;
                    case LifecycleCanvasEventKind.Entered:
                        EnteredCount++;
                        break;
                    case LifecycleCanvasEventKind.Exited:
                        ExitedCount++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public int GetCount(LifecycleCanvasEventKind eventKind)
            {
                switch (eventKind)
                {
                    case LifecycleCanvasEventKind.Available:
                        return AvailableCount;
                    case LifecycleCanvasEventKind.Releasing:
                        return ReleasingCount;
                    case LifecycleCanvasEventKind.Entered:
                        return EnteredCount;
                    case LifecycleCanvasEventKind.Exited:
                        return ExitedCount;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(eventKind), eventKind, null);
                }
            }
        }

        private readonly struct LifecycleEventRecord
        {
            public LifecycleEventRecord(
                int sequence,
                int frame,
                double unscaledTime,
                LifecycleCanvasScope scope,
                LifecycleCanvasEventKind eventKind,
                string identity,
                string sourcePath,
                string sourceScene)
            {
                Sequence = sequence;
                Frame = frame;
                UnscaledTime = unscaledTime;
                Scope = scope;
                EventKind = eventKind;
                Identity = identity;
                SourcePath = sourcePath;
                SourceScene = sourceScene;
            }

            public int Sequence { get; }
            public int Frame { get; }
            public double UnscaledTime { get; }
            public LifecycleCanvasScope Scope { get; }
            public LifecycleCanvasEventKind EventKind { get; }
            public string Identity { get; }
            public string SourcePath { get; }
            public string SourceScene { get; }
        }
    }
}
