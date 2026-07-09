using Immersive.Framework.PlayerBinding;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.FrameworkProof.Editor
{
    /// <summary>
    /// FIRSTGAME-local editor helper that creates a temporary proof GameObject in the open scene.
    /// It does not save the scene, modify project settings, create framework contracts, create an extra PlayerInput, or bind gameplay lifecycle.
    /// </summary>
    public static class FirstGamePlayerBindingUsabilityProbeCreator
    {
        private const string MenuPath = "FIRSTGAME/Immersive Framework/Create Player Binding Usability Probe";
        private const string PreferredGameplayActionMap = "Player";
        private const string SecondaryGameplayActionMap = "Gameplay";

        [MenuItem(MenuPath)]
        public static void CreateProbeInOpenScene()
        {
            GameObject root = new GameObject("F53A FIRSTGAME Player Binding Usability Probe");
            Undo.RegisterCreatedObjectUndo(root, "Create FIRSTGAME Player Binding Usability Probe");

            root.AddComponent<PlayerControlBindingTargetBehaviour>();
            root.AddComponent<UnityPlayerInputBridgeTargetBehaviour>();
            root.AddComponent<UnityPlayerInputActivationTargetBehaviour>();

            FirstGamePlayerBindingUsabilityProbe probe = root.AddComponent<FirstGamePlayerBindingUsabilityProbe>();
            PlayerInput existingPlayerInput = FindReusablePlayerInput(root);
            string actionMapName = ResolveExpectedActionMap(existingPlayerInput);
            probe.ConfigureExternalPlayerInput(existingPlayerInput, actionMapName);

            Selection.activeGameObject = root;

            Debug.Log(
                "[F53A_FIRSTGAME_PLAYER_BINDING_PROBE_CREATOR] " +
                $"status='{(existingPlayerInput != null ? "Succeeded" : "CreatedWithMissingPlayerInput")}' " +
                "created='F53A FIRSTGAME Player Binding Usability Probe' " +
                "createdPlayerInput='False' " +
                $"reusedPlayerInput='{(existingPlayerInput != null ? existingPlayerInput.name : "<none>")}' " +
                $"expectedGameplayActionMap='{(string.IsNullOrWhiteSpace(actionMapName) ? "<none>" : actionMapName)}' " +
                "sceneSaved='False' movement='False' actorSpawning='False' gameplayCommandExecution='False'.",
                root);
        }

        private static PlayerInput FindReusablePlayerInput(GameObject probeRoot)
        {
            PlayerInput[] candidates = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

            PlayerInput fallback = null;
            PlayerInput currentMapFallback = null;

            for (int i = 0; i < candidates.Length; i++)
            {
                PlayerInput candidate = candidates[i];
                if (candidate == null || candidate.gameObject == probeRoot)
                {
                    continue;
                }

                if (candidate.actions != null && candidate.actions.FindActionMap(PreferredGameplayActionMap, false) != null)
                {
                    return candidate;
                }

                if (candidate.actions != null && candidate.actions.FindActionMap(SecondaryGameplayActionMap, false) != null)
                {
                    fallback = candidate;
                }

                if (candidate.currentActionMap != null && currentMapFallback == null)
                {
                    currentMapFallback = candidate;
                }

                fallback ??= candidate;
            }

            return currentMapFallback != null ? currentMapFallback : fallback;
        }

        private static string ResolveExpectedActionMap(PlayerInput input)
        {
            if (input == null || input.actions == null)
            {
                return PreferredGameplayActionMap;
            }

            if (input.actions.FindActionMap(PreferredGameplayActionMap, false) != null)
            {
                return PreferredGameplayActionMap;
            }

            if (input.actions.FindActionMap(SecondaryGameplayActionMap, false) != null)
            {
                return SecondaryGameplayActionMap;
            }

            return input.currentActionMap != null ? input.currentActionMap.name : PreferredGameplayActionMap;
        }
    }
}
