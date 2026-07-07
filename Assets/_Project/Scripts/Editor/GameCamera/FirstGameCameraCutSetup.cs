using Immersive.Framework.ActivityFlow;
using Immersive.Framework.Authoring;
using Immersive.Framework.Camera;
using Immersive.Framework.Camera.Cinemachine;
using Immersive.Framework.RouteLifecycle;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Editor.GameCamera
{
    /// <summary>
    /// Editor-only, idempotent FIRSTGAME camera setup for the first Cinemachine Route/Activity cut.
    /// It configures scene-authored bindings without using runtime global searches.
    /// </summary>
    public static class FirstGameCameraCutSetup
    {
        private const string MenuScenePath = "Assets/_Project/Scenes/Menu/FG_Menu.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";

        private const string MenuRoutePath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Routes/FG_MenuRoute.asset";
        private const string GameplayRoutePath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Routes/FG_GameplayRoute.asset";

        private const string ActivityAPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_A.asset";
        private const string ActivityBPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_B.asset";
        private const string ActivityCPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_C_RouteFallback.asset";
        private const string ActivityDPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_D_StopBgm.asset";

        private static bool hasBlockingIssue;

        [MenuItem("Tools/FIRSTGAME/Camera/Configure Route-Activity Camera")]
        public static void ConfigureCameraCut()
        {
            hasBlockingIssue = false;

            ConfigureMenuScene();
            ConfigureGameplayScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (hasBlockingIssue)
            {
                Debug.LogError("[FIRSTGAME_CAMERA_SETUP] Route/Activity camera setup completed with blocking issues. Fix the logged errors before Play.");
                return;
            }

            Debug.Log("[FIRSTGAME_CAMERA_SETUP] Route/Activity camera setup configured.");
        }

        private static void ConfigureMenuScene()
        {
            Scene scene = EditorSceneManager.OpenScene(MenuScenePath, OpenSceneMode.Single);
            RouteAsset menuRoute = LoadRequiredAsset<RouteAsset>(MenuRoutePath);

            if (menuRoute == null)
            {
                return;
            }

            GameObject mainCamera = EnsureMainCamera(scene, new Vector3(0f, 1f, -10f), Quaternion.identity);
            EnsureComponent<UnityEngine.Camera>(mainCamera);
            EnsureComponent<CinemachineBrain>(mainCamera);
            RemoveForbiddenCameraComponents(mainCamera, removeCamera: false, removeBrain: false);

            GameObject root = EnsureRoot(scene, "FirstGameCameraRoot");
            RouteContentBinding routeContentBinding = EnsureComponent<RouteContentBinding>(root);
            FrameworkCameraDirector director = EnsureComponent<FrameworkCameraDirector>(root);
            FrameworkCinemachineRigApplier rigApplier = EnsureComponent<FrameworkCinemachineRigApplier>(root);
            FrameworkRouteCameraBinding routeCameraBinding = EnsureComponent<FrameworkRouteCameraBinding>(root);

            GameObject menuRig = EnsureCinemachineRig(scene, "MenuRoute_CameraRig", new Vector3(0f, 1f, -10f), Quaternion.identity);

            SetSerialized(routeContentBinding, "route", menuRoute);
            SetSerialized(routeContentBinding, "localContentId", "firstgame.menu.route.camera");
            SetSerialized(routeContentBinding, "requiredness", 10);

            SetSerialized(director, "defaultCameraRig", menuRig);
            SetSerialized(director, "defaultAnchors", (Object)null);
            SetSerialized(director, "routePriority", 20);
            SetSerialized(director, "activityPriority", 100);
            SetSerialized(director, "setRigActiveState", true);
            SetSerialized(director, "rigApplier", rigApplier);
            SetSerialized(director, "logTransitions", true);

            SetSerialized(routeCameraBinding, "routeCameraRig", menuRig);
            SetSerialized(routeCameraBinding, "routeAnchors", (Object)null);
            SetSerialized(routeCameraBinding, "director", director);
            SetSerialized(routeCameraBinding, "startupActivityCameraBinding", (Object)null);

            menuRig.SetActive(false);

            ValidateObjectReference(routeContentBinding, "route", menuRoute, "RouteContentBinding route on Menu camera root");
            ValidateObjectReference(routeCameraBinding, "routeCameraRig", menuRig, "FrameworkRouteCameraBinding routeCameraRig on Menu camera root");
            ValidateObjectReference(routeCameraBinding, "director", director, "FrameworkRouteCameraBinding director on Menu camera root");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureGameplayScene()
        {
            Scene scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
            RouteAsset gameplayRoute = LoadRequiredAsset<RouteAsset>(GameplayRoutePath);
            ActivityAsset activityA = LoadRequiredAsset<ActivityAsset>(ActivityAPath);
            ActivityAsset activityB = LoadRequiredAsset<ActivityAsset>(ActivityBPath);
            ActivityAsset activityC = LoadRequiredAsset<ActivityAsset>(ActivityCPath);
            ActivityAsset activityD = LoadRequiredAsset<ActivityAsset>(ActivityDPath);

            if (gameplayRoute == null || activityA == null || activityB == null || activityC == null || activityD == null)
            {
                return;
            }

            GameObject mainCamera = EnsureMainCamera(scene, new Vector3(0f, 4f, -10f), Quaternion.Euler(20f, 0f, 0f));
            EnsureComponent<UnityEngine.Camera>(mainCamera);
            EnsureComponent<CinemachineBrain>(mainCamera);
            RemoveForbiddenCameraComponents(mainCamera, removeCamera: false, removeBrain: false);

            GameObject root = EnsureRoot(scene, "FirstGameCameraRoot");
            RouteContentBinding routeContentBinding = EnsureComponent<RouteContentBinding>(root);
            FrameworkCameraDirector director = EnsureComponent<FrameworkCameraDirector>(root);
            FrameworkCinemachineRigApplier rigApplier = EnsureComponent<FrameworkCinemachineRigApplier>(root);
            FrameworkRouteCameraBinding routeCameraBinding = EnsureComponent<FrameworkRouteCameraBinding>(root);

            GameObject player = FindInScene(scene, "PlayerPrototype");
            if (player == null)
            {
                Debug.LogWarning("[FIRSTGAME_CAMERA_SETUP] PlayerPrototype was not found. Camera rigs will be positioned but not assigned tracking targets.");
            }

            GameObject anchorsObject = EnsureRoot(scene, "FirstGameCameraAnchors");
            FrameworkCameraAnchorHost anchors = EnsureComponent<FrameworkCameraAnchorHost>(anchorsObject);
            Transform target = player != null ? player.transform : null;
            SetSerialized(anchors, "trackingTarget", target);
            SetSerialized(anchors, "lookAtTarget", target);

            GameObject routeRig = EnsureCinemachineRig(scene, "GameplayRoute_CameraRig", new Vector3(0f, 5f, -10f), Quaternion.Euler(25f, 0f, 0f));
            GameObject activityARig = EnsureCinemachineRig(scene, "ActivityA_CameraRig", new Vector3(0f, 3.5f, -7f), Quaternion.Euler(18f, 0f, 0f));

            SetSerialized(routeContentBinding, "route", gameplayRoute);
            SetSerialized(routeContentBinding, "localContentId", "firstgame.gameplay.route.camera");
            SetSerialized(routeContentBinding, "requiredness", 10);

            SetSerialized(director, "defaultCameraRig", routeRig);
            SetSerialized(director, "defaultAnchors", anchors);
            SetSerialized(director, "routePriority", 20);
            SetSerialized(director, "activityPriority", 100);
            SetSerialized(director, "setRigActiveState", true);
            SetSerialized(director, "rigApplier", rigApplier);
            SetSerialized(director, "logTransitions", true);

            FrameworkActivityCameraBinding activityABinding = EnsureActivityBinding(scene, "ActivityA_ContentRoot", activityA, activityARig, FrameworkCameraActivityPolicy.UseOwnOrRetainActivityUntilRouteExit, anchors, director);
            EnsureActivityBinding(scene, "ActivityB_ContentRoot", activityB, null, FrameworkCameraActivityPolicy.UseOwnOrRetainActivityUntilRouteExit, anchors, director);
            EnsureActivityBinding(scene, "ActivityC_RouteFallback_ContentRoot", activityC, null, FrameworkCameraActivityPolicy.UseRoute, anchors, director);
            EnsureActivityBinding(scene, "ActivityD_StopBgm_ContentRoot", activityD, null, FrameworkCameraActivityPolicy.UseRoute, anchors, director);

            SetSerialized(routeCameraBinding, "routeCameraRig", routeRig);
            SetSerialized(routeCameraBinding, "routeAnchors", anchors);
            SetSerialized(routeCameraBinding, "director", director);
            SetSerialized(routeCameraBinding, "startupActivityCameraBinding", activityABinding);

            routeRig.SetActive(false);
            activityARig.SetActive(false);

            ValidateObjectReference(routeContentBinding, "route", gameplayRoute, "RouteContentBinding route on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "routeCameraRig", routeRig, "FrameworkRouteCameraBinding routeCameraRig on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "routeAnchors", anchors, "FrameworkRouteCameraBinding routeAnchors on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "director", director, "FrameworkRouteCameraBinding director on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "startupActivityCameraBinding", activityABinding, "FrameworkRouteCameraBinding startupActivityCameraBinding on Gameplay camera root");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static FrameworkActivityCameraBinding EnsureActivityBinding(
            Scene scene,
            string rootName,
            ActivityAsset expectedActivity,
            GameObject rig,
            FrameworkCameraActivityPolicy policy,
            FrameworkCameraAnchorHost anchors,
            FrameworkCameraDirector director)
        {
            GameObject root = FindInScene(scene, rootName);
            if (root == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Activity root not found. root='{rootName}'.");
                return null;
            }

            ActivityLocalVisibilityAdapter adapter = EnsureComponent<ActivityLocalVisibilityAdapter>(root);
            SetSerialized(adapter, "activity", expectedActivity);

            FrameworkActivityCameraBinding binding = EnsureComponent<FrameworkActivityCameraBinding>(root);
            SetSerialized(binding, "assignedActivity", expectedActivity);
            SetSerialized(binding, "activityCameraRig", rig);
            SetSerialized(binding, "policy", (int)policy);
            SetSerialized(binding, "anchors", anchors);
            SetSerialized(binding, "director", director);

            ValidateObjectReference(adapter, "activity", expectedActivity, $"ActivityLocalVisibilityAdapter activity on '{rootName}'");
            ValidateObjectReference(binding, "assignedActivity", expectedActivity, $"FrameworkActivityCameraBinding assignedActivity on '{rootName}'");
            ValidateObjectReference(binding, "director", director, $"FrameworkActivityCameraBinding director on '{rootName}'");
            ValidateObjectReference(binding, "anchors", anchors, $"FrameworkActivityCameraBinding anchors on '{rootName}'");

            if (rig != null)
            {
                ValidateObjectReference(binding, "activityCameraRig", rig, $"FrameworkActivityCameraBinding activityCameraRig on '{rootName}'");
            }

            return binding;
        }

        private static GameObject EnsureMainCamera(Scene scene, Vector3 position, Quaternion rotation)
        {
            GameObject cameraObject = FindInScene(scene, "Main Camera");
            if (cameraObject == null)
            {
                cameraObject = new GameObject("Main Camera");
                SceneManager.MoveGameObjectToScene(cameraObject, scene);
            }

            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetPositionAndRotation(position, rotation);
            return cameraObject;
        }

        private static GameObject EnsureRoot(Scene scene, string name)
        {
            GameObject root = FindInScene(scene, name);
            if (root != null)
            {
                return root;
            }

            root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return root;
        }

        private static GameObject EnsureCinemachineRig(Scene scene, string name, Vector3 position, Quaternion rotation)
        {
            GameObject rig = FindInScene(scene, name);
            if (rig == null)
            {
                rig = new GameObject(name);
                SceneManager.MoveGameObjectToScene(rig, scene);
            }

            rig.transform.SetPositionAndRotation(position, rotation);
            EnsureComponent<CinemachineCamera>(rig);
            RemoveForbiddenCameraComponents(rig, removeCamera: true, removeBrain: true);
            return rig;
        }

        private static void RemoveForbiddenCameraComponents(GameObject owner, bool removeCamera, bool removeBrain)
        {
            if (removeCamera && owner.TryGetComponent(out UnityEngine.Camera camera))
            {
                Object.DestroyImmediate(camera);
            }

            if (removeBrain && owner.TryGetComponent(out CinemachineBrain brain))
            {
                Object.DestroyImmediate(brain);
            }

            if (owner.TryGetComponent(out AudioListener listener))
            {
                Object.DestroyImmediate(listener);
            }
        }

        private static T EnsureComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static GameObject FindInScene(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform match = FindInChildren(root.transform, name);
                if (match != null)
                {
                    return match.gameObject;
                }
            }

            return null;
        }

        private static Transform FindInChildren(Transform root, string name)
        {
            if (root.name == name)
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform match = FindInChildren(root.GetChild(i), name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static T LoadRequiredAsset<T>(string path) where T : Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Required asset not found. path='{path}'.");
            }

            return asset;
        }

        private static void SetSerialized(Object target, string propertyName, Object value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.objectReferenceValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetSerialized(Object target, string propertyName, string value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.stringValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetSerialized(Object target, string propertyName, int value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.intValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetSerialized(Object target, string propertyName, bool value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.boolValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static SerializedProperty FindProperty(Object target, string propertyName)
        {
            if (target == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Cannot set property on null target. property='{propertyName}'.");
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Serialized property not found. target='{target.GetType().Name}' property='{propertyName}'.");
            }

            return property;
        }

        private static void ValidateObjectReference(Object target, string propertyName, Object expected, string context)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            Object actual = property.objectReferenceValue;
            if (!ReferenceEquals(actual, expected))
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Object reference was not assigned. context='{context}' expected='{FormatObject(expected)}' actual='{FormatObject(actual)}'.");
            }
        }

        private static void ReportBlockingIssue(string message)
        {
            hasBlockingIssue = true;
            Debug.LogError(message);
        }

        private static string FormatObject(Object value)
        {
            return value != null ? value.name : "<missing>";
        }
    }
}
