using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private const string NotObserved = "NOT OBSERVED";

        [Header("Lifecycle Report")]
        [SerializeField]
        private TMP_Text reportValue;

        [SerializeField, Min(1)]
        private int visibleTimelineEntries = 12;

        [Header("Console Diagnostics")]
        [SerializeField]
        private bool writeConsoleLog = true;

        [SerializeField]
        private bool includeFrame = true;

        [SerializeField]
        private bool includeUnscaledTime = true;

        [SerializeField]
        private bool includeSourcePath = true;

        private readonly Dictionary<CallbackKey, CallbackState> configuredCallbacks =
            new Dictionary<CallbackKey, CallbackState>();

        private readonly Queue<LifecycleEventRecord> timeline =
            new Queue<LifecycleEventRecord>();

        private bool initializationAttempted;
        private bool isReady;
        private int sequence;
        private ScopeState sceneState;
        private ScopeState routeState;
        private ScopeState activityState;

        public bool IsReady => isReady;
        public int Sequence => sequence;
        public int ConfiguredCallbackCount => configuredCallbacks.Count;
        public int ObservedCallbackCount => configuredCallbacks.Values.Count(state => state.ObservedCount > 0);
        public int PendingCallbackCount => ConfiguredCallbackCount - ObservedCallbackCount;
        public string RenderedReport => reportValue != null ? reportValue.text : string.Empty;

        private void Awake()
        {
            Initialize();
        }

        private void OnValidate()
        {
            visibleTimelineEntries = Mathf.Max(1, visibleTimelineEntries);
        }

        public void RegisterConfiguredReporter(
            LifecycleCanvasScope scope,
            string identity,
            Component source)
        {
            if (!isReady && !Initialize())
            {
                return;
            }

            string normalizedIdentity = NormalizeIdentity(identity, scope, null, source);
            GameObject sourceObject = source != null ? source.gameObject : gameObject;
            string sourcePath = BuildTransformPath(sourceObject.transform);
            string sourceScene = ResolveSceneName(sourceObject);
            int sourceInstanceId = source != null ? source.GetEntityId() : GetEntityId();

            foreach (LifecycleCanvasEventKind eventKind in GetExpectedEvents(scope))
            {
                var key = new CallbackKey(sourceInstanceId, eventKind);
                if (configuredCallbacks.ContainsKey(key))
                {
                    continue;
                }

                configuredCallbacks.Add(
                    key,
                    new CallbackState(
                        scope,
                        eventKind,
                        normalizedIdentity,
                        sourcePath,
                        sourceScene));
            }

            RenderReport();
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

            RegisterConfiguredReporter(scope, identity, source);

            string normalizedIdentity = NormalizeIdentity(identity, scope, eventKind, source);
            GameObject sourceObject = source != null ? source.gameObject : gameObject;
            string sourcePath = BuildTransformPath(sourceObject.transform);
            string sourceScene = ResolveSceneName(sourceObject);
            int sourceInstanceId = source != null ? source.GetEntityId() : GetEntityId();

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

            var key = new CallbackKey(sourceInstanceId, eventKind);
            if (!configuredCallbacks.TryGetValue(key, out CallbackState callbackState))
            {
                callbackState = new CallbackState(
                    scope,
                    eventKind,
                    normalizedIdentity,
                    sourcePath,
                    sourceScene);
                configuredCallbacks.Add(key, callbackState);
            }

            callbackState.Observe(record);
            ApplyNavigationState(record);

            timeline.Enqueue(record);
            while (timeline.Count > visibleTimelineEntries)
            {
                timeline.Dequeue();
            }

            RenderReport();

            if (writeConsoleLog)
            {
                Debug.Log(FormatConsoleLog(record), sourceObject);
            }
        }

        [ContextMenu("Clear Timeline")]
        public void ClearTimeline()
        {
            timeline.Clear();
            RenderReport();
        }

        [ContextMenu("Reset Lifecycle Report")]
        public void ResetReport()
        {
            if (!isReady && !Initialize())
            {
                return;
            }

            sequence = 0;
            timeline.Clear();
            sceneState = default;
            routeState = default;
            activityState = default;

            foreach (CallbackState callback in configuredCallbacks.Values)
            {
                callback.ResetObservation();
            }

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

            visibleTimelineEntries = Mathf.Max(1, visibleTimelineEntries);
            isReady = true;
            RenderReport();
            return true;
        }

        private void ApplyNavigationState(LifecycleEventRecord record)
        {
            var state = new ScopeState(record.EventKind, record.Identity, record.Sequence);

            switch (record.Scope)
            {
                case LifecycleCanvasScope.Scene:
                    sceneState = state;
                    break;

                case LifecycleCanvasScope.Route:
                    routeState = state;
                    break;

                case LifecycleCanvasScope.Activity:
                    activityState = state;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(record.Scope), record.Scope, null);
            }
        }

        private void RenderReport()
        {
            if (!isReady || reportValue == null)
            {
                return;
            }

            var builder = new StringBuilder(2048);
            int configured = ConfiguredCallbackCount;
            int observed = ObservedCallbackCount;
            int pending = configured - observed;

            builder.Append("<b>EXECUTION SUMMARY</b>")
                .AppendLine()
                .Append("Callbacks configured: ").Append(configured)
                .Append("  |  observed: ").Append(observed)
                .Append("  |  pending: ").Append(pending)
                .Append("  |  events received: ").Append(sequence)
                .AppendLine()
                .AppendLine();

            builder.AppendLine("<b>CURRENT NAVIGATION</b>");
            AppendScopeState(builder, "Scene", sceneState);
            AppendScopeState(builder, "Route", routeState);
            AppendScopeState(builder, "Activity", activityState);
            builder.AppendLine();

            builder.AppendLine("<b>CONFIGURED CALLBACKS</b>");
            if (configuredCallbacks.Count == 0)
            {
                builder.AppendLine("No lifecycle callback reporters registered.");
            }
            else
            {
                IEnumerable<CallbackState> callbacks = configuredCallbacks.Values
                    .OrderBy(callback => callback.Scope)
                    .ThenBy(callback => callback.Identity, StringComparer.Ordinal)
                    .ThenBy(callback => callback.EventKind)
                    .ThenBy(callback => callback.SourcePath, StringComparer.Ordinal);

                foreach (CallbackState callback in callbacks)
                {
                    AppendCallbackState(builder, callback);
                }
            }

            builder.AppendLine();
            builder.Append("<b>EVENT TIMELINE</b>  ")
                .Append(timeline.Count)
                .Append('/')
                .Append(visibleTimelineEntries)
                .AppendLine();

            if (timeline.Count == 0)
            {
                builder.Append("No lifecycle events received.");
            }
            else
            {
                LifecycleEventRecord[] records = timeline.ToArray();
                for (int index = records.Length - 1; index >= 0; index--)
                {
                    AppendTimelineRecord(builder, records[index]);
                    if (index > 0)
                    {
                        builder.AppendLine();
                    }
                }
            }

            reportValue.text = builder.ToString();
        }

        private static void AppendScopeState(
            StringBuilder builder,
            string label,
            ScopeState state)
        {
            builder.Append(label.PadRight(9));

            if (!state.HasValue)
            {
                builder.AppendLine(NotObserved);
                return;
            }

            builder.Append(ToDisplayName(state.EventKind).PadRight(11))
                .Append(" | ")
                .Append(state.Identity)
                .Append(" | last=#")
                .Append(state.Sequence.ToString("D3", CultureInfo.InvariantCulture))
                .AppendLine();
        }

        private void AppendCallbackState(StringBuilder builder, CallbackState callback)
        {
            builder.Append(callback.ObservedCount > 0 ? "[OK] " : "[--] ")
                .Append(callback.Scope.ToString().ToUpperInvariant())
                .Append(' ')
                .Append(ToDisplayName(callback.EventKind))
                .Append(" | ")
                .Append(callback.Identity);

            if (callback.ObservedCount > 0)
            {
                builder.Append(" | count=")
                    .Append(callback.ObservedCount)
                    .Append(" last=#")
                    .Append(callback.LastSequence.ToString("D3", CultureInfo.InvariantCulture));

                if (includeFrame)
                {
                    builder.Append(" F")
                        .Append(callback.LastFrame.ToString("D4", CultureInfo.InvariantCulture));
                }

                if (includeUnscaledTime)
                {
                    builder.Append(' ')
                        .Append(callback.LastUnscaledTime.ToString("0.000", CultureInfo.InvariantCulture))
                        .Append('s');
                }
            }
            else
            {
                builder.Append(" | waiting");
            }

            if (includeSourcePath)
            {
                builder.Append(" | ").Append(callback.SourcePath);
            }

            builder.AppendLine();
        }

        private void AppendTimelineRecord(StringBuilder builder, LifecycleEventRecord record)
        {
            builder.Append('#')
                .Append(record.Sequence.ToString("D3", CultureInfo.InvariantCulture));

            if (includeFrame)
            {
                builder.Append("  F")
                    .Append(record.Frame.ToString("D4", CultureInfo.InvariantCulture));
            }

            if (includeUnscaledTime)
            {
                builder.Append("  ")
                    .Append(record.UnscaledTime.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append('s');
            }

            builder.Append("  ")
                .Append(record.Scope.ToString().ToUpperInvariant())
                .Append(' ')
                .Append(ToDisplayName(record.EventKind))
                .Append(" | ")
                .Append(record.Identity);

            if (includeSourcePath)
            {
                builder.Append(" | ").Append(record.SourcePath);
            }
        }

        private string FormatConsoleLog(LifecycleEventRecord record)
        {
            var builder = new StringBuilder(384);
            builder.Append("[FIRSTGAME_LIFECYCLE]")
                .Append(" sequence='").Append(record.Sequence).Append('\'')
                .Append(" scope='").Append(record.Scope).Append('\'')
                .Append(" event='").Append(record.EventKind).Append('\'')
                .Append(" identity='").Append(Escape(record.Identity)).Append('\'')
                .Append(" source='").Append(Escape(record.SourcePath)).Append('\'')
                .Append(" scene='").Append(Escape(record.SourceScene)).Append('\'')
                .Append(" callbackObserved='True'");

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
            LifecycleCanvasEventKind? eventKind,
            Component source)
        {
            string normalized = identity?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(normalized))
            {
                return normalized;
            }

            Debug.LogError(
                "[FIRSTGAME_LIFECYCLE] Lifecycle callback reporter has an empty identity. " +
                $"scope='{scope}' event='{eventKind?.ToString() ?? "Registration"}'.",
                source);
            return MissingIdentity;
        }

        private static IEnumerable<LifecycleCanvasEventKind> GetExpectedEvents(
            LifecycleCanvasScope scope)
        {
            switch (scope)
            {
                case LifecycleCanvasScope.Scene:
                    yield return LifecycleCanvasEventKind.Available;
                    yield return LifecycleCanvasEventKind.Releasing;
                    break;

                case LifecycleCanvasScope.Route:
                case LifecycleCanvasScope.Activity:
                    yield return LifecycleCanvasEventKind.Entered;
                    yield return LifecycleCanvasEventKind.Exited;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }
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

            return eventKind == LifecycleCanvasEventKind.Entered ||
                   eventKind == LifecycleCanvasEventKind.Exited;
        }

        private static string ToDisplayName(LifecycleCanvasEventKind eventKind)
        {
            return eventKind.ToString().ToUpperInvariant();
        }

        private static string ResolveSceneName(GameObject sourceObject)
        {
            return sourceObject.scene.IsValid() && !string.IsNullOrWhiteSpace(sourceObject.scene.name)
                ? sourceObject.scene.name
                : MissingScene;
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

        private readonly struct CallbackKey : IEquatable<CallbackKey>
        {
            public CallbackKey(int sourceInstanceId, LifecycleCanvasEventKind eventKind)
            {
                SourceInstanceId = sourceInstanceId;
                EventKind = eventKind;
            }

            private int SourceInstanceId { get; }
            private LifecycleCanvasEventKind EventKind { get; }

            public bool Equals(CallbackKey other)
            {
                return SourceInstanceId == other.SourceInstanceId && EventKind == other.EventKind;
            }

            public override bool Equals(object obj)
            {
                return obj is CallbackKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (SourceInstanceId * 397) ^ (int)EventKind;
                }
            }
        }

        private sealed class CallbackState
        {
            public CallbackState(
                LifecycleCanvasScope scope,
                LifecycleCanvasEventKind eventKind,
                string identity,
                string sourcePath,
                string sourceScene)
            {
                Scope = scope;
                EventKind = eventKind;
                Identity = identity;
                SourcePath = sourcePath;
                SourceScene = sourceScene;
            }

            public LifecycleCanvasScope Scope { get; }
            public LifecycleCanvasEventKind EventKind { get; }
            public string Identity { get; }
            public string SourcePath { get; }
            public string SourceScene { get; }
            public int ObservedCount { get; private set; }
            public int LastSequence { get; private set; }
            public int LastFrame { get; private set; }
            public double LastUnscaledTime { get; private set; }

            public void Observe(LifecycleEventRecord record)
            {
                ObservedCount++;
                LastSequence = record.Sequence;
                LastFrame = record.Frame;
                LastUnscaledTime = record.UnscaledTime;
            }

            public void ResetObservation()
            {
                ObservedCount = 0;
                LastSequence = 0;
                LastFrame = 0;
                LastUnscaledTime = 0d;
            }
        }

        private readonly struct ScopeState
        {
            public ScopeState(
                LifecycleCanvasEventKind eventKind,
                string identity,
                int sequence)
            {
                HasValue = true;
                EventKind = eventKind;
                Identity = identity;
                Sequence = sequence;
            }

            public bool HasValue { get; }
            public LifecycleCanvasEventKind EventKind { get; }
            public string Identity { get; }
            public int Sequence { get; }
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
