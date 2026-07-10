using Immersive.Framework.PlayerBinding;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace FirstGame.FrameworkIntegration.Editor._Project.Scripts.Editor.FrameworkIntegration
{
    /// <summary>
    /// FIRSTGAME editor-only validation for real player wiring against the accepted F52 PlayerControl/PlayerInput chain.
    /// This tool resolves player identity from canonical declarations, not from GameObject.name.
    /// It does not create test objects, read InputActions, enable movement, spawn actors, execute gameplay or save scenes.
    /// </summary>
    public static class FirstGameRealPlayerBindingValidator
    {
        private const string ValidateMenuPath = "FIRSTGAME/Immersive Framework/Validate Real Player Binding";

        [MenuItem(ValidateMenuPath)]
        public static void ValidateRealPlayerBinding()
        {
            ValidationSnapshot snapshot = BuildSnapshotFromCanonicalResolver(Selection.activeGameObject);
            LogValidation(snapshot, null);
        }

        private static ValidationSnapshot BuildSnapshotFromCanonicalResolver(GameObject selectedObject)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!FirstGamePlayerIdentityResolver.TryResolveCanonicalPlayer(
                    scene,
                    selectedObject,
                    out FirstGameResolvedPlayer resolvedPlayer,
                    out string failureReason))
            {
                return ValidationSnapshot.Failed(failureReason);
            }

            return BuildSnapshot(resolvedPlayer, "None");
        }

        private static ValidationSnapshot BuildSnapshot(FirstGameResolvedPlayer resolvedPlayer, string resolverFailureReason)
        {
            GameObject target = resolvedPlayer.GameObject;
            PlayerInput playerInput = resolvedPlayer.PlayerInput;
            bool hasPlayerInput = playerInput != null;
            bool hasInputActions = playerInput != null && playerInput.actions != null;
            bool hasActionMap = FirstGamePlayerIdentityResolver.HasExpectedGameplayActionMap(playerInput);
            bool hasControlTarget = target != null && target.GetComponent<PlayerControlBindingTargetBehaviour>() != null;
            bool hasBridgeTarget = target != null && target.GetComponent<UnityPlayerInputBridgeTargetBehaviour>() != null;
            bool hasActivationTarget = target != null && target.GetComponent<UnityPlayerInputActivationTargetBehaviour>() != null;
            bool canonicalPlayerIdentity = resolvedPlayer.HasExpectedIdentity;
            bool succeeded = canonicalPlayerIdentity && hasPlayerInput && hasInputActions && hasActionMap && hasControlTarget && hasBridgeTarget && hasActivationTarget;

            return new ValidationSnapshot(
                succeeded,
                target != null ? target.name : "<none>",
                hasPlayerInput,
                hasInputActions,
                hasActionMap,
                hasControlTarget,
                hasBridgeTarget,
                hasActivationTarget,
                canonicalPlayerIdentity,
                resolvedPlayer.IdentitySource,
                resolvedPlayer.ResolutionSource,
                resolvedPlayer.ResolvedByName,
                resolvedPlayer.ActorId,
                resolvedPlayer.PlayerSlotId,
                BuildFailureReason(
                    resolverFailureReason,
                    canonicalPlayerIdentity,
                    hasPlayerInput,
                    hasInputActions,
                    hasActionMap,
                    hasControlTarget,
                    hasBridgeTarget,
                    hasActivationTarget));
        }

        private static string BuildFailureReason(
            string resolverFailureReason,
            bool canonicalPlayerIdentity,
            bool hasPlayerInput,
            bool hasInputActions,
            bool hasActionMap,
            bool hasControlTarget,
            bool hasBridgeTarget,
            bool hasActivationTarget)
        {
            if (!string.IsNullOrWhiteSpace(resolverFailureReason) && resolverFailureReason != "None")
            {
                return resolverFailureReason;
            }

            if (!canonicalPlayerIdentity)
            {
                return "MissingCanonicalPlayerIdentity";
            }

            if (!hasPlayerInput)
            {
                return "MissingPlayerInput";
            }

            if (!hasInputActions)
            {
                return "MissingInputActions";
            }

            if (!hasActionMap)
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

            if (!hasActivationTarget)
            {
                return "MissingUnityPlayerInputActivationTarget";
            }

            return "None";
        }

        private static void LogValidation(ValidationSnapshot snapshot, string mode)
        {
            string status = snapshot.Succeeded ? "Succeeded" : "Failed";
            string modeText = string.IsNullOrWhiteSpace(mode) ? "validate" : mode;
            Debug.Log(
                "[F53B_FIRSTGAME_REAL_PLAYER_BINDING] " +
                $"status='{status}' " +
                $"mode='{modeText}' " +
                $"playerObject='{snapshot.PlayerObjectName}' " +
                $"canonicalPlayerObject='{snapshot.CanonicalPlayerObject}' " +
                $"identitySource='{snapshot.IdentitySource}' " +
                $"resolutionSource='{snapshot.ResolutionSource}' " +
                $"resolvedByName='{snapshot.ResolvedByName}' " +
                $"actorId='{snapshot.ActorId}' " +
                $"playerSlotId='{snapshot.PlayerSlotId}' " +
                $"playerInput='{snapshot.HasPlayerInput}' " +
                $"inputActions='{snapshot.HasInputActions}' " +
                $"expectedGameplayActionMap='{FirstGamePlayerIdentityResolver.ExpectedGameplayActionMap}' " +
                $"expectedGameplayActionMapFound='{snapshot.HasExpectedActionMap}' " +
                $"playerControlBindingTarget='{snapshot.HasPlayerControlBindingTarget}' " +
                $"unityPlayerInputBridgeTarget='{snapshot.HasUnityPlayerInputBridgeTarget}' " +
                $"unityPlayerInputActivationTarget='{snapshot.HasUnityPlayerInputActivationTarget}' " +
                "createdTestObject='False' createdPlayerInput='False' movement='False' actorSpawning='False' gameplayCommandExecution='False' " +
                $"failureReason='{snapshot.FailureReason}'.");
        }

        private readonly struct ValidationSnapshot
        {
            public ValidationSnapshot(
                bool succeeded,
                string playerObjectName,
                bool hasPlayerInput,
                bool hasInputActions,
                bool hasExpectedActionMap,
                bool hasPlayerControlBindingTarget,
                bool hasUnityPlayerInputBridgeTarget,
                bool hasUnityPlayerInputActivationTarget,
                bool canonicalPlayerObject,
                string identitySource,
                string resolutionSource,
                bool resolvedByName,
                string actorId,
                string playerSlotId,
                string failureReason)
            {
                Succeeded = succeeded;
                PlayerObjectName = playerObjectName;
                HasPlayerInput = hasPlayerInput;
                HasInputActions = hasInputActions;
                HasExpectedActionMap = hasExpectedActionMap;
                HasPlayerControlBindingTarget = hasPlayerControlBindingTarget;
                HasUnityPlayerInputBridgeTarget = hasUnityPlayerInputBridgeTarget;
                HasUnityPlayerInputActivationTarget = hasUnityPlayerInputActivationTarget;
                CanonicalPlayerObject = canonicalPlayerObject;
                IdentitySource = string.IsNullOrWhiteSpace(identitySource) ? "<none>" : identitySource;
                ResolutionSource = string.IsNullOrWhiteSpace(resolutionSource) ? "<none>" : resolutionSource;
                ResolvedByName = resolvedByName;
                ActorId = string.IsNullOrWhiteSpace(actorId) ? "<none>" : actorId;
                PlayerSlotId = string.IsNullOrWhiteSpace(playerSlotId) ? "<none>" : playerSlotId;
                FailureReason = failureReason;
            }

            public bool Succeeded { get; }

            public string PlayerObjectName { get; }

            public bool HasPlayerInput { get; }

            public bool HasInputActions { get; }

            public bool HasExpectedActionMap { get; }

            public bool HasPlayerControlBindingTarget { get; }

            public bool HasUnityPlayerInputBridgeTarget { get; }

            public bool HasUnityPlayerInputActivationTarget { get; }

            public bool CanonicalPlayerObject { get; }

            public string IdentitySource { get; }

            public string ResolutionSource { get; }

            public bool ResolvedByName { get; }

            public string ActorId { get; }

            public string PlayerSlotId { get; }

            public string FailureReason { get; }

            public static ValidationSnapshot Failed(string failureReason)
            {
                return new ValidationSnapshot(
                    false,
                    "<none>",
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    "<none>",
                    "<none>",
                    false,
                    "<none>",
                    "<none>",
                    string.IsNullOrWhiteSpace(failureReason) ? "Unknown" : failureReason);
            }
        }
    }
}
