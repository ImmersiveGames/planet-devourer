using System;
using System.Linq;
using Immersive.Framework.ActivityRestart;
using Immersive.Framework.ObjectReset;
using Immersive.Framework.PlayerAuthoring;
using Immersive.Framework.Reset.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace FirstGame.FrameworkIntegration.Editor._Project.Scripts.Editor.FrameworkIntegration
{
    public static class FirstGameG1BGameplaySceneBuilder
    {
        private const string MenuPath = "FIRSTGAME/Immersive Framework/Gameplay/G1B Rebuild Minimal Playable Loop";
        private const string ScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";
        private const string ObjectiveTypeName = "FirstGame.Gameplay.FirstGameMinimalLoopObjective";
        private const string ControlsTypeName = "FirstGame.Gameplay.FirstGameMinimalLoopControls";
        private const string ProofTypeName = "FirstGame.Gameplay.Diagnostics.FirstGameG1BLoopProof";

        private static readonly string[] ObsoleteObjectNames =
        {
            "TestProb",
            "Button_ResetRoom",
            "Button_RestartActivity",
            "P2G Diagnostics",
            "G1B_MinimalPlayableLoop"
        };

        private static readonly string[] ObsoleteComponentTypeNames =
        {
            "FirstGame.Player.FirstGameP2GMinimalControlMovementProof",
            "_Project.Scripts.FirstGamePlayerResetProbe"
        };

        [MenuItem(MenuPath)]
        public static void Rebuild()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            PlayerComposer playerComposer = FindSingleInScene<PlayerComposer>(scene, nameof(PlayerComposer));
            Transform player = playerComposer.transform;

            ObjectResetGroupTrigger resetTemplate = FindFirstInScene<ObjectResetGroupTrigger>(scene);
            ActivityRestartTrigger restartTemplate = FindFirstInScene<ActivityRestartTrigger>(scene);
            string resetTemplateJson = resetTemplate != null ? EditorJsonUtility.ToJson(resetTemplate) : string.Empty;
            string restartTemplateJson = restartTemplate != null ? EditorJsonUtility.ToJson(restartTemplate) : string.Empty;

            RemoveObsoleteObjects(scene);
            RemoveObsoleteComponents(scene);

            GameObject root = new GameObject("G1B_MinimalPlayableLoop");
            Undo.RegisterCreatedObjectUndo(root, "Create G1B gameplay root");

            Transform environment = CreateChild(root.transform, "Environment");
            BuildEnvironment(environment);

            player.position = new Vector3(0f, 0.75f, -7f);
            player.rotation = Quaternion.identity;

            Component objective = BuildObjective(root.transform, player);
            Component controls = BuildControls(root.transform, resetTemplateJson, restartTemplateJson);
            BuildDiagnostics(root.transform, objective, playerComposer);
            BuildInstructionBoard(root.transform);

            EditorUtility.SetDirty(playerComposer);
            EditorUtility.SetDirty(objective);
            EditorUtility.SetDirty(controls);
            EditorSceneManager.MarkSceneDirty(scene);

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new InvalidOperationException($"Could not save rebuilt gameplay scene at '{ScenePath}'.");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "[G1B_FIRSTGAME_GAMEPLAY_BUILDER] status='Succeeded' " +
                $"scene='{ScenePath}' player='{playerComposer.name}' " +
                "message='Enter through Menu -> Gameplay. Reach the goal, use Escape for Pause, R for Reset, and T for Activity Restart.'.");
        }

        private static void BuildEnvironment(Transform parent)
        {
            CreatePrimitive(parent, "Floor", PrimitiveType.Cube, new Vector3(0f, -0.5f, 0f), new Vector3(18f, 1f, 22f));
            CreatePrimitive(parent, "LeftWall", PrimitiveType.Cube, new Vector3(-9f, 1.5f, 0f), new Vector3(1f, 4f, 22f));
            CreatePrimitive(parent, "RightWall", PrimitiveType.Cube, new Vector3(9f, 1.5f, 0f), new Vector3(1f, 4f, 22f));
            CreatePrimitive(parent, "BackWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, -11f), new Vector3(18f, 4f, 1f));
            CreatePrimitive(parent, "GoalWall", PrimitiveType.Cube, new Vector3(0f, 1.5f, 11f), new Vector3(18f, 4f, 1f));

            CreatePrimitive(parent, "LeftObstacle", PrimitiveType.Cube, new Vector3(-3f, 0.75f, -1f), new Vector3(2f, 1.5f, 5f));
            CreatePrimitive(parent, "RightObstacle", PrimitiveType.Cube, new Vector3(3f, 0.75f, 3f), new Vector3(2f, 1.5f, 5f));
            CreatePrimitive(parent, "CenterMarker", PrimitiveType.Cylinder, new Vector3(0f, 0.25f, 1f), new Vector3(1.5f, 0.25f, 1.5f));
        }

        private static Component BuildObjective(Transform parent, Transform player)
        {
            Transform objectiveRoot = CreateChild(parent, "Gameplay/MinimalGoal");
            objectiveRoot.position = new Vector3(0f, 0f, 8f);

            GameObject triggerObject = new GameObject("GoalTrigger");
            triggerObject.transform.SetParent(objectiveRoot, false);
            BoxCollider trigger = triggerObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(5f, 3f, 3f);
            trigger.center = new Vector3(0f, 1.5f, 0f);

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "GoalCore";
            visual.transform.SetParent(objectiveRoot, false);
            visual.transform.localPosition = new Vector3(0f, 1f, 0f);
            visual.transform.localScale = new Vector3(2f, 2f, 2f);
            UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());

            Component objective = triggerObject.AddComponent(ResolveRuntimeType(ObjectiveTypeName));
            SetObject(objective, "expectedPlayer", player);
            SetObject(objective, "objectiveVisual", visual.transform);
            SetObject(objective, "objectiveRenderer", visual.GetComponent<Renderer>());

            UnityResetSubjectAdapter adapter = triggerObject.AddComponent<UnityResetSubjectAdapter>();
            SetBool(adapter, "registerOnEnable", true);
            SetBool(adapter, "unregisterOnDisable", true);
            SetBool(adapter, "retryUntilRuntimeAvailable", true);
            SetString(adapter, "subjectId", "firstgame.minimal-loop.goal");
            SetInt(adapter, "participantDiscovery", 20);
            SetBool(adapter, "includeInactiveParticipants", true);
            SetBool(adapter, "includeUnityResettableComponents", true);

            return objective;
        }

        private static Component BuildControls(
            Transform parent,
            string resetTemplateJson,
            string restartTemplateJson)
        {
            Transform controlsRoot = CreateChild(parent, "Gameplay/LoopControls");

            ObjectResetGroupTrigger reset = controlsRoot.gameObject.AddComponent<ObjectResetGroupTrigger>();
            if (!string.IsNullOrEmpty(resetTemplateJson))
            {
                EditorJsonUtility.FromJsonOverwrite(resetTemplateJson, reset);
            }
            else
            {
                SetString(reset, "groupId", "firstgame.reset.room");
                SetString(reset, "reason", "firstgame.minimal-loop.reset");
                SetNestedInt(reset, "selection.mode", 40);
            }

            ActivityRestartTrigger restart = controlsRoot.gameObject.AddComponent<ActivityRestartTrigger>();
            if (!string.IsNullOrEmpty(restartTemplateJson))
            {
                EditorJsonUtility.FromJsonOverwrite(restartTemplateJson, restart);
            }
            else
            {
                SetBool(restart, "useCurrentActivityWhenTargetMissing", true);
                SetBool(restart, "requireTargetActivityIsCurrent", true);
                SetString(restart, "reason", "firstgame.minimal-loop.restart");
            }

            Component controls = controlsRoot.gameObject.AddComponent(ResolveRuntimeType(ControlsTypeName));
            SetObject(controls, "resetTrigger", reset);
            SetObject(controls, "activityRestartTrigger", restart);
            return controls;
        }

        private static void BuildDiagnostics(
            Transform parent,
            Component objective,
            PlayerComposer playerComposer)
        {
            Transform diagnostics = CreateChild(parent, "Diagnostics");
            Component proof = diagnostics.gameObject.AddComponent(ResolveRuntimeType(ProofTypeName));
            SetObject(proof, "objective", objective);
            SetObject(proof, "player", playerComposer.transform);
            SetObject(proof, "cameraTrackingTarget", ResolveCameraTarget(playerComposer));
        }

        private static Transform ResolveCameraTarget(PlayerComposer composer)
        {
            SerializedObject serialized = new SerializedObject(composer);
            SerializedProperty target = serialized.FindProperty("cameraTarget");
            return target != null ? target.objectReferenceValue as Transform : composer.transform;
        }

        private static void BuildInstructionBoard(Transform parent)
        {
            GameObject board = CreatePrimitive(parent, "Instructions", PrimitiveType.Cube,
                new Vector3(0f, 1.5f, -9.5f), new Vector3(8f, 2.5f, 0.25f));
            board.transform.rotation = Quaternion.identity;
        }

        private static GameObject CreatePrimitive(
            Transform parent,
            string name,
            PrimitiveType type,
            Vector3 position,
            Vector3 scale)
        {
            GameObject value = GameObject.CreatePrimitive(type);
            value.name = name;
            value.transform.SetParent(parent, false);
            value.transform.position = position;
            value.transform.localScale = scale;
            Undo.RegisterCreatedObjectUndo(value, "Create " + name);
            return value;
        }

        private static Transform CreateChild(Transform parent, string path)
        {
            Transform current = parent;
            foreach (string segment in path.Split('/'))
            {
                Transform existing = current.Find(segment);
                if (existing != null)
                {
                    current = existing;
                    continue;
                }

                GameObject child = new GameObject(segment);
                child.transform.SetParent(current, false);
                Undo.RegisterCreatedObjectUndo(child, "Create " + segment);
                current = child.transform;
            }

            return current;
        }

        private static void RemoveObsoleteObjects(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true).Reverse())
                {
                    if (transform == null || !ObsoleteObjectNames.Contains(transform.name, StringComparer.Ordinal))
                    {
                        continue;
                    }

                    Undo.DestroyObjectImmediate(transform.gameObject);
                }
            }
        }

        private static void RemoveObsoleteComponents(Scene scene)
        {
            foreach (MonoBehaviour component in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {
                if (component == null || component.gameObject.scene != scene)
                {
                    continue;
                }

                string fullName = component.GetType().FullName;
                if (ObsoleteComponentTypeNames.Contains(fullName, StringComparer.Ordinal))
                {
                    Undo.DestroyObjectImmediate(component);
                }
            }
        }


        private static Type ResolveRuntimeType(string fullName)
        {
            Type type = TypeCache.GetTypesDerivedFrom<MonoBehaviour>()
                .SingleOrDefault(candidate =>
                    string.Equals(candidate.FullName, fullName, StringComparison.Ordinal));

            if (type == null)
            {
                throw new InvalidOperationException(
                    $"Runtime component type '{fullName}' was not found. Confirm the G1B runtime scripts compiled before running the builder.");
            }

            return type;
        }

        private static T FindSingleInScene<T>(Scene scene, string label) where T : Component
        {
            T[] matches = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include)
                .Where(value => value != null && value.gameObject.scene == scene)
                .ToArray();

            if (matches.Length != 1)
            {
                throw new InvalidOperationException($"Expected exactly one {label} in '{scene.name}', found '{matches.Length}'.");
            }

            return matches[0];
        }

        private static T FindFirstInScene<T>(Scene scene) where T : Component
        {
            return UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include)
                .FirstOrDefault(value => value != null && value.gameObject.scene == scene);
        }

        private static void SetObject(UnityEngine.Object target, string property, UnityEngine.Object value) =>
            SetProperty(target, property, item => item.objectReferenceValue = value);

        private static void SetString(UnityEngine.Object target, string property, string value) =>
            SetProperty(target, property, item => item.stringValue = value);

        private static void SetBool(UnityEngine.Object target, string property, bool value) =>
            SetProperty(target, property, item => item.boolValue = value);

        private static void SetInt(UnityEngine.Object target, string property, int value) =>
            SetProperty(target, property, item => item.intValue = value);

        private static void SetNestedInt(UnityEngine.Object target, string property, int value) =>
            SetProperty(target, property, item => item.intValue = value);

        private static void SetProperty(UnityEngine.Object target, string property, Action<SerializedProperty> assign)
        {
            SerializedObject serialized = new SerializedObject(target);
            SerializedProperty item = serialized.FindProperty(property);
            if (item == null)
            {
                Debug.LogWarning($"[G1B_FIRSTGAME_GAMEPLAY_BUILDER] property='{property}' target='{target.GetType().Name}' status='Skipped'.");
                return;
            }

            assign(item);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
