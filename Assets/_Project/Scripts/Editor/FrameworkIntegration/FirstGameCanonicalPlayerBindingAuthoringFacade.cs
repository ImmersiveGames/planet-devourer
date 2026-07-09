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
    /// FIRSTGAME editor-only authoring facade for the already-proven real player binding chain.
    /// This tool centralizes typed references for identity, PlayerInput/F52 targets and camera anchors.
    /// It does not create runtime lifecycle, route InputActions, enable movement, spawn actors, execute gameplay or save progression.
    /// </summary>
    public static class FirstGameCanonicalPlayerBindingAuthoringFacade
    {
        private const string ValidateMenuPath = "FIRSTGAME/Immersive Framework/Validate Canonical Player Binding Facade";
        private const string ApplyMenuPath = "FIRSTGAME/Immersive Framework/Apply Canonical Player Binding Facade";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";
        private const string LogTag = "[F53D_FIRSTGAME_PLAYER_BINDING_FACADE]";

        [MenuItem(ValidateMenuPath)]
        public static void ValidateCanonicalPlayerBindingFacade()
        {
            FacadeSnapshot snapshot = BuildSnapshot(apply: false);
            LogSnapshot(snapshot);
        }

        [MenuItem(ApplyMenuPath)]
        public static void ApplyCanonicalPlayerBindingFacade()
        {
            FacadeSnapshot snapshot = BuildSnapshot(apply: true);
            LogSnapshot(snapshot);
        }

        private static FacadeSnapshot BuildSnapshot(bool apply)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                return FacadeSnapshot.Failed(apply, "InvalidScene");
            }

            if (!string.Equals(scene.path, GameplayScenePath, System.StringComparison.Ordinal))
            {
                return FacadeSnapshot.Failed(apply, "UnexpectedActiveScene");
            }

            if (!FirstGamePlayerIdentityResolver.TryResolveCanonicalPlayer(
                    scene,
                    Selection.activeGameObject,
                    out FirstGameResolvedPlayer resolvedPlayer,
                    out string failureReason))
            {
                return FacadeSnapshot.Failed(apply, failureReason, resolvedPlayer);
            }

            GameObject playerObject = resolvedPlayer.GameObject;
            if (playerObject == null)
            {
                return FacadeSnapshot.Failed(apply, "MissingResolvedPlayerObject", resolvedPlayer);
            }

            UndoState undoState = default;
            if (apply)
            {
                Undo.SetCurrentGroupName("Apply FIRSTGAME Canonical Player Binding Facade");
                undoState = new UndoState(Undo.GetCurrentGroup());
            }

            ComponentCreationFlags creationFlags = default;
            PlayerControlBindingTargetBehaviour controlTarget = GetOrCreateComponent<PlayerControlBindingTargetBehaviour>(playerObject, apply, ref creationFlags.CreatedPlayerControlBindingTarget);
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget = GetOrCreateComponent<UnityPlayerInputBridgeTargetBehaviour>(playerObject, apply, ref creationFlags.CreatedUnityPlayerInputBridgeTarget);
            UnityPlayerInputActivationTargetBehaviour activationTarget = GetOrCreateComponent<UnityPlayerInputActivationTargetBehaviour>(playerObject, apply, ref creationFlags.CreatedUnityPlayerInputActivationTarget);

            if (apply)
            {
                if (controlTarget != null)
                {
                    ConfigurePlayerControlTarget(controlTarget, playerObject.name);
                }

                if (bridgeTarget != null)
                {
                    ConfigureBridgeTarget(bridgeTarget, playerObject.name, resolvedPlayer.PlayerInput);
                }

                if (activationTarget != null)
                {
                    ConfigureActivationTarget(activationTarget, playerObject.name, resolvedPlayer.PlayerInput);
                }
            }

            IReadOnlyList<FrameworkCameraAnchorHost> anchorHosts = FindSceneComponents<FrameworkCameraAnchorHost>(scene);
            FrameworkCameraAnchorHost anchorHost = ResolveAnchorHost(anchorHosts, resolvedPlayer.Transform, apply, out string anchorFailureReason);
            if (anchorHost == null)
            {
                return FacadeSnapshot.FromResolved(
                    apply,
                    resolvedPlayer,
                    controlTarget,
                    bridgeTarget,
                    activationTarget,
                    null,
                    anchorHosts.Count,
                    matchingAnchorHosts: 0,
                    creationFlags,
                    anchorFailureReason);
            }

            if (apply)
            {
                ConfigureAnchorHost(anchorHost, resolvedPlayer.Transform);
                EditorUtility.SetDirty(playerObject);
                EditorUtility.SetDirty(anchorHost);
                EditorSceneManager.MarkSceneDirty(scene);
                Undo.CollapseUndoOperations(undoState.GroupId);
            }

            int matchingAnchors = CountMatchingAnchorHosts(anchorHosts, resolvedPlayer.Transform);
            FacadeSnapshot snapshot = FacadeSnapshot.FromResolved(
                apply,
                resolvedPlayer,
                controlTarget,
                bridgeTarget,
                activationTarget,
                anchorHost,
                anchorHosts.Count,
                matchingAnchors,
                creationFlags,
                "None");

            return snapshot;
        }

        private static T GetOrCreateComponent<T>(GameObject owner, bool apply, ref bool created) where T : Component
        {
            T component = owner.GetComponent<T>();
            if (component != null || !apply)
            {
                return component;
            }

            created = true;
            return Undo.AddComponent<T>(owner);
        }

        private static FrameworkCameraAnchorHost ResolveAnchorHost(
            IReadOnlyList<FrameworkCameraAnchorHost> anchorHosts,
            Transform playerTransform,
            bool apply,
            out string failureReason)
        {
            failureReason = "None";
            if (anchorHosts.Count == 0)
            {
                failureReason = "MissingFrameworkCameraAnchorHost";
                return null;
            }

            FrameworkCameraAnchorHost matching = null;
            int matchingCount = 0;
            for (int i = 0; i < anchorHosts.Count; i++)
            {
                FrameworkCameraAnchorHost candidate = anchorHosts[i];
                if (candidate == null)
                {
                    continue;
                }

                FirstGameCanonicalPlayerBindingAuthoringFacade.ReadAnchorTargets(candidate, out Transform trackingTarget, out Transform lookAtTarget);
                if (ReferenceEquals(trackingTarget, playerTransform) && ReferenceEquals(lookAtTarget, playerTransform))
                {
                    matching = candidate;
                    matchingCount++;
                }
            }

            if (matchingCount == 1)
            {
                return matching;
            }

            if (matchingCount > 1)
            {
                failureReason = "MultipleMatchingFrameworkCameraAnchorHosts";
                return null;
            }

            if (anchorHosts.Count == 1)
            {
                return anchorHosts[0];
            }

            failureReason = apply ? "AmbiguousFrameworkCameraAnchorHosts" : "NoMatchingFrameworkCameraAnchorHost";
            return null;
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

        private static int CountMatchingAnchorHosts(IReadOnlyList<FrameworkCameraAnchorHost> anchorHosts, Transform playerTransform)
        {
            int count = 0;
            for (int i = 0; i < anchorHosts.Count; i++)
            {
                FrameworkCameraAnchorHost anchorHost = anchorHosts[i];
                if (anchorHost == null)
                {
                    continue;
                }

                FirstGameCanonicalPlayerBindingAuthoringFacade.ReadAnchorTargets(anchorHost, out Transform trackingTarget, out Transform lookAtTarget);
                if (ReferenceEquals(trackingTarget, playerTransform) && ReferenceEquals(lookAtTarget, playerTransform))
                {
                    count++;
                }
            }

            return count;
        }

        private static void ConfigurePlayerControlTarget(PlayerControlBindingTargetBehaviour component, string playerObjectName)
        {
            SerializedObject serialized = new SerializedObject(component);
            SetStringIfExists(serialized, "bindingTargetName", $"FIRSTGAME {playerObjectName} PlayerControl Binding Target");
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
        }

        private static void ConfigureBridgeTarget(UnityPlayerInputBridgeTargetBehaviour component, string playerObjectName, PlayerInput playerInput)
        {
            SerializedObject serialized = new SerializedObject(component);
            SetStringIfExists(serialized, "bridgeTargetName", $"FIRSTGAME {playerObjectName} Unity PlayerInput Bridge Target");
            SetStringIfExists(serialized, "expectedPlayerSlotId", FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw);
            SetObjectIfExists(serialized, "playerInput", playerInput);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
        }

        private static void ConfigureActivationTarget(UnityPlayerInputActivationTargetBehaviour component, string playerObjectName, PlayerInput playerInput)
        {
            SerializedObject serialized = new SerializedObject(component);
            SetStringIfExists(serialized, "activationTargetName", $"FIRSTGAME {playerObjectName} Unity PlayerInput Activation Target");
            SetStringIfExists(serialized, "expectedPlayerSlotId", FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw);
            SetObjectIfExists(serialized, "playerInput", playerInput);
            SetStringIfExists(serialized, "actionMapName", FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
        }

        private static void ConfigureAnchorHost(FrameworkCameraAnchorHost anchorHost, Transform playerTransform)
        {
            SerializedObject serialized = new SerializedObject(anchorHost);
            SetObjectIfExists(serialized, "trackingTarget", playerTransform);
            SetObjectIfExists(serialized, "lookAtTarget", playerTransform);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(anchorHost);
        }

        private static void SetStringIfExists(SerializedObject serialized, string propertyName, string value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value;
            }
        }

        private static void SetObjectIfExists(SerializedObject serialized, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
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

        private static void LogSnapshot(FacadeSnapshot snapshot)
        {
            string status = snapshot.Succeeded ? "Succeeded" : "Failed";
            Debug.Log(
                LogTag + " " +
                $"status='{status}' " +
                $"mode='{snapshot.Mode}' " +
                $"playerObject='{snapshot.PlayerObjectName}' " +
                $"identitySource='{snapshot.IdentitySource}' " +
                $"resolutionSource='{snapshot.ResolutionSource}' " +
                $"resolvedByName='{snapshot.ResolvedByName}' " +
                $"actorId='{snapshot.ActorId}' " +
                $"playerSlotId='{snapshot.PlayerSlotId}' " +
                $"playerInput='{snapshot.HasPlayerInput}' " +
                $"inputActions='{snapshot.HasInputActions}' " +
                $"expectedGameplayActionMap='{FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap}' " +
                $"expectedGameplayActionMapFound='{snapshot.HasExpectedGameplayActionMap}' " +
                $"playerControlBindingTarget='{snapshot.HasPlayerControlBindingTarget}' " +
                $"unityPlayerInputBridgeTarget='{snapshot.HasUnityPlayerInputBridgeTarget}' " +
                $"unityPlayerInputBridgePlayerInputReference='{snapshot.UnityPlayerInputBridgePlayerInputReference}' " +
                $"unityPlayerInputBridgePlayerInputMatchesResolved='{snapshot.UnityPlayerInputBridgePlayerInputMatchesResolved}' " +
                $"unityPlayerInputBridgeExpectedSlot='{snapshot.UnityPlayerInputBridgeExpectedSlot}' " +
                $"unityPlayerInputBridgeExpectedSlotMatches='{snapshot.UnityPlayerInputBridgeExpectedSlotMatches}' " +
                $"unityPlayerInputActivationTarget='{snapshot.HasUnityPlayerInputActivationTarget}' " +
                $"unityPlayerInputActivationPlayerInputReference='{snapshot.UnityPlayerInputActivationPlayerInputReference}' " +
                $"unityPlayerInputActivationPlayerInputMatchesResolved='{snapshot.UnityPlayerInputActivationPlayerInputMatchesResolved}' " +
                $"unityPlayerInputActivationExpectedSlot='{snapshot.UnityPlayerInputActivationExpectedSlot}' " +
                $"unityPlayerInputActivationExpectedSlotMatches='{snapshot.UnityPlayerInputActivationExpectedSlotMatches}' " +
                $"unityPlayerInputActivationActionMap='{snapshot.UnityPlayerInputActivationActionMap}' " +
                $"unityPlayerInputActivationActionMapMatchesExpected='{snapshot.UnityPlayerInputActivationActionMapMatchesExpected}' " +
                $"anchorHost='{snapshot.AnchorHostName}' " +
                $"anchorHostCount='{snapshot.AnchorHostCount}' " +
                $"matchingAnchorHosts='{snapshot.MatchingAnchorHosts}' " +
                $"trackingTarget='{snapshot.TrackingTargetName}' " +
                $"lookAtTarget='{snapshot.LookAtTargetName}' " +
                $"trackingTargetMatchesPlayer='{snapshot.TrackingTargetMatchesPlayer}' " +
                $"lookAtTargetMatchesPlayer='{snapshot.LookAtTargetMatchesPlayer}' " +
                $"typedTransformBinding='{snapshot.TypedTransformBinding}' " +
                $"targetIsPlayerRoot='{snapshot.TargetIsPlayerRoot}' " +
                $"facadeCentralizedReferences='{snapshot.FacadeCentralizedReferences}' " +
                $"createdPlayerControlBindingTarget='{snapshot.CreatedPlayerControlBindingTarget}' " +
                $"createdUnityPlayerInputBridgeTarget='{snapshot.CreatedUnityPlayerInputBridgeTarget}' " +
                $"createdUnityPlayerInputActivationTarget='{snapshot.CreatedUnityPlayerInputActivationTarget}' " +
                "createdTestObject='False' createdPlayerInput='False' movement='False' actorSpawning='False' gameplayCommandExecution='False' " +
                $"failureReason='{snapshot.FailureReason}'.");
        }

        private readonly struct UndoState
        {
            public UndoState(int groupId)
            {
                GroupId = groupId;
            }

            public int GroupId { get; }
        }

        private struct ComponentCreationFlags
        {
            public bool CreatedPlayerControlBindingTarget;
            public bool CreatedUnityPlayerInputBridgeTarget;
            public bool CreatedUnityPlayerInputActivationTarget;
        }

        private readonly struct FacadeSnapshot
        {
            private FacadeSnapshot(
                bool apply,
                bool succeeded,
                string playerObjectName,
                string identitySource,
                string resolutionSource,
                bool resolvedByName,
                string actorId,
                string playerSlotId,
                bool hasPlayerInput,
                bool hasInputActions,
                bool hasExpectedGameplayActionMap,
                bool hasPlayerControlBindingTarget,
                bool hasUnityPlayerInputBridgeTarget,
                string unityPlayerInputBridgePlayerInputReference,
                bool unityPlayerInputBridgePlayerInputMatchesResolved,
                string unityPlayerInputBridgeExpectedSlot,
                bool unityPlayerInputBridgeExpectedSlotMatches,
                bool hasUnityPlayerInputActivationTarget,
                string unityPlayerInputActivationPlayerInputReference,
                bool unityPlayerInputActivationPlayerInputMatchesResolved,
                string unityPlayerInputActivationExpectedSlot,
                bool unityPlayerInputActivationExpectedSlotMatches,
                string unityPlayerInputActivationActionMap,
                bool unityPlayerInputActivationActionMapMatchesExpected,
                string anchorHostName,
                int anchorHostCount,
                int matchingAnchorHosts,
                string trackingTargetName,
                string lookAtTargetName,
                bool trackingTargetMatchesPlayer,
                bool lookAtTargetMatchesPlayer,
                bool typedTransformBinding,
                bool targetIsPlayerRoot,
                bool facadeCentralizedReferences,
                bool createdPlayerControlBindingTarget,
                bool createdUnityPlayerInputBridgeTarget,
                bool createdUnityPlayerInputActivationTarget,
                string failureReason)
            {
                Mode = apply ? "apply" : "validate";
                Succeeded = succeeded;
                PlayerObjectName = playerObjectName;
                IdentitySource = identitySource;
                ResolutionSource = resolutionSource;
                ResolvedByName = resolvedByName;
                ActorId = actorId;
                PlayerSlotId = playerSlotId;
                HasPlayerInput = hasPlayerInput;
                HasInputActions = hasInputActions;
                HasExpectedGameplayActionMap = hasExpectedGameplayActionMap;
                HasPlayerControlBindingTarget = hasPlayerControlBindingTarget;
                HasUnityPlayerInputBridgeTarget = hasUnityPlayerInputBridgeTarget;
                UnityPlayerInputBridgePlayerInputReference = unityPlayerInputBridgePlayerInputReference;
                UnityPlayerInputBridgePlayerInputMatchesResolved = unityPlayerInputBridgePlayerInputMatchesResolved;
                UnityPlayerInputBridgeExpectedSlot = unityPlayerInputBridgeExpectedSlot;
                UnityPlayerInputBridgeExpectedSlotMatches = unityPlayerInputBridgeExpectedSlotMatches;
                HasUnityPlayerInputActivationTarget = hasUnityPlayerInputActivationTarget;
                UnityPlayerInputActivationPlayerInputReference = unityPlayerInputActivationPlayerInputReference;
                UnityPlayerInputActivationPlayerInputMatchesResolved = unityPlayerInputActivationPlayerInputMatchesResolved;
                UnityPlayerInputActivationExpectedSlot = unityPlayerInputActivationExpectedSlot;
                UnityPlayerInputActivationExpectedSlotMatches = unityPlayerInputActivationExpectedSlotMatches;
                UnityPlayerInputActivationActionMap = unityPlayerInputActivationActionMap;
                UnityPlayerInputActivationActionMapMatchesExpected = unityPlayerInputActivationActionMapMatchesExpected;
                AnchorHostName = anchorHostName;
                AnchorHostCount = anchorHostCount;
                MatchingAnchorHosts = matchingAnchorHosts;
                TrackingTargetName = trackingTargetName;
                LookAtTargetName = lookAtTargetName;
                TrackingTargetMatchesPlayer = trackingTargetMatchesPlayer;
                LookAtTargetMatchesPlayer = lookAtTargetMatchesPlayer;
                TypedTransformBinding = typedTransformBinding;
                TargetIsPlayerRoot = targetIsPlayerRoot;
                FacadeCentralizedReferences = facadeCentralizedReferences;
                CreatedPlayerControlBindingTarget = createdPlayerControlBindingTarget;
                CreatedUnityPlayerInputBridgeTarget = createdUnityPlayerInputBridgeTarget;
                CreatedUnityPlayerInputActivationTarget = createdUnityPlayerInputActivationTarget;
                FailureReason = string.IsNullOrWhiteSpace(failureReason) ? "None" : failureReason;
            }

            public string Mode { get; }
            public bool Succeeded { get; }
            public string PlayerObjectName { get; }
            public string IdentitySource { get; }
            public string ResolutionSource { get; }
            public bool ResolvedByName { get; }
            public string ActorId { get; }
            public string PlayerSlotId { get; }
            public bool HasPlayerInput { get; }
            public bool HasInputActions { get; }
            public bool HasExpectedGameplayActionMap { get; }
            public bool HasPlayerControlBindingTarget { get; }
            public bool HasUnityPlayerInputBridgeTarget { get; }
            public string UnityPlayerInputBridgePlayerInputReference { get; }
            public bool UnityPlayerInputBridgePlayerInputMatchesResolved { get; }
            public string UnityPlayerInputBridgeExpectedSlot { get; }
            public bool UnityPlayerInputBridgeExpectedSlotMatches { get; }
            public bool HasUnityPlayerInputActivationTarget { get; }
            public string UnityPlayerInputActivationPlayerInputReference { get; }
            public bool UnityPlayerInputActivationPlayerInputMatchesResolved { get; }
            public string UnityPlayerInputActivationExpectedSlot { get; }
            public bool UnityPlayerInputActivationExpectedSlotMatches { get; }
            public string UnityPlayerInputActivationActionMap { get; }
            public bool UnityPlayerInputActivationActionMapMatchesExpected { get; }
            public string AnchorHostName { get; }
            public int AnchorHostCount { get; }
            public int MatchingAnchorHosts { get; }
            public string TrackingTargetName { get; }
            public string LookAtTargetName { get; }
            public bool TrackingTargetMatchesPlayer { get; }
            public bool LookAtTargetMatchesPlayer { get; }
            public bool TypedTransformBinding { get; }
            public bool TargetIsPlayerRoot { get; }
            public bool FacadeCentralizedReferences { get; }
            public bool CreatedPlayerControlBindingTarget { get; }
            public bool CreatedUnityPlayerInputBridgeTarget { get; }
            public bool CreatedUnityPlayerInputActivationTarget { get; }
            public string FailureReason { get; }

            public static FacadeSnapshot Failed(bool apply, string failureReason)
            {
                return Failed(apply, failureReason, default);
            }

            public static FacadeSnapshot Failed(bool apply, string failureReason, FirstGameResolvedPlayer resolvedPlayer)
            {
                return new FacadeSnapshot(
                    apply,
                    false,
                    resolvedPlayer.GameObject != null ? resolvedPlayer.PlayerObjectName : "<none>",
                    string.IsNullOrWhiteSpace(resolvedPlayer.IdentitySource) ? "<none>" : resolvedPlayer.IdentitySource,
                    string.IsNullOrWhiteSpace(resolvedPlayer.ResolutionSource) ? "<none>" : resolvedPlayer.ResolutionSource,
                    resolvedPlayer.ResolvedByName,
                    string.IsNullOrWhiteSpace(resolvedPlayer.ActorId) ? "<none>" : resolvedPlayer.ActorId,
                    string.IsNullOrWhiteSpace(resolvedPlayer.PlayerSlotId) ? "<none>" : resolvedPlayer.PlayerSlotId,
                    false,
                    false,
                    false,
                    false,
                    false,
                    "<none>",
                    false,
                    "<none>",
                    false,
                    false,
                    "<none>",
                    false,
                    "<none>",
                    false,
                    "<none>",
                    false,
                    "<none>",
                    0,
                    0,
                    "<none>",
                    "<none>",
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    failureReason);
            }

            public static FacadeSnapshot FromResolved(
                bool apply,
                FirstGameResolvedPlayer resolvedPlayer,
                PlayerControlBindingTargetBehaviour controlTarget,
                UnityPlayerInputBridgeTargetBehaviour bridgeTarget,
                UnityPlayerInputActivationTargetBehaviour activationTarget,
                FrameworkCameraAnchorHost anchorHost,
                int anchorHostCount,
                int matchingAnchorHosts,
                ComponentCreationFlags creationFlags,
                string incomingFailureReason)
            {
                PlayerInput playerInput = resolvedPlayer.PlayerInput;
                bool hasPlayerInput = playerInput != null;
                bool hasInputActions = playerInput != null && playerInput.actions != null;
                bool hasExpectedGameplayActionMap = FirstGamePlayerIdentityResolver.HasExpectedGameplayActionMap(playerInput);

                PlayerInput bridgePlayerInput = FirstGameCanonicalPlayerBindingAuthoringFacade.GetObjectIfExists(bridgeTarget, "playerInput") as PlayerInput;
                string bridgeExpectedSlot = FirstGameCanonicalPlayerBindingAuthoringFacade.GetStringIfExists(bridgeTarget, "expectedPlayerSlotId");
                bool bridgePlayerInputMatchesResolved = bridgePlayerInput != null && ReferenceEquals(bridgePlayerInput, playerInput);
                bool bridgeExpectedSlotMatches = string.Equals(bridgeExpectedSlot, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal);

                PlayerInput activationPlayerInput = FirstGameCanonicalPlayerBindingAuthoringFacade.GetObjectIfExists(activationTarget, "playerInput") as PlayerInput;
                string activationExpectedSlot = FirstGameCanonicalPlayerBindingAuthoringFacade.GetStringIfExists(activationTarget, "expectedPlayerSlotId");
                string activationActionMap = FirstGameCanonicalPlayerBindingAuthoringFacade.GetStringIfExists(activationTarget, "actionMapName");
                bool activationPlayerInputMatchesResolved = activationPlayerInput != null && ReferenceEquals(activationPlayerInput, playerInput);
                bool activationExpectedSlotMatches = string.Equals(activationExpectedSlot, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal);
                bool activationActionMapMatchesExpected = string.Equals(activationActionMap, FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap, System.StringComparison.Ordinal);

                Transform trackingTarget = null;
                Transform lookAtTarget = null;
                if (anchorHost != null)
                {
                    FirstGameCanonicalPlayerBindingAuthoringFacade.ReadAnchorTargets(anchorHost, out trackingTarget, out lookAtTarget);
                }

                bool trackingTargetMatchesPlayer = trackingTarget != null && ReferenceEquals(trackingTarget, resolvedPlayer.Transform);
                bool lookAtTargetMatchesPlayer = lookAtTarget != null && ReferenceEquals(lookAtTarget, resolvedPlayer.Transform);
                bool typedTransformBinding = trackingTargetMatchesPlayer && lookAtTargetMatchesPlayer;
                bool targetIsPlayerRoot = typedTransformBinding && ReferenceEquals(trackingTarget, resolvedPlayer.GameObject.transform);

                bool facadeCentralizedReferences =
                    resolvedPlayer.HasExpectedIdentity &&
                    !resolvedPlayer.ResolvedByName &&
                    hasPlayerInput &&
                    hasInputActions &&
                    hasExpectedGameplayActionMap &&
                    controlTarget != null &&
                    bridgeTarget != null &&
                    bridgePlayerInputMatchesResolved &&
                    bridgeExpectedSlotMatches &&
                    activationTarget != null &&
                    activationPlayerInputMatchesResolved &&
                    activationExpectedSlotMatches &&
                    activationActionMapMatchesExpected &&
                    typedTransformBinding &&
                    targetIsPlayerRoot;

                string failureReason = BuildFailureReason(
                    incomingFailureReason,
                    resolvedPlayer.HasExpectedIdentity,
                    resolvedPlayer.ResolvedByName,
                    hasPlayerInput,
                    hasInputActions,
                    hasExpectedGameplayActionMap,
                    controlTarget != null,
                    bridgeTarget != null,
                    bridgePlayerInputMatchesResolved,
                    bridgeExpectedSlotMatches,
                    activationTarget != null,
                    activationPlayerInputMatchesResolved,
                    activationExpectedSlotMatches,
                    activationActionMapMatchesExpected,
                    anchorHost != null,
                    typedTransformBinding,
                    targetIsPlayerRoot);

                bool succeeded = string.Equals(failureReason, "None", System.StringComparison.Ordinal) && facadeCentralizedReferences;
                return new FacadeSnapshot(
                    apply,
                    succeeded,
                    resolvedPlayer.PlayerObjectName,
                    resolvedPlayer.IdentitySource,
                    resolvedPlayer.ResolutionSource,
                    resolvedPlayer.ResolvedByName,
                    resolvedPlayer.ActorId,
                    resolvedPlayer.PlayerSlotId,
                    hasPlayerInput,
                    hasInputActions,
                    hasExpectedGameplayActionMap,
                    controlTarget != null,
                    bridgeTarget != null,
                    bridgePlayerInput != null ? bridgePlayerInput.name : "<none>",
                    bridgePlayerInputMatchesResolved,
                    string.IsNullOrWhiteSpace(bridgeExpectedSlot) ? "<none>" : bridgeExpectedSlot,
                    bridgeExpectedSlotMatches,
                    activationTarget != null,
                    activationPlayerInput != null ? activationPlayerInput.name : "<none>",
                    activationPlayerInputMatchesResolved,
                    string.IsNullOrWhiteSpace(activationExpectedSlot) ? "<none>" : activationExpectedSlot,
                    activationExpectedSlotMatches,
                    string.IsNullOrWhiteSpace(activationActionMap) ? "<none>" : activationActionMap,
                    activationActionMapMatchesExpected,
                    anchorHost != null ? anchorHost.name : "<none>",
                    anchorHostCount,
                    matchingAnchorHosts,
                    trackingTarget != null ? trackingTarget.name : "<none>",
                    lookAtTarget != null ? lookAtTarget.name : "<none>",
                    trackingTargetMatchesPlayer,
                    lookAtTargetMatchesPlayer,
                    typedTransformBinding,
                    targetIsPlayerRoot,
                    facadeCentralizedReferences,
                    creationFlags.CreatedPlayerControlBindingTarget,
                    creationFlags.CreatedUnityPlayerInputBridgeTarget,
                    creationFlags.CreatedUnityPlayerInputActivationTarget,
                    failureReason);
            }

            private static string BuildFailureReason(
                string incomingFailureReason,
                bool expectedIdentity,
                bool resolvedByName,
                bool hasPlayerInput,
                bool hasInputActions,
                bool hasExpectedGameplayActionMap,
                bool hasPlayerControlTarget,
                bool hasBridgeTarget,
                bool bridgePlayerInputMatchesResolved,
                bool bridgeExpectedSlotMatches,
                bool hasActivationTarget,
                bool activationPlayerInputMatchesResolved,
                bool activationExpectedSlotMatches,
                bool activationActionMapMatchesExpected,
                bool hasAnchorHost,
                bool typedTransformBinding,
                bool targetIsPlayerRoot)
            {
                if (!string.IsNullOrWhiteSpace(incomingFailureReason) && incomingFailureReason != "None")
                {
                    return incomingFailureReason;
                }

                if (!expectedIdentity)
                {
                    return "MissingCanonicalPlayerIdentity";
                }

                if (resolvedByName)
                {
                    return "ResolvedByName";
                }

                if (!hasPlayerInput)
                {
                    return "MissingPlayerInput";
                }

                if (!hasInputActions)
                {
                    return "MissingInputActions";
                }

                if (!hasExpectedGameplayActionMap)
                {
                    return "MissingPlayerActionMap";
                }

                if (!hasPlayerControlTarget)
                {
                    return "MissingPlayerControlBindingTarget";
                }

                if (!hasBridgeTarget)
                {
                    return "MissingUnityPlayerInputBridgeTarget";
                }

                if (!bridgePlayerInputMatchesResolved)
                {
                    return "UnityPlayerInputBridgeReferenceMismatch";
                }

                if (!bridgeExpectedSlotMatches)
                {
                    return "UnityPlayerInputBridgeExpectedSlotMismatch";
                }

                if (!hasActivationTarget)
                {
                    return "MissingUnityPlayerInputActivationTarget";
                }

                if (!activationPlayerInputMatchesResolved)
                {
                    return "UnityPlayerInputActivationReferenceMismatch";
                }

                if (!activationExpectedSlotMatches)
                {
                    return "UnityPlayerInputActivationExpectedSlotMismatch";
                }

                if (!activationActionMapMatchesExpected)
                {
                    return "UnityPlayerInputActivationActionMapMismatch";
                }

                if (!hasAnchorHost)
                {
                    return "MissingFrameworkCameraAnchorHost";
                }

                if (!typedTransformBinding)
                {
                    return "CameraAnchorTargetMismatch";
                }

                if (!targetIsPlayerRoot)
                {
                    return "CameraTargetIsNotPlayerRoot";
                }

                return "None";
            }
        }
    }
}
