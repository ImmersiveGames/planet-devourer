using System.Collections.Generic;
using Immersive.Framework.Camera;
using Immersive.Framework.PlayerBinding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkIntegration.Editor
{
    /// <summary>
    /// FIRSTGAME editor-only proof that the canonical player binding facade can repair controlled reference drift.
    /// This tool deliberately drifts already-proven serialized references, invokes the facade apply tool, then validates
    /// that references return to the canonical real player chain. It does not create runtime lifecycle, route input,
    /// enable movement, spawn actors, execute gameplay, save progression or create test objects.
    /// </summary>
    public static class FirstGameCanonicalPlayerBindingFacadeRepairProof
    {
        private const string RepairProofMenuPath = "FIRSTGAME/Immersive Framework/Run Canonical Player Binding Facade Repair Proof";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";
        private const string LogTag = "[F53E_FIRSTGAME_PLAYER_BINDING_FACADE_REPAIR_PROOF]";

        private const string DriftedPlayerSlotId = "drifted.player";
        private const string DriftedActionMap = "DriftedActionMap";

        [MenuItem(RepairProofMenuPath)]
        public static void RunCanonicalPlayerBindingFacadeRepairProof()
        {
            RepairProofSnapshot snapshot = RunProof();
            LogProof(snapshot);
        }

        private static RepairProofSnapshot RunProof()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                return RepairProofSnapshot.Failed("InvalidScene");
            }

            if (!string.Equals(scene.path, GameplayScenePath, System.StringComparison.Ordinal))
            {
                return RepairProofSnapshot.Failed("UnexpectedActiveScene");
            }

            if (!FirstGamePlayerIdentityResolver.TryResolveCanonicalPlayer(
                    scene,
                    Selection.activeGameObject,
                    out FirstGameResolvedPlayer resolvedPlayer,
                    out string resolveFailureReason))
            {
                return RepairProofSnapshot.Failed(resolveFailureReason, resolvedPlayer);
            }

            GameObject playerObject = resolvedPlayer.GameObject;
            if (playerObject == null)
            {
                return RepairProofSnapshot.Failed("MissingResolvedPlayerObject", resolvedPlayer);
            }

            PlayerControlBindingTargetBehaviour controlTarget = playerObject.GetComponent<PlayerControlBindingTargetBehaviour>();
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget = playerObject.GetComponent<UnityPlayerInputBridgeTargetBehaviour>();
            UnityPlayerInputActivationTargetBehaviour activationTarget = playerObject.GetComponent<UnityPlayerInputActivationTargetBehaviour>();

            if (controlTarget == null)
            {
                return RepairProofSnapshot.Failed("MissingPlayerControlBindingTarget", resolvedPlayer);
            }

            if (bridgeTarget == null)
            {
                return RepairProofSnapshot.Failed("MissingUnityPlayerInputBridgeTarget", resolvedPlayer);
            }

            if (activationTarget == null)
            {
                return RepairProofSnapshot.Failed("MissingUnityPlayerInputActivationTarget", resolvedPlayer);
            }

            IReadOnlyList<FrameworkCameraAnchorHost> anchorHosts = FindSceneComponents<FrameworkCameraAnchorHost>(scene);
            FrameworkCameraAnchorHost anchorHost = ResolveMatchingAnchorHost(anchorHosts, resolvedPlayer.Transform, out int matchingAnchorHosts, out string anchorFailureReason);
            if (anchorHost == null)
            {
                return RepairProofSnapshot.Failed(anchorFailureReason, resolvedPlayer, anchorHosts.Count, matchingAnchorHosts);
            }

            BindingState originalState = CaptureState(bridgeTarget, activationTarget, anchorHost);
            if (!originalState.IsCanonical(resolvedPlayer))
            {
                return RepairProofSnapshot.FromState(
                    false,
                    resolvedPlayer,
                    anchorHost,
                    anchorHosts.Count,
                    matchingAnchorHosts,
                    originalState,
                    initialCanonical: false,
                    controlledDriftCreated: false,
                    facadeApplyInvoked: false,
                    repairSucceeded: false,
                    restoredOriginalOnFailure: false,
                    selectionRestored: true,
                    failureReason: "InitialFacadeStateNotCanonical");
            }

            GameObject previousSelection = Selection.activeGameObject;
            bool selectionRestored = false;
            bool controlledDriftCreated = false;
            bool facadeApplyInvoked = false;
            bool repairSucceeded = false;
            bool restoredOriginalOnFailure = false;
            string failureReason = "None";
            BindingState finalState = originalState;

            Undo.SetCurrentGroupName("Run FIRSTGAME Canonical Player Binding Facade Repair Proof");
            int undoGroup = Undo.GetCurrentGroup();

            try
            {
                Undo.RecordObject(bridgeTarget, "Create controlled FIRSTGAME PlayerInput bridge drift");
                Undo.RecordObject(activationTarget, "Create controlled FIRSTGAME PlayerInput activation drift");
                Undo.RecordObject(anchorHost, "Create controlled FIRSTGAME camera anchor drift");

                ApplyControlledDrift(bridgeTarget, activationTarget, anchorHost);
                EditorUtility.SetDirty(bridgeTarget);
                EditorUtility.SetDirty(activationTarget);
                EditorUtility.SetDirty(anchorHost);
                EditorSceneManager.MarkSceneDirty(scene);

                BindingState driftedState = CaptureState(bridgeTarget, activationTarget, anchorHost);
                controlledDriftCreated = driftedState.HasControlledDrift();
                if (!controlledDriftCreated)
                {
                    failureReason = "ControlledDriftWasNotCreated";
                    RestoreOriginalState(bridgeTarget, activationTarget, anchorHost, originalState);
                    restoredOriginalOnFailure = true;
                    finalState = CaptureState(bridgeTarget, activationTarget, anchorHost);
                    return RepairProofSnapshot.FromState(
                        false,
                        resolvedPlayer,
                        anchorHost,
                        anchorHosts.Count,
                        matchingAnchorHosts,
                        finalState,
                        initialCanonical: true,
                        controlledDriftCreated,
                        facadeApplyInvoked,
                        repairSucceeded,
                        restoredOriginalOnFailure,
                        selectionRestored,
                        failureReason);
                }

                Selection.activeGameObject = resolvedPlayer.GameObject;
                FirstGameCanonicalPlayerBindingAuthoringFacade.ApplyCanonicalPlayerBindingFacade();
                facadeApplyInvoked = true;

                finalState = CaptureState(bridgeTarget, activationTarget, anchorHost);
                repairSucceeded = finalState.IsCanonical(resolvedPlayer);
                if (!repairSucceeded)
                {
                    failureReason = "FacadeApplyDidNotRepairControlledDrift";
                    RestoreOriginalState(bridgeTarget, activationTarget, anchorHost, originalState);
                    restoredOriginalOnFailure = true;
                    finalState = CaptureState(bridgeTarget, activationTarget, anchorHost);
                }

                Undo.CollapseUndoOperations(undoGroup);
            }
            catch (System.Exception exception)
            {
                failureReason = string.IsNullOrWhiteSpace(exception.GetType().Name) ? "RepairProofException" : exception.GetType().Name;
                RestoreOriginalState(bridgeTarget, activationTarget, anchorHost, originalState);
                restoredOriginalOnFailure = true;
                finalState = CaptureState(bridgeTarget, activationTarget, anchorHost);
            }
            finally
            {
                Selection.activeGameObject = previousSelection;
                selectionRestored = ReferenceEquals(Selection.activeGameObject, previousSelection);
                EditorUtility.SetDirty(bridgeTarget);
                EditorUtility.SetDirty(activationTarget);
                EditorUtility.SetDirty(anchorHost);
                EditorSceneManager.MarkSceneDirty(scene);
            }

            bool succeeded = controlledDriftCreated && facadeApplyInvoked && repairSucceeded && string.Equals(failureReason, "None", System.StringComparison.Ordinal);
            return RepairProofSnapshot.FromState(
                succeeded,
                resolvedPlayer,
                anchorHost,
                anchorHosts.Count,
                matchingAnchorHosts,
                finalState,
                initialCanonical: true,
                controlledDriftCreated,
                facadeApplyInvoked,
                repairSucceeded,
                restoredOriginalOnFailure,
                selectionRestored,
                failureReason);
        }

        private static IReadOnlyList<T> FindSceneComponents<T>(Scene scene) where T : Component
        {
            List<T> components = new List<T>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                components.AddRange(root.GetComponentsInChildren<T>(true));
            }

            return components;
        }

        private static FrameworkCameraAnchorHost ResolveMatchingAnchorHost(
            IReadOnlyList<FrameworkCameraAnchorHost> anchorHosts,
            Transform playerTransform,
            out int matchingAnchorHosts,
            out string failureReason)
        {
            matchingAnchorHosts = 0;
            failureReason = "None";
            FrameworkCameraAnchorHost match = null;

            for (int i = 0; i < anchorHosts.Count; i++)
            {
                FrameworkCameraAnchorHost candidate = anchorHosts[i];
                if (candidate == null)
                {
                    continue;
                }

                ReadAnchorTargets(candidate, out Transform trackingTarget, out Transform lookAtTarget);
                if (ReferenceEquals(trackingTarget, playerTransform) && ReferenceEquals(lookAtTarget, playerTransform))
                {
                    match = candidate;
                    matchingAnchorHosts++;
                }
            }

            if (matchingAnchorHosts == 1)
            {
                return match;
            }

            failureReason = matchingAnchorHosts == 0
                ? "NoMatchingFrameworkCameraAnchorHost"
                : "MultipleMatchingFrameworkCameraAnchorHosts";
            return null;
        }

        private static BindingState CaptureState(
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget,
            UnityPlayerInputActivationTargetBehaviour activationTarget,
            FrameworkCameraAnchorHost anchorHost)
        {
            PlayerInput bridgePlayerInput = GetObjectIfExists(bridgeTarget, "playerInput") as PlayerInput;
            string bridgeExpectedSlot = GetStringIfExists(bridgeTarget, "expectedPlayerSlotId");
            PlayerInput activationPlayerInput = GetObjectIfExists(activationTarget, "playerInput") as PlayerInput;
            string activationExpectedSlot = GetStringIfExists(activationTarget, "expectedPlayerSlotId");
            string activationActionMap = GetStringIfExists(activationTarget, "actionMapName");
            ReadAnchorTargets(anchorHost, out Transform trackingTarget, out Transform lookAtTarget);

            return new BindingState(
                bridgePlayerInput,
                bridgeExpectedSlot,
                activationPlayerInput,
                activationExpectedSlot,
                activationActionMap,
                trackingTarget,
                lookAtTarget);
        }

        private static void ApplyControlledDrift(
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget,
            UnityPlayerInputActivationTargetBehaviour activationTarget,
            FrameworkCameraAnchorHost anchorHost)
        {
            SetObjectIfExists(bridgeTarget, "playerInput", null);
            SetStringIfExists(bridgeTarget, "expectedPlayerSlotId", DriftedPlayerSlotId);
            SetObjectIfExists(activationTarget, "playerInput", null);
            SetStringIfExists(activationTarget, "expectedPlayerSlotId", DriftedPlayerSlotId);
            SetStringIfExists(activationTarget, "actionMapName", DriftedActionMap);
            SetObjectIfExists(anchorHost, "trackingTarget", null);
            SetObjectIfExists(anchorHost, "lookAtTarget", null);
        }

        private static void RestoreOriginalState(
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget,
            UnityPlayerInputActivationTargetBehaviour activationTarget,
            FrameworkCameraAnchorHost anchorHost,
            BindingState originalState)
        {
            SetObjectIfExists(bridgeTarget, "playerInput", originalState.BridgePlayerInput);
            SetStringIfExists(bridgeTarget, "expectedPlayerSlotId", originalState.BridgeExpectedSlot);
            SetObjectIfExists(activationTarget, "playerInput", originalState.ActivationPlayerInput);
            SetStringIfExists(activationTarget, "expectedPlayerSlotId", originalState.ActivationExpectedSlot);
            SetStringIfExists(activationTarget, "actionMapName", originalState.ActivationActionMap);
            SetObjectIfExists(anchorHost, "trackingTarget", originalState.TrackingTarget);
            SetObjectIfExists(anchorHost, "lookAtTarget", originalState.LookAtTarget);
        }

        private static void SetStringIfExists(UnityEngine.Object target, string propertyName, string value)
        {
            if (target == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                return;
            }

            property.stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetObjectIfExists(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            if (target == null)
            {
                return;
            }

            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                return;
            }

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static string GetStringIfExists(UnityEngine.Object target, string propertyName)
        {
            if (target == null)
            {
                return string.Empty;
            }

            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            return property != null ? property.stringValue : string.Empty;
        }

        private static UnityEngine.Object GetObjectIfExists(UnityEngine.Object target, string propertyName)
        {
            if (target == null)
            {
                return null;
            }

            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty property = serialized.FindProperty(propertyName);
            return property != null ? property.objectReferenceValue : null;
        }

        private static void ReadAnchorTargets(FrameworkCameraAnchorHost anchorHost, out Transform trackingTarget, out Transform lookAtTarget)
        {
            trackingTarget = GetObjectIfExists(anchorHost, "trackingTarget") as Transform;
            lookAtTarget = GetObjectIfExists(anchorHost, "lookAtTarget") as Transform;
        }

        private static void LogProof(RepairProofSnapshot snapshot)
        {
            string status = snapshot.Succeeded ? "Succeeded" : "Failed";
            Debug.Log(
                LogTag + " " +
                $"status='{status}' " +
                $"playerObject='{snapshot.PlayerObjectName}' " +
                $"identitySource='{snapshot.IdentitySource}' " +
                $"resolutionSource='{snapshot.ResolutionSource}' " +
                $"resolvedByName='{snapshot.ResolvedByName}' " +
                $"actorId='{snapshot.ActorId}' " +
                $"playerSlotId='{snapshot.PlayerSlotId}' " +
                $"anchorHost='{snapshot.AnchorHostName}' " +
                $"anchorHostCount='{snapshot.AnchorHostCount}' " +
                $"matchingAnchorHosts='{snapshot.MatchingAnchorHosts}' " +
                $"initialCanonical='{snapshot.InitialCanonical}' " +
                $"controlledDriftCreated='{snapshot.ControlledDriftCreated}' " +
                $"facadeApplyInvoked='{snapshot.FacadeApplyInvoked}' " +
                $"repairSucceeded='{snapshot.RepairSucceeded}' " +
                $"selectionRestored='{snapshot.SelectionRestored}' " +
                $"restoredOriginalOnFailure='{snapshot.RestoredOriginalOnFailure}' " +
                $"bridgePlayerInputAfter='{snapshot.BridgePlayerInputAfter}' " +
                $"bridgeExpectedSlotAfter='{snapshot.BridgeExpectedSlotAfter}' " +
                $"activationPlayerInputAfter='{snapshot.ActivationPlayerInputAfter}' " +
                $"activationExpectedSlotAfter='{snapshot.ActivationExpectedSlotAfter}' " +
                $"activationActionMapAfter='{snapshot.ActivationActionMapAfter}' " +
                $"trackingTargetAfter='{snapshot.TrackingTargetAfter}' " +
                $"lookAtTargetAfter='{snapshot.LookAtTargetAfter}' " +
                $"typedReferencesRepaired='{snapshot.TypedReferencesRepaired}' " +
                $"createdPlayerControlBindingTarget='False' " +
                $"createdUnityPlayerInputBridgeTarget='False' " +
                $"createdUnityPlayerInputActivationTarget='False' " +
                "createdTestObject='False' createdPlayerInput='False' movement='False' actorSpawning='False' gameplayCommandExecution='False' " +
                $"failureReason='{snapshot.FailureReason}'.");
        }

        private readonly struct BindingState
        {
            public BindingState(
                PlayerInput bridgePlayerInput,
                string bridgeExpectedSlot,
                PlayerInput activationPlayerInput,
                string activationExpectedSlot,
                string activationActionMap,
                Transform trackingTarget,
                Transform lookAtTarget)
            {
                BridgePlayerInput = bridgePlayerInput;
                BridgeExpectedSlot = bridgeExpectedSlot;
                ActivationPlayerInput = activationPlayerInput;
                ActivationExpectedSlot = activationExpectedSlot;
                ActivationActionMap = activationActionMap;
                TrackingTarget = trackingTarget;
                LookAtTarget = lookAtTarget;
            }

            public PlayerInput BridgePlayerInput { get; }

            public string BridgeExpectedSlot { get; }

            public PlayerInput ActivationPlayerInput { get; }

            public string ActivationExpectedSlot { get; }

            public string ActivationActionMap { get; }

            public Transform TrackingTarget { get; }

            public Transform LookAtTarget { get; }

            public bool IsCanonical(FirstGameResolvedPlayer resolvedPlayer)
            {
                return resolvedPlayer.PlayerInput != null &&
                       ReferenceEquals(BridgePlayerInput, resolvedPlayer.PlayerInput) &&
                       string.Equals(BridgeExpectedSlot, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal) &&
                       ReferenceEquals(ActivationPlayerInput, resolvedPlayer.PlayerInput) &&
                       string.Equals(ActivationExpectedSlot, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal) &&
                       string.Equals(ActivationActionMap, FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap, System.StringComparison.Ordinal) &&
                       ReferenceEquals(TrackingTarget, resolvedPlayer.Transform) &&
                       ReferenceEquals(LookAtTarget, resolvedPlayer.Transform);
            }

            public bool HasControlledDrift()
            {
                return BridgePlayerInput == null &&
                       string.Equals(BridgeExpectedSlot, DriftedPlayerSlotId, System.StringComparison.Ordinal) &&
                       ActivationPlayerInput == null &&
                       string.Equals(ActivationExpectedSlot, DriftedPlayerSlotId, System.StringComparison.Ordinal) &&
                       string.Equals(ActivationActionMap, DriftedActionMap, System.StringComparison.Ordinal) &&
                       TrackingTarget == null &&
                       LookAtTarget == null;
            }
        }

        private readonly struct RepairProofSnapshot
        {
            private RepairProofSnapshot(
                bool succeeded,
                string playerObjectName,
                string identitySource,
                string resolutionSource,
                bool resolvedByName,
                string actorId,
                string playerSlotId,
                string anchorHostName,
                int anchorHostCount,
                int matchingAnchorHosts,
                bool initialCanonical,
                bool controlledDriftCreated,
                bool facadeApplyInvoked,
                bool repairSucceeded,
                bool restoredOriginalOnFailure,
                bool selectionRestored,
                string bridgePlayerInputAfter,
                string bridgeExpectedSlotAfter,
                string activationPlayerInputAfter,
                string activationExpectedSlotAfter,
                string activationActionMapAfter,
                string trackingTargetAfter,
                string lookAtTargetAfter,
                bool typedReferencesRepaired,
                string failureReason)
            {
                Succeeded = succeeded;
                PlayerObjectName = playerObjectName;
                IdentitySource = identitySource;
                ResolutionSource = resolutionSource;
                ResolvedByName = resolvedByName;
                ActorId = actorId;
                PlayerSlotId = playerSlotId;
                AnchorHostName = anchorHostName;
                AnchorHostCount = anchorHostCount;
                MatchingAnchorHosts = matchingAnchorHosts;
                InitialCanonical = initialCanonical;
                ControlledDriftCreated = controlledDriftCreated;
                FacadeApplyInvoked = facadeApplyInvoked;
                RepairSucceeded = repairSucceeded;
                RestoredOriginalOnFailure = restoredOriginalOnFailure;
                SelectionRestored = selectionRestored;
                BridgePlayerInputAfter = bridgePlayerInputAfter;
                BridgeExpectedSlotAfter = bridgeExpectedSlotAfter;
                ActivationPlayerInputAfter = activationPlayerInputAfter;
                ActivationExpectedSlotAfter = activationExpectedSlotAfter;
                ActivationActionMapAfter = activationActionMapAfter;
                TrackingTargetAfter = trackingTargetAfter;
                LookAtTargetAfter = lookAtTargetAfter;
                TypedReferencesRepaired = typedReferencesRepaired;
                FailureReason = string.IsNullOrWhiteSpace(failureReason) ? "None" : failureReason;
            }

            public bool Succeeded { get; }

            public string PlayerObjectName { get; }

            public string IdentitySource { get; }

            public string ResolutionSource { get; }

            public bool ResolvedByName { get; }

            public string ActorId { get; }

            public string PlayerSlotId { get; }

            public string AnchorHostName { get; }

            public int AnchorHostCount { get; }

            public int MatchingAnchorHosts { get; }

            public bool InitialCanonical { get; }

            public bool ControlledDriftCreated { get; }

            public bool FacadeApplyInvoked { get; }

            public bool RepairSucceeded { get; }

            public bool RestoredOriginalOnFailure { get; }

            public bool SelectionRestored { get; }

            public string BridgePlayerInputAfter { get; }

            public string BridgeExpectedSlotAfter { get; }

            public string ActivationPlayerInputAfter { get; }

            public string ActivationExpectedSlotAfter { get; }

            public string ActivationActionMapAfter { get; }

            public string TrackingTargetAfter { get; }

            public string LookAtTargetAfter { get; }

            public bool TypedReferencesRepaired { get; }

            public string FailureReason { get; }

            public static RepairProofSnapshot Failed(string failureReason)
            {
                return new RepairProofSnapshot(
                    false,
                    "<none>",
                    "<none>",
                    "<none>",
                    false,
                    "<none>",
                    "<none>",
                    "<none>",
                    0,
                    0,
                    false,
                    false,
                    false,
                    false,
                    false,
                    true,
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    false,
                    failureReason);
            }

            public static RepairProofSnapshot Failed(
                string failureReason,
                FirstGameResolvedPlayer resolvedPlayer,
                int anchorHostCount = 0,
                int matchingAnchorHosts = 0)
            {
                return new RepairProofSnapshot(
                    false,
                    resolvedPlayer.PlayerObjectName,
                    resolvedPlayer.IdentitySource,
                    resolvedPlayer.ResolutionSource,
                    resolvedPlayer.ResolvedByName,
                    resolvedPlayer.ActorId,
                    resolvedPlayer.PlayerSlotId,
                    "<none>",
                    anchorHostCount,
                    matchingAnchorHosts,
                    false,
                    false,
                    false,
                    false,
                    false,
                    true,
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    "<none>",
                    false,
                    failureReason);
            }

            public static RepairProofSnapshot FromState(
                bool succeeded,
                FirstGameResolvedPlayer resolvedPlayer,
                FrameworkCameraAnchorHost anchorHost,
                int anchorHostCount,
                int matchingAnchorHosts,
                BindingState state,
                bool initialCanonical,
                bool controlledDriftCreated,
                bool facadeApplyInvoked,
                bool repairSucceeded,
                bool restoredOriginalOnFailure,
                bool selectionRestored,
                string failureReason)
            {
                bool typedReferencesRepaired = repairSucceeded && state.IsCanonical(resolvedPlayer);
                return new RepairProofSnapshot(
                    succeeded,
                    resolvedPlayer.PlayerObjectName,
                    resolvedPlayer.IdentitySource,
                    resolvedPlayer.ResolutionSource,
                    resolvedPlayer.ResolvedByName,
                    resolvedPlayer.ActorId,
                    resolvedPlayer.PlayerSlotId,
                    anchorHost != null ? anchorHost.name : "<none>",
                    anchorHostCount,
                    matchingAnchorHosts,
                    initialCanonical,
                    controlledDriftCreated,
                    facadeApplyInvoked,
                    repairSucceeded,
                    restoredOriginalOnFailure,
                    selectionRestored,
                    state.BridgePlayerInput != null ? state.BridgePlayerInput.name : "<none>",
                    string.IsNullOrWhiteSpace(state.BridgeExpectedSlot) ? "<none>" : state.BridgeExpectedSlot,
                    state.ActivationPlayerInput != null ? state.ActivationPlayerInput.name : "<none>",
                    string.IsNullOrWhiteSpace(state.ActivationExpectedSlot) ? "<none>" : state.ActivationExpectedSlot,
                    string.IsNullOrWhiteSpace(state.ActivationActionMap) ? "<none>" : state.ActivationActionMap,
                    state.TrackingTarget != null ? state.TrackingTarget.name : "<none>",
                    state.LookAtTarget != null ? state.LookAtTarget.name : "<none>",
                    typedReferencesRepaired,
                    failureReason);
            }
        }
    }
}
