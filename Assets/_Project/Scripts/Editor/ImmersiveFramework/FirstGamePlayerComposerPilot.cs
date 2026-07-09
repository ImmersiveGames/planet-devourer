using Immersive.Framework.PlayerAuthoring;
using UnityEditor;
using UnityEngine;

namespace FirstGame.Editor.ImmersiveFramework
{
    /// <summary>
    /// FIRSTGAME-only pilot helper for adopting the official framework PlayerComposer.
    /// This helper configures the selected GameObject with product-level PlayerComposer intent.
    /// It does not materialize bindings directly and does not replace the official PlayerComposer Inspector Apply/Rebuild flow.
    /// </summary>
    internal static class FirstGamePlayerComposerPilot
    {
        private const string MenuRoot = "FIRSTGAME/Immersive Framework/Player Composer Pilot/";
        private const string DefaultActorId = "player.actor";
        private const string DefaultPlayerSlotId = "player.1";

        [MenuItem(MenuRoot + "Configure Selected Player Composer", false, 2200)]
        private static void ConfigureSelectedPlayerComposer()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[FIRSTGAME][PlayerComposerPilot] Select a Player GameObject before configuring the official PlayerComposer.");
                return;
            }

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Configure FIRSTGAME PlayerComposer Pilot");

            PlayerComposer composer = selected.GetComponent<PlayerComposer>();
            if (composer == null)
            {
                composer = Undo.AddComponent<PlayerComposer>(selected);
            }

            Component playerInput = FindPlayerInput(selected);
            Transform anchorsRoot = FindOrCreateDirectChild(selected.transform, "Anchors");
            Transform cameraTarget = FindOrCreateDirectChild(anchorsRoot, "CameraTarget");
            Transform lookAtTarget = FindOrCreateDirectChild(anchorsRoot, "LookAtTarget");
            string gameplayActionMap = ResolveGameplayActionMap(playerInput);

            SerializedObject serializedComposer = new SerializedObject(composer);
            SetString(serializedComposer, "actorId", DefaultActorId);
            SetString(serializedComposer, "playerSlotId", DefaultPlayerSlotId);
            SetObject(serializedComposer, "playerInput", playerInput);
            SetString(serializedComposer, "gameplayActionMap", gameplayActionMap);
            SetObject(serializedComposer, "cameraTarget", cameraTarget);
            SetObject(serializedComposer, "lookAtTarget", lookAtTarget);
            SetBool(serializedComposer, "resetEnabled", false);
            SetEnum(serializedComposer, "resetParticipantPolicy", (int)PlayerComposerResetParticipantPolicy.None);
            SetBool(serializedComposer, "createBindingsRootIfMissing", true);
            SetBool(serializedComposer, "createAnchorsIfMissing", true);
            SetBool(serializedComposer, "inputBindingRequired", true);
            SetBool(serializedComposer, "cameraBindingRequired", true);
            SetBool(serializedComposer, "materializeSlotOccupancy", true);
            SetBool(serializedComposer, "materializePassiveEntryViewControl", false);
            SetBool(serializedComposer, "logApplyRebuildDiagnostics", true);
            serializedComposer.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(composer);
            EditorUtility.SetDirty(selected);
            Selection.activeObject = composer;
            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log(
                $"[FIRSTGAME][PlayerComposerPilot] Configured official PlayerComposer intent. player='{selected.name}' actorId='{DefaultActorId}' playerSlotId='{DefaultPlayerSlotId}' actionMap='{gameplayActionMap}' hasPlayerInput='{(playerInput != null)}'. Use the PlayerComposer Inspector Apply/Rebuild button to materialize framework bindings.",
                selected);
        }

        [MenuItem(MenuRoot + "Configure Selected Player Composer", true)]
        private static bool CanConfigureSelectedPlayerComposer()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem(MenuRoot + "Validate Selected Player Composer", false, 2201)]
        private static void ValidateSelectedPlayerComposer()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[FIRSTGAME][PlayerComposerPilot] Select a Player GameObject before validating the official PlayerComposer.");
                return;
            }

            PlayerComposer composer = selected.GetComponent<PlayerComposer>();
            if (composer == null)
            {
                Debug.LogWarning($"[FIRSTGAME][PlayerComposerPilot] Selected GameObject has no official PlayerComposer. player='{selected.name}'.", selected);
                return;
            }

            if (!composer.TryValidateForApply(out string issue))
            {
                Debug.LogWarning($"[FIRSTGAME][PlayerComposerPilot] PlayerComposer validation failed. player='{selected.name}' issue='{issue}'.", selected);
                return;
            }

            PlayerComposerDebugSnapshot snapshot = composer.CreateDebugSnapshot();
            Debug.Log(
                $"[FIRSTGAME][PlayerComposerPilot] PlayerComposer validation succeeded. player='{selected.name}' actorId='{snapshot.ActorId}' playerSlotId='{snapshot.PlayerSlotId}' playerInput='{snapshot.PlayerInputName}' actionMap='{snapshot.GameplayActionMap}' actionMapFound='{snapshot.ActionMapFound}' resetEnabled='{snapshot.ResetEnabled}'.",
                selected);
        }

        [MenuItem(MenuRoot + "Validate Selected Player Composer", true)]
        private static bool CanValidateSelectedPlayerComposer()
        {
            return Selection.activeGameObject != null;
        }

        private static Transform FindOrCreateDirectChild(Transform parent, string childName)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == childName)
                {
                    return child;
                }
            }

            GameObject created = new GameObject(childName);
            Undo.RegisterCreatedObjectUndo(created, $"Create {childName}");
            created.transform.SetParent(parent, false);
            return created.transform;
        }

        private static Component FindPlayerInput(GameObject selected)
        {
            System.Type playerInputType = System.Type.GetType("UnityEngine.InputSystem.PlayerInput, Unity.InputSystem");
            return playerInputType != null ? selected.GetComponent(playerInputType) : null;
        }

        private static string ResolveGameplayActionMap(Component playerInput)
        {
            if (playerInput == null)
            {
                return "Player";
            }

            SerializedObject serializedPlayerInput = new SerializedObject(playerInput);
            SerializedProperty defaultActionMap = serializedPlayerInput.FindProperty("m_DefaultActionMap");
            if (defaultActionMap != null && !string.IsNullOrWhiteSpace(defaultActionMap.stringValue))
            {
                return defaultActionMap.stringValue;
            }

            // Avoid a direct Unity.InputSystem assembly reference from the FIRSTGAME editor assembly.
            // The official PlayerComposer validation still owns the real action-map consistency check.
            return "Player";
        }

        private static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value ?? string.Empty;
            }
        }

        private static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.boolValue = value;
            }
        }

        private static void SetEnum(SerializedObject serializedObject, string propertyName, int enumValueIndex)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.enumValueIndex = enumValueIndex;
            }
        }

        private static void SetObject(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
        }
    }
}
