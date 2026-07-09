using Immersive.Framework.PlayerBinding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.FrameworkIntegration.Editor
{
    /// <summary>
    /// FIRSTGAME editor-only validation for real PlayerPrototype wiring against the accepted F52 PlayerControl/PlayerInput chain.
    /// This tool does not create test objects, read InputActions, enable movement, spawn actors, execute gameplay or save scenes.
    /// </summary>
    public static class FirstGameRealPlayerBindingValidator
    {
        private const string ValidateMenuPath = "FIRSTGAME/Immersive Framework/Validate Real Player Binding";
        private const string EnsureSelectedMenuPath = "FIRSTGAME/Immersive Framework/Ensure Selected Player Binding Components";
        private const string CleanupPreflightMenuPath = "FIRSTGAME/Immersive Framework/Cleanup F53A Preflight Proof Assets";

        private const string CanonicalPlayerObjectName = "PlayerPrototype";
        private const string CanonicalPlayerSlotId = "player.1";
        private const string CanonicalGameplayActionMap = "Player";

        private static readonly string[] F53APreflightAssetPaths =
        {
            "Assets/_Project/Scripts/FrameworkProof",
            "Assets/_Project/Scripts/Editor/FrameworkProof",
            "Assets/_Project/Documentation/F53A-PlayerBinding-Usability-Proof.md"
        };

        [MenuItem(ValidateMenuPath)]
        public static void ValidateRealPlayerBinding()
        {
            PlayerInput playerInput = FindRealPlayerInput();
            ValidationSnapshot snapshot = BuildSnapshot(playerInput);
            LogValidation(snapshot, null);
        }

        [MenuItem(EnsureSelectedMenuPath)]
        public static void EnsureSelectedPlayerBindingComponents()
        {
            GameObject selected = Selection.activeGameObject;
            PlayerInput playerInput = FindPlayerInputFromSelection(selected);

            if (playerInput == null)
            {
                Debug.LogError(
                    "[F53B_FIRSTGAME_REAL_PLAYER_BINDING] status='Failed' " +
                    "failureReason='MissingSelectedPlayerInput' " +
                    "message='Select the real FIRSTGAME PlayerPrototype or a child/parent containing PlayerInput before ensuring binding components.'.");
                return;
            }

            GameObject target = playerInput.gameObject;
            Undo.SetCurrentGroupName("Ensure FIRSTGAME Player Binding Components");
            int undoGroup = Undo.GetCurrentGroup();

            PlayerControlBindingTargetBehaviour controlTarget = EnsureComponent<PlayerControlBindingTargetBehaviour>(target);
            UnityPlayerInputBridgeTargetBehaviour bridgeTarget = EnsureComponent<UnityPlayerInputBridgeTargetBehaviour>(target);
            UnityPlayerInputActivationTargetBehaviour activationTarget = EnsureComponent<UnityPlayerInputActivationTargetBehaviour>(target);

            ConfigurePlayerControlTarget(controlTarget, target.name);
            ConfigureBridgeTarget(bridgeTarget, target.name, playerInput);
            ConfigureActivationTarget(activationTarget, target.name, playerInput);

            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(target.scene);
            Undo.CollapseUndoOperations(undoGroup);

            ValidationSnapshot snapshot = BuildSnapshot(playerInput);
            LogValidation(snapshot, "ensure-selected");
        }

        [MenuItem(CleanupPreflightMenuPath)]
        public static void CleanupF53APreflightProofAssets()
        {
            int removed = 0;
            for (int i = 0; i < F53APreflightAssetPaths.Length; i++)
            {
                string path = F53APreflightAssetPaths[i];
                if (!AssetDatabase.IsValidFolder(path) && AssetDatabase.LoadAssetAtPath<Object>(path) == null)
                {
                    continue;
                }

                if (AssetDatabase.DeleteAsset(path))
                {
                    removed++;
                }
                else
                {
                    Debug.LogWarning($"[F53B_FIRSTGAME_PREFLIGHT_CLEANUP] status='Partial' failedPath='{path}'.");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log(
                "[F53B_FIRSTGAME_PREFLIGHT_CLEANUP] " +
                $"status='Succeeded' removed='{removed}' " +
                "paths='Assets/_Project/Scripts/FrameworkProof; Assets/_Project/Scripts/Editor/FrameworkProof; Assets/_Project/Documentation/F53A-PlayerBinding-Usability-Proof.md'.");
        }

        private static PlayerInput FindRealPlayerInput()
        {
            PlayerInput selectedInput = FindPlayerInputFromSelection(Selection.activeGameObject);
            if (selectedInput != null)
            {
                return selectedInput;
            }

            PlayerInput[] inputs = Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
            PlayerInput fallback = null;

            for (int i = 0; i < inputs.Length; i++)
            {
                PlayerInput input = inputs[i];
                if (input == null)
                {
                    continue;
                }

                if (input.name == CanonicalPlayerObjectName)
                {
                    return input;
                }

                if (fallback == null && HasActionMap(input, CanonicalGameplayActionMap))
                {
                    fallback = input;
                }

                if (fallback == null)
                {
                    fallback = input;
                }
            }

            return fallback;
        }

        private static PlayerInput FindPlayerInputFromSelection(GameObject selected)
        {
            if (selected == null)
            {
                return null;
            }

            PlayerInput input = selected.GetComponent<PlayerInput>();
            if (input != null)
            {
                return input;
            }

            input = selected.GetComponentInParent<PlayerInput>();
            if (input != null)
            {
                return input;
            }

            return selected.GetComponentInChildren<PlayerInput>();
        }

        private static T EnsureComponent<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return Undo.AddComponent<T>(target);
        }

        private static ValidationSnapshot BuildSnapshot(PlayerInput playerInput)
        {
            GameObject target = playerInput != null ? playerInput.gameObject : null;
            bool hasPlayerInput = playerInput != null;
            bool hasInputActions = playerInput != null && playerInput.actions != null;
            bool hasActionMap = HasActionMap(playerInput, CanonicalGameplayActionMap);
            bool hasControlTarget = target != null && target.GetComponent<PlayerControlBindingTargetBehaviour>() != null;
            bool hasBridgeTarget = target != null && target.GetComponent<UnityPlayerInputBridgeTargetBehaviour>() != null;
            bool hasActivationTarget = target != null && target.GetComponent<UnityPlayerInputActivationTargetBehaviour>() != null;
            bool canonicalObject = target != null && target.name == CanonicalPlayerObjectName;
            bool succeeded = hasPlayerInput && hasInputActions && hasActionMap && hasControlTarget && hasBridgeTarget && hasActivationTarget;

            return new ValidationSnapshot(
                succeeded,
                target != null ? target.name : "<none>",
                hasPlayerInput,
                hasInputActions,
                hasActionMap,
                hasControlTarget,
                hasBridgeTarget,
                hasActivationTarget,
                canonicalObject,
                BuildFailureReason(hasPlayerInput, hasInputActions, hasActionMap, hasControlTarget, hasBridgeTarget, hasActivationTarget));
        }

        private static bool HasActionMap(PlayerInput playerInput, string actionMapName)
        {
            return playerInput != null &&
                   playerInput.actions != null &&
                   playerInput.actions.FindActionMap(actionMapName, false) != null;
        }

        private static string BuildFailureReason(
            bool hasPlayerInput,
            bool hasInputActions,
            bool hasActionMap,
            bool hasControlTarget,
            bool hasBridgeTarget,
            bool hasActivationTarget)
        {
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

        private static void ConfigurePlayerControlTarget(PlayerControlBindingTargetBehaviour component, string playerObjectName)
        {
            SerializedObject serialized = new SerializedObject(component);
            SetStringIfExists(serialized, "bindingTargetName", $"FIRSTGAME {playerObjectName} PlayerControl Binding Target");
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureBridgeTarget(UnityPlayerInputBridgeTargetBehaviour component, string playerObjectName, PlayerInput playerInput)
        {
            SerializedObject serialized = new SerializedObject(component);
            SetStringIfExists(serialized, "bridgeTargetName", $"FIRSTGAME {playerObjectName} Unity PlayerInput Bridge Target");
            SetStringIfExists(serialized, "expectedPlayerSlotId", CanonicalPlayerSlotId);
            SetObjectIfExists(serialized, "playerInput", playerInput);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureActivationTarget(UnityPlayerInputActivationTargetBehaviour component, string playerObjectName, PlayerInput playerInput)
        {
            SerializedObject serialized = new SerializedObject(component);
            SetStringIfExists(serialized, "activationTargetName", $"FIRSTGAME {playerObjectName} Unity PlayerInput Activation Target");
            SetStringIfExists(serialized, "expectedPlayerSlotId", CanonicalPlayerSlotId);
            SetObjectIfExists(serialized, "playerInput", playerInput);
            SetStringIfExists(serialized, "actionMapName", CanonicalGameplayActionMap);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetStringIfExists(SerializedObject serialized, string propertyName, string value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value;
            }
        }

        private static void SetObjectIfExists(SerializedObject serialized, string propertyName, Object value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
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
                $"playerInput='{snapshot.HasPlayerInput}' " +
                $"inputActions='{snapshot.HasInputActions}' " +
                $"expectedGameplayActionMap='Player' " +
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

            public string FailureReason { get; }
        }
    }
}
