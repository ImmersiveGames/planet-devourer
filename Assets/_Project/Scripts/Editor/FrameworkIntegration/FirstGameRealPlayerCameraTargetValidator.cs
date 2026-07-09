using Immersive.Framework.Camera;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkIntegration.Editor
{
    /// <summary>
    /// FIRSTGAME editor-only proof for the real player camera target binding.
    /// This validates the current route/activity camera path through FrameworkCameraAnchorHost.
    /// It does not create PlayerView contracts, move the player, route input, spawn actors, execute gameplay or save progression.
    /// </summary>
    public static class FirstGameRealPlayerCameraTargetValidator
    {
        private const string ValidateMenuPath = "FIRSTGAME/Immersive Framework/Validate Real Player Camera Target";
        private const string ExpectedSceneName = "FG_Gameplay";
        private const string TrackingTargetPropertyName = "trackingTarget";
        private const string LookAtTargetPropertyName = "lookAtTarget";

        private const string PlayerViewBindingTargetTypeName = "PlayerViewBindingTargetBehaviour";
        private const string PlayerViewCameraTargetBindingTargetTypeName = "PlayerViewCameraTargetBindingTargetBehaviour";
        private const string PlayerViewCameraActivationTargetTypeName = "PlayerViewCameraActivationTargetBehaviour";

        [MenuItem(ValidateMenuPath)]
        public static void ValidateRealPlayerCameraTarget()
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

            FrameworkCameraAnchorHost[] anchorHosts = FindSceneComponents<FrameworkCameraAnchorHost>(scene);
            if (anchorHosts.Length == 0)
            {
                LogProof(resolvedPlayer, null, anchorHosts.Length, 0, null, null, false, false, "MissingCameraAnchorHost");
                return;
            }

            FrameworkCameraAnchorHost matchingHost = null;
            Transform matchingTrackingTarget = null;
            Transform matchingLookAtTarget = null;
            int matchingHosts = 0;

            for (int i = 0; i < anchorHosts.Length; i++)
            {
                FrameworkCameraAnchorHost host = anchorHosts[i];
                if (host == null)
                {
                    continue;
                }

                Transform trackingTarget = ReadTransformReference(host, TrackingTargetPropertyName);
                Transform lookAtTarget = ReadTransformReference(host, LookAtTargetPropertyName);
                bool trackingMatches = trackingTarget == resolvedPlayer.Transform;
                bool lookAtMatches = lookAtTarget == resolvedPlayer.Transform;

                if (trackingMatches && lookAtMatches)
                {
                    matchingHost = host;
                    matchingTrackingTarget = trackingTarget;
                    matchingLookAtTarget = lookAtTarget;
                    matchingHosts++;
                }
            }

            if (matchingHosts == 0)
            {
                FrameworkCameraAnchorHost firstHost = anchorHosts[0];
                LogProof(
                    resolvedPlayer,
                    firstHost,
                    anchorHosts.Length,
                    matchingHosts,
                    firstHost != null ? ReadTransformReference(firstHost, TrackingTargetPropertyName) : null,
                    firstHost != null ? ReadTransformReference(firstHost, LookAtTargetPropertyName) : null,
                    false,
                    false,
                    "CameraAnchorHostTargetMismatch");
                return;
            }

            if (matchingHosts > 1)
            {
                LogProof(
                    resolvedPlayer,
                    matchingHost,
                    anchorHosts.Length,
                    matchingHosts,
                    matchingTrackingTarget,
                    matchingLookAtTarget,
                    true,
                    true,
                    "MultipleMatchingCameraAnchorHosts");
                return;
            }

            LogProof(
                resolvedPlayer,
                matchingHost,
                anchorHosts.Length,
                matchingHosts,
                matchingTrackingTarget,
                matchingLookAtTarget,
                true,
                true,
                "None");
        }

        private static Transform ReadTransformReference(Object target, string propertyName)
        {
            if (target == null)
            {
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            return property != null ? property.objectReferenceValue as Transform : null;
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
                "[F53C2_FIRSTGAME_REAL_PLAYER_CAMERA_TARGET] " +
                "status='Failed' " +
                "playerObject='<none>' " +
                "identitySource='<none>' " +
                "resolutionSource='<none>' " +
                "resolvedByName='False' " +
                "actorId='<none>' " +
                "playerSlotId='<none>' " +
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

        private static void LogProof(
            FirstGameResolvedPlayer resolvedPlayer,
            FrameworkCameraAnchorHost anchorHost,
            int anchorHostCount,
            int matchingAnchorHosts,
            Transform trackingTarget,
            Transform lookAtTarget,
            bool trackingTargetMatchesPlayer,
            bool lookAtTargetMatchesPlayer,
            string failureReason)
        {
            bool succeeded = string.Equals(failureReason, "None", System.StringComparison.Ordinal);
            bool typedTransformBinding = trackingTarget != null && lookAtTarget != null;
            bool targetIsPlayerRoot = trackingTarget == resolvedPlayer.Transform && lookAtTarget == resolvedPlayer.Transform;
            bool formalPlayerViewBindingTargets =
                HasComponentByTypeName(resolvedPlayer.GameObject, PlayerViewBindingTargetTypeName) ||
                HasComponentByTypeName(resolvedPlayer.GameObject, PlayerViewCameraTargetBindingTargetTypeName) ||
                HasComponentByTypeName(resolvedPlayer.GameObject, PlayerViewCameraActivationTargetTypeName);

            Debug.Log(
                "[F53C2_FIRSTGAME_REAL_PLAYER_CAMERA_TARGET] " +
                $"status='{(succeeded ? "Succeeded" : "Failed")}' " +
                $"playerObject='{resolvedPlayer.PlayerObjectName}' " +
                $"identitySource='{resolvedPlayer.IdentitySource}' " +
                $"resolutionSource='{resolvedPlayer.ResolutionSource}' " +
                $"resolvedByName='{resolvedPlayer.ResolvedByName}' " +
                $"actorId='{resolvedPlayer.ActorId}' " +
                $"playerSlotId='{resolvedPlayer.PlayerSlotId}' " +
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

        private static string FormatFailureReason(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }
    }
}
