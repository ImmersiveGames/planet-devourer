using Immersive.Framework.Camera;
using Immersive.Framework.PlayerBinding;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkIntegration.Editor
{
    /// <summary>
    /// FIRSTGAME editor-only full proof for the accepted real player binding chain.
    /// This validates canonical player identity, F52 PlayerControl/PlayerInput binding evidence and the current route/activity camera target.
    /// It does not route InputActions, enable movement, spawn actors, execute gameplay commands, create temporary objects or save progression.
    /// </summary>
    public static class FirstGameFullPlayerBindingChainValidator
    {
        private const string ValidateMenuPath = "FIRSTGAME/Immersive Framework/Validate Full Player Binding Chain";
        private const string ExpectedSceneName = "FG_Gameplay";
        private const string TrackingTargetPropertyName = "trackingTarget";
        private const string LookAtTargetPropertyName = "lookAtTarget";
        private const string PlayerInputPropertyName = "playerInput";
        private const string ExpectedPlayerSlotIdPropertyName = "expectedPlayerSlotId";
        private const string ActionMapNamePropertyName = "actionMapName";

        private const string PlayerViewBindingTargetTypeName = "PlayerViewBindingTargetBehaviour";
        private const string PlayerViewCameraTargetBindingTargetTypeName = "PlayerViewCameraTargetBindingTargetBehaviour";
        private const string PlayerViewCameraActivationTargetTypeName = "PlayerViewCameraActivationTargetBehaviour";

        [MenuItem(ValidateMenuPath)]
        public static void ValidateFullPlayerBindingChain()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                LogFailure("InvalidScene");
                return;
            }

            if (!string.Equals(scene.name, ExpectedSceneName, System.StringComparison.Ordinal))
            {
                LogFailure($"UnexpectedActiveScene:{scene.name}");
                return;
            }

            if (!FirstGamePlayerIdentityResolver.TryResolveCanonicalPlayer(
                    scene,
                    Selection.activeGameObject,
                    out FirstGameResolvedPlayer resolvedPlayer,
                    out string playerFailureReason))
            {
                LogFailure(playerFailureReason);
                return;
            }

            PlayerInput playerInput = resolvedPlayer.PlayerInput;
            GameObject playerObject = resolvedPlayer.GameObject;

            bool hasPlayerInput = playerInput != null;
            bool hasInputActions = playerInput != null && playerInput.actions != null;
            bool hasExpectedActionMap = FirstGamePlayerIdentityResolver.HasExpectedGameplayActionMap(playerInput);

            PlayerControlBindingTargetBehaviour controlTarget = playerObject != null
                ? playerObject.GetComponent<PlayerControlBindingTargetBehaviour>()
                : null;
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget = playerObject != null
                ? playerObject.GetComponent<UnityPlayerInputBridgeTargetBehaviour>()
                : null;
            UnityPlayerInputActivationTargetBehaviour activationTarget = playerObject != null
                ? playerObject.GetComponent<UnityPlayerInputActivationTargetBehaviour>()
                : null;

            bool hasControlTarget = controlTarget != null;
            bool hasBridgeTarget = bridgeTarget != null;
            bool hasActivationTarget = activationTarget != null;

            PlayerInput bridgePlayerInputReference = ReadObjectReference<PlayerInput>(bridgeTarget, PlayerInputPropertyName);
            PlayerInput activationPlayerInputReference = ReadObjectReference<PlayerInput>(activationTarget, PlayerInputPropertyName);
            string bridgeExpectedSlotId = ReadString(bridgeTarget, ExpectedPlayerSlotIdPropertyName);
            string activationExpectedSlotId = ReadString(activationTarget, ExpectedPlayerSlotIdPropertyName);
            string activationActionMapName = ReadString(activationTarget, ActionMapNamePropertyName);

            bool bridgePlayerInputReferenceMatches = bridgePlayerInputReference == playerInput;
            bool activationPlayerInputReferenceMatches = activationPlayerInputReference == playerInput;
            bool bridgeExpectedSlotMatches = string.Equals(bridgeExpectedSlotId, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal);
            bool activationExpectedSlotMatches = string.Equals(activationExpectedSlotId, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal);
            bool activationActionMapMatchesExpected = string.Equals(activationActionMapName, FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap, System.StringComparison.Ordinal);

            FrameworkCameraAnchorHost[] anchorHosts = FindSceneComponents<FrameworkCameraAnchorHost>(scene);
            FrameworkCameraAnchorHost matchingAnchorHost = null;
            Transform matchingTrackingTarget = null;
            Transform matchingLookAtTarget = null;
            int matchingAnchorHosts = 0;

            for (int i = 0; i < anchorHosts.Length; i++)
            {
                FrameworkCameraAnchorHost host = anchorHosts[i];
                if (host == null)
                {
                    continue;
                }

                Transform trackingTarget = ReadObjectReference<Transform>(host, TrackingTargetPropertyName);
                Transform lookAtTarget = ReadObjectReference<Transform>(host, LookAtTargetPropertyName);
                if (trackingTarget == resolvedPlayer.Transform && lookAtTarget == resolvedPlayer.Transform)
                {
                    matchingAnchorHost = host;
                    matchingTrackingTarget = trackingTarget;
                    matchingLookAtTarget = lookAtTarget;
                    matchingAnchorHosts++;
                }
            }

            FrameworkCameraAnchorHost logAnchorHost = matchingAnchorHost;
            Transform logTrackingTarget = matchingTrackingTarget;
            Transform logLookAtTarget = matchingLookAtTarget;
            if (logAnchorHost == null && anchorHosts.Length > 0)
            {
                logAnchorHost = anchorHosts[0];
                logTrackingTarget = ReadObjectReference<Transform>(logAnchorHost, TrackingTargetPropertyName);
                logLookAtTarget = ReadObjectReference<Transform>(logAnchorHost, LookAtTargetPropertyName);
            }

            string failureReason = BuildFailureReason(
                hasPlayerInput,
                hasInputActions,
                hasExpectedActionMap,
                hasControlTarget,
                hasBridgeTarget,
                bridgePlayerInputReference != null,
                bridgePlayerInputReferenceMatches,
                bridgeExpectedSlotMatches,
                hasActivationTarget,
                activationPlayerInputReference != null,
                activationPlayerInputReferenceMatches,
                activationExpectedSlotMatches,
                activationActionMapMatchesExpected,
                anchorHosts.Length,
                matchingAnchorHosts);

            LogFullProof(
                resolvedPlayer,
                hasPlayerInput,
                hasInputActions,
                hasExpectedActionMap,
                hasControlTarget,
                hasBridgeTarget,
                bridgePlayerInputReference,
                bridgePlayerInputReferenceMatches,
                bridgeExpectedSlotId,
                bridgeExpectedSlotMatches,
                hasActivationTarget,
                activationPlayerInputReference,
                activationPlayerInputReferenceMatches,
                activationExpectedSlotId,
                activationExpectedSlotMatches,
                activationActionMapName,
                activationActionMapMatchesExpected,
                logAnchorHost,
                anchorHosts.Length,
                matchingAnchorHosts,
                logTrackingTarget,
                logLookAtTarget,
                failureReason);
        }

        private static string BuildFailureReason(
            bool hasPlayerInput,
            bool hasInputActions,
            bool hasExpectedActionMap,
            bool hasControlTarget,
            bool hasBridgeTarget,
            bool hasBridgePlayerInputReference,
            bool bridgePlayerInputReferenceMatches,
            bool bridgeExpectedSlotMatches,
            bool hasActivationTarget,
            bool hasActivationPlayerInputReference,
            bool activationPlayerInputReferenceMatches,
            bool activationExpectedSlotMatches,
            bool activationActionMapMatchesExpected,
            int anchorHostCount,
            int matchingAnchorHosts)
        {
            if (!hasPlayerInput)
            {
                return "MissingPlayerInput";
            }

            if (!hasInputActions)
            {
                return "MissingInputActions";
            }

            if (!hasExpectedActionMap)
            {
                return "MissingPlayerActionMap";
            }

            if (!hasControlTarget)
            {
                return "MissingPlayerControlBindingTarget";
            }

            if (!hasBridgeTarget)
            {
                return "MissingUnityPlayerInputBridgeTarget";
            }

            if (!hasBridgePlayerInputReference)
            {
                return "MissingUnityPlayerInputBridgePlayerInputReference";
            }

            if (!bridgePlayerInputReferenceMatches)
            {
                return "UnityPlayerInputBridgePlayerInputMismatch";
            }

            if (!bridgeExpectedSlotMatches)
            {
                return "UnityPlayerInputBridgeExpectedSlotMismatch";
            }

            if (!hasActivationTarget)
            {
                return "MissingUnityPlayerInputActivationTarget";
            }

            if (!hasActivationPlayerInputReference)
            {
                return "MissingUnityPlayerInputActivationPlayerInputReference";
            }

            if (!activationPlayerInputReferenceMatches)
            {
                return "UnityPlayerInputActivationPlayerInputMismatch";
            }

            if (!activationExpectedSlotMatches)
            {
                return "UnityPlayerInputActivationExpectedSlotMismatch";
            }

            if (!activationActionMapMatchesExpected)
            {
                return "UnityPlayerInputActivationActionMapMismatch";
            }

            if (anchorHostCount == 0)
            {
                return "MissingCameraAnchorHost";
            }

            if (matchingAnchorHosts == 0)
            {
                return "CameraAnchorHostTargetMismatch";
            }

            if (matchingAnchorHosts > 1)
            {
                return "MultipleMatchingCameraAnchorHosts";
            }

            return "None";
        }

        private static T ReadObjectReference<T>(Object target, string propertyName) where T : Object
        {
            if (target == null)
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            return property != null ? property.objectReferenceValue as T : null;
        }

        private static string ReadString(Object target, string propertyName)
        {
            if (target == null)
            {
                return string.Empty;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            return property != null ? property.stringValue : string.Empty;
        }

        private static T[] FindSceneComponents<T>(Scene scene) where T : Component
        {
            System.Collections.Generic.List<T> components = new System.Collections.Generic.List<T>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                components.AddRange(root.GetComponentsInChildren<T>(true));
            }

            return components.ToArray();
        }

        private static bool HasComponentByTypeName(GameObject target, string typeName)
        {
            if (target == null)
            {
                return false;
            }

            Component[] components = target.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    continue;
                }

                System.Type componentType = component.GetType();
                if (string.Equals(componentType.Name, typeName, System.StringComparison.Ordinal) ||
                    string.Equals(componentType.FullName, typeName, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void LogFailure(string failureReason)
        {
            Debug.Log(
                "[F53C3_FIRSTGAME_FULL_PLAYER_BINDING_CHAIN] " +
                "status='Failed' " +
                "playerObject='<none>' " +
                "identitySource='<none>' " +
                "resolutionSource='<none>' " +
                "resolvedByName='False' " +
                "actorId='<none>' " +
                "playerSlotId='<none>' " +
                "playerInput='False' " +
                "inputActions='False' " +
                $"expectedGameplayActionMap='{FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap}' " +
                "expectedGameplayActionMapFound='False' " +
                "playerControlBindingTarget='False' " +
                "unityPlayerInputBridgeTarget='False' " +
                "unityPlayerInputBridgePlayerInputReference='False' " +
                "unityPlayerInputBridgePlayerInputMatchesResolved='False' " +
                "unityPlayerInputBridgeExpectedSlotMatches='False' " +
                "unityPlayerInputActivationTarget='False' " +
                "unityPlayerInputActivationPlayerInputReference='False' " +
                "unityPlayerInputActivationPlayerInputMatchesResolved='False' " +
                "unityPlayerInputActivationExpectedSlotMatches='False' " +
                "unityPlayerInputActivationActionMapMatchesExpected='False' " +
                "anchorHost='<none>' " +
                "anchorHostCount='0' " +
                "matchingAnchorHosts='0' " +
                "trackingTarget='<none>' " +
                "lookAtTarget='<none>' " +
                "trackingTargetMatchesPlayer='False' " +
                "lookAtTargetMatchesPlayer='False' " +
                "typedTransformBinding='False' " +
                "targetIsPlayerRoot='False' " +
                "formalPlayerViewBindingTargets='False' " +
                "createdTestObject='False' createdPlayerInput='False' movement='False' actorSpawning='False' gameplayCommandExecution='False' " +
                $"failureReason='{FormatFailureReason(failureReason)}'.");
        }

        private static void LogFullProof(
            FirstGameResolvedPlayer resolvedPlayer,
            bool hasPlayerInput,
            bool hasInputActions,
            bool hasExpectedActionMap,
            bool hasControlTarget,
            bool hasBridgeTarget,
            PlayerInput bridgePlayerInputReference,
            bool bridgePlayerInputReferenceMatches,
            string bridgeExpectedSlotId,
            bool bridgeExpectedSlotMatches,
            bool hasActivationTarget,
            PlayerInput activationPlayerInputReference,
            bool activationPlayerInputReferenceMatches,
            string activationExpectedSlotId,
            bool activationExpectedSlotMatches,
            string activationActionMapName,
            bool activationActionMapMatchesExpected,
            FrameworkCameraAnchorHost anchorHost,
            int anchorHostCount,
            int matchingAnchorHosts,
            Transform trackingTarget,
            Transform lookAtTarget,
            string failureReason)
        {
            bool succeeded = string.Equals(failureReason, "None", System.StringComparison.Ordinal);
            bool trackingTargetMatchesPlayer = trackingTarget == resolvedPlayer.Transform;
            bool lookAtTargetMatchesPlayer = lookAtTarget == resolvedPlayer.Transform;
            bool typedTransformBinding = trackingTarget != null && lookAtTarget != null;
            bool targetIsPlayerRoot = trackingTargetMatchesPlayer && lookAtTargetMatchesPlayer;
            bool formalPlayerViewBindingTargets =
                HasComponentByTypeName(resolvedPlayer.GameObject, PlayerViewBindingTargetTypeName) ||
                HasComponentByTypeName(resolvedPlayer.GameObject, PlayerViewCameraTargetBindingTargetTypeName) ||
                HasComponentByTypeName(resolvedPlayer.GameObject, PlayerViewCameraActivationTargetTypeName);

            Debug.Log(
                "[F53C3_FIRSTGAME_FULL_PLAYER_BINDING_CHAIN] " +
                $"status='{(succeeded ? "Succeeded" : "Failed")}' " +
                $"playerObject='{resolvedPlayer.PlayerObjectName}' " +
                $"identitySource='{resolvedPlayer.IdentitySource}' " +
                $"resolutionSource='{resolvedPlayer.ResolutionSource}' " +
                $"resolvedByName='{resolvedPlayer.ResolvedByName}' " +
                $"actorId='{resolvedPlayer.ActorId}' " +
                $"playerSlotId='{resolvedPlayer.PlayerSlotId}' " +
                $"playerInput='{hasPlayerInput}' " +
                $"inputActions='{hasInputActions}' " +
                $"expectedGameplayActionMap='{FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap}' " +
                $"expectedGameplayActionMapFound='{hasExpectedActionMap}' " +
                $"playerControlBindingTarget='{hasControlTarget}' " +
                $"unityPlayerInputBridgeTarget='{hasBridgeTarget}' " +
                $"unityPlayerInputBridgePlayerInputReference='{FormatObject(bridgePlayerInputReference)}' " +
                $"unityPlayerInputBridgePlayerInputMatchesResolved='{bridgePlayerInputReferenceMatches}' " +
                $"unityPlayerInputBridgeExpectedSlot='{FormatText(bridgeExpectedSlotId)}' " +
                $"unityPlayerInputBridgeExpectedSlotMatches='{bridgeExpectedSlotMatches}' " +
                $"unityPlayerInputActivationTarget='{hasActivationTarget}' " +
                $"unityPlayerInputActivationPlayerInputReference='{FormatObject(activationPlayerInputReference)}' " +
                $"unityPlayerInputActivationPlayerInputMatchesResolved='{activationPlayerInputReferenceMatches}' " +
                $"unityPlayerInputActivationExpectedSlot='{FormatText(activationExpectedSlotId)}' " +
                $"unityPlayerInputActivationExpectedSlotMatches='{activationExpectedSlotMatches}' " +
                $"unityPlayerInputActivationActionMap='{FormatText(activationActionMapName)}' " +
                $"unityPlayerInputActivationActionMapMatchesExpected='{activationActionMapMatchesExpected}' " +
                $"anchorHost='{FormatObject(anchorHost)}' " +
                $"anchorHostCount='{anchorHostCount}' " +
                $"matchingAnchorHosts='{matchingAnchorHosts}' " +
                $"trackingTarget='{FormatObject(trackingTarget)}' " +
                $"lookAtTarget='{FormatObject(lookAtTarget)}' " +
                $"trackingTargetMatchesPlayer='{trackingTargetMatchesPlayer}' " +
                $"lookAtTargetMatchesPlayer='{lookAtTargetMatchesPlayer}' " +
                $"typedTransformBinding='{typedTransformBinding}' " +
                $"targetIsPlayerRoot='{targetIsPlayerRoot}' " +
                $"formalPlayerViewBindingTargets='{formalPlayerViewBindingTargets}' " +
                "createdTestObject='False' createdPlayerInput='False' movement='False' actorSpawning='False' gameplayCommandExecution='False' " +
                $"failureReason='{FormatFailureReason(failureReason)}'.");
        }

        private static string FormatObject(Object value)
        {
            return value != null ? value.name : "<none>";
        }

        private static string FormatText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "<none>" : value;
        }

        private static string FormatFailureReason(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }
    }
}
