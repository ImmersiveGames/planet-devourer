using System;
using System.Collections.Generic;
using System.Linq;
using Immersive.Framework.Authoring;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkModels.M02.Editor
{
    /// <summary>
    /// Materializes the complete, independent M02 authoring foundation.
    /// Existing files are preserved. No cross-asset references, lifecycle
    /// participants, bootstrap objects, Build Profiles or ProjectSettings are assigned.
    /// </summary>
    internal static class M02ApplicationFoundationResolver
    {
        private const string CreateMenuPath =
            "Tools/Immersive Framework/FIRSTGAME/M02/Create Missing Scaffold";
        private const string ResolveMenuPath =
            "Tools/Immersive Framework/FIRSTGAME/M02/Resolve Application Foundation";

        private const string Root =
            "Assets/_Project/FrameworkModels/M02_LifecycleEvents";
        private const string ApplicationFolder = Root + "/Application";
        private const string RoutesFolder = Root + "/Routes";
        private const string ActivitiesFolder = Root + "/Activities";
        private const string ProfilesFolder = Root + "/Profiles";
        private const string ScenesFolder = Root + "/Scenes";
        private const string PrefabsFolder = Root + "/Prefabs";

        private const string PersistentContentSourceScenePath =
            "Packages/com.immersive.framework/Editor/SceneTemplates/PersistentContent/PersistentContentTemplateSource.unity";
        private const string PersistentContentScenePath =
            ScenesFolder + "/M02_PersistentContent.unity";

        private static readonly List<string> Created = new List<string>();
        private static readonly List<string> Preserved = new List<string>();
        private static readonly List<string> Modified = new List<string>();

        [MenuItem(CreateMenuPath, false, 110)]
        private static void CreateMissingScaffold()
        {
            Execute(includePersistentContent: false, normalizeSceneAuthorities: false);
        }

        [MenuItem(CreateMenuPath, true)]
        [MenuItem(ResolveMenuPath, true)]
        private static bool ValidateCommands()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        [MenuItem(ResolveMenuPath, false, 111)]
        private static void ResolveApplicationFoundation()
        {
            Execute(includePersistentContent: true, normalizeSceneAuthorities: true);
        }

        private static void Execute(
            bool includePersistentContent,
            bool normalizeSceneAuthorities)
        {
            SceneSetup[] previousSetup =
                EditorSceneManager.GetSceneManagerSetup();

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Created.Clear();
            Preserved.Clear();
            Modified.Clear();

            try
            {
                EnsureFolders();
                CreateMissingAuthoringAssets();
                CreateMissingScenes();
                CreateMissingPrefabs();

                if (includePersistentContent)
                {
                    CreatePersistentContentSceneIfMissing();
                }

                if (normalizeSceneAuthorities)
                {
                    RemoveGeneratedSceneAuthorities();
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            finally
            {
                RestorePreviousSceneSetup(previousSetup);
            }

            string mode = includePersistentContent
                ? "application foundation"
                : "scaffold";

            string summary =
                $"M02 {mode} resolved. " +
                $"created={Created.Count} modified={Modified.Count} preserved={Preserved.Count}.\n\n" +
                "Unity entry scene: M02_Boot.\n" +
                "Game Application Content Scene: M02_PersistentContent.\n" +
                "Game Application Startup Route: Route_M02_A.\n" +
                "Route_M02_A Primary Scene: M02_RouteA.\n\n" +
                "No authoring references, lifecycle participants, bootstrap objects, Build Profiles or ProjectSettings were assigned.";

            Debug.Log(summary);
            EditorUtility.DisplayDialog(
                "M02 Authoring Foundation",
                summary,
                "OK");
        }

        private static void EnsureFolders()
        {
            EnsureFolder(Root);
            EnsureFolder(ApplicationFolder);
            EnsureFolder(RoutesFolder);
            EnsureFolder(ActivitiesFolder);
            EnsureFolder(ProfilesFolder);
            EnsureFolder(ScenesFolder);
            EnsureFolder(PrefabsFolder);
        }

        private static void CreateMissingAuthoringAssets()
        {
            CreateAuthoringAsset<GameApplicationAsset>(
                ApplicationFolder + "/GA_M02_Lifecycle.asset",
                "GA_M02_Lifecycle",
                "M02 Lifecycle Events",
                "Independent Game Application for the M02 Lifecycle Events demonstration.",
                Array.Empty<string>());

            CreateAuthoringAsset<RouteAsset>(
                RoutesFolder + "/Route_M02_A.asset",
                "Route_M02_A",
                "M02 Route A",
                "Route A for the M02 Lifecycle Events demonstration.",
                new[] { "routeId" });

            CreateAuthoringAsset<RouteAsset>(
                RoutesFolder + "/Route_M02_B.asset",
                "Route_M02_B",
                "M02 Route B",
                "Route B for the M02 Lifecycle Events demonstration.",
                new[] { "routeId" });

            CreateAuthoringAsset<ActivityAsset>(
                ActivitiesFolder + "/Activity_M02_A.asset",
                "Activity_M02_A",
                "M02 Activity A",
                "Activity A for the M02 Lifecycle Events demonstration.",
                new[] { "activityId" });

            CreateAuthoringAsset<ActivityAsset>(
                ActivitiesFolder + "/Activity_M02_B.asset",
                "Activity_M02_B",
                "M02 Activity B",
                "Activity B for the M02 Lifecycle Events demonstration.",
                new[] { "activityId" });

            CreateAuthoringAsset<ActivityContentProfileAsset>(
                ProfilesFolder + "/ActivityContent_M02_A.asset",
                "ActivityContent_M02_A",
                "M02 Activity Content A",
                "Additive scene content for M02 Activity A.",
                Array.Empty<string>());

            CreateAuthoringAsset<ActivityContentProfileAsset>(
                ProfilesFolder + "/ActivityContent_M02_B.asset",
                "ActivityContent_M02_B",
                "M02 Activity Content B",
                "Additive scene content for M02 Activity B.",
                Array.Empty<string>());
        }

        private static void CreateAuthoringAsset<T>(
            string path,
            string assetName,
            string displayName,
            string description,
            string[] identityPropertyNames)
            where T : ScriptableObject
        {
            UnityEngine.Object existing =
                AssetDatabase.LoadMainAssetAtPath(path);
            if (existing != null)
            {
                Preserved.Add(path);
                return;
            }

            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = assetName;
            AssetDatabase.CreateAsset(asset, path);

            var serialized = new SerializedObject(asset);
            SetFirstString(
                serialized,
                displayName,
                "applicationName",
                "routeName",
                "activityName",
                "displayName",
                "profileName");
            SetFirstString(
                serialized,
                description,
                "description",
                "authoringDescription",
                "notes");

            if (identityPropertyNames != null && identityPropertyNames.Length > 0)
            {
                SetFirstString(
                    serialized,
                    Guid.NewGuid().ToString("N"),
                    identityPropertyNames);
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            Created.Add(path);
        }

        private static void SetFirstString(
            SerializedObject serialized,
            string value,
            params string[] propertyNames)
        {
            if (serialized == null || string.IsNullOrEmpty(value) || propertyNames == null)
            {
                return;
            }

            for (int index = 0; index < propertyNames.Length; index++)
            {
                SerializedProperty property =
                    serialized.FindProperty(propertyNames[index]);
                if (property == null ||
                    property.propertyType != SerializedPropertyType.String)
                {
                    continue;
                }

                property.stringValue = value;
                return;
            }
        }

        private static void CreateMissingScenes()
        {
            CreateSceneIfMissing("M02_Boot", SceneRole.Boot);
            CreateSceneIfMissing("M02_RouteA", SceneRole.Route);
            CreateSceneIfMissing("M02_RouteB", SceneRole.Route);
            CreateSceneIfMissing("M02_ActivityA_Add", SceneRole.Activity);
            CreateSceneIfMissing("M02_ActivityB_Add", SceneRole.Activity);
        }

        private static void CreateSceneIfMissing(
            string sceneName,
            SceneRole role)
        {
            string path = ScenesFolder + "/" + sceneName + ".unity";
            SceneAsset existing =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (existing != null)
            {
                Preserved.Add(path);
                return;
            }

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject(sceneName + "_Root");
            var identity = new GameObject("Scene Identity");
            identity.transform.SetParent(root.transform, false);

            if (role == SceneRole.Boot)
            {
                var entry = new GameObject(
                    "Application Entry (Configured Through Framework Settings)");
                entry.transform.SetParent(root.transform, false);
            }
            else
            {
                PrimitiveType primitive = role == SceneRole.Route
                    ? PrimitiveType.Cube
                    : PrimitiveType.Sphere;

                GameObject visual = GameObject.CreatePrimitive(primitive);
                visual.name = role == SceneRole.Route
                    ? "Route Visual Placeholder"
                    : "Activity Visual Placeholder";
                visual.transform.SetParent(root.transform, false);
                visual.transform.localPosition = Vector3.up * 0.75f;
                visual.transform.localScale = role == SceneRole.Route
                    ? new Vector3(3f, 0.5f, 3f)
                    : Vector3.one * 1.25f;

                Collider collider = visual.GetComponent<Collider>();
                if (collider != null)
                {
                    UnityEngine.Object.DestroyImmediate(collider);
                }
            }

            GameObject label = CreateWorldLabel(
                sceneName,
                new Vector3(0f, 2.2f, 0f));
            label.transform.SetParent(root.transform, false);

            if (!EditorSceneManager.SaveScene(scene, path))
            {
                throw new InvalidOperationException(
                    $"Failed to create M02 scene '{path}'.");
            }

            Created.Add(path);
        }

        private static void CreateMissingPrefabs()
        {
            CreatePrefabIfMissing("PF_M02_SceneLifecycleObject");
            CreatePrefabIfMissing("PF_M02_RouteLifecycleObject");
            CreatePrefabIfMissing("PF_M02_ActivityLifecycleObject");
        }

        private static void CreatePrefabIfMissing(string prefabName)
        {
            string path = PrefabsFolder + "/" + prefabName + ".prefab";
            GameObject existing =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                Preserved.Add(path);
                return;
            }

            var root = new GameObject(prefabName);
            try
            {
                GameObject visual = GameObject.CreatePrimitive(
                    PrimitiveType.Cube);
                visual.name = "Visual Placeholder";
                visual.transform.SetParent(root.transform, false);
                visual.transform.localPosition = Vector3.up * 0.65f;
                visual.transform.localScale = Vector3.one * 1.25f;

                Collider collider = visual.GetComponent<Collider>();
                if (collider != null)
                {
                    UnityEngine.Object.DestroyImmediate(collider);
                }

                GameObject label = CreateWorldLabel(
                    prefabName,
                    new Vector3(0f, 1.65f, 0f));
                label.transform.SetParent(root.transform, false);

                new GameObject("Framework Participant (Configure Manually)")
                    .transform.SetParent(root.transform, false);
                new GameObject("Presentation Binding (Configure Manually)")
                    .transform.SetParent(root.transform, false);

                GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, path);
                if (saved == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to create M02 prefab '{path}'.");
                }

                Created.Add(path);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static GameObject CreateWorldLabel(
            string text,
            Vector3 localPosition)
        {
            var label = new GameObject("Label");
            label.transform.localPosition = localPosition;

            TextMesh textMesh = label.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.1f;
            textMesh.fontSize = 48;

            return label;
        }

        private static void CreatePersistentContentSceneIfMissing()
        {
            SceneAsset existing =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    PersistentContentScenePath);
            if (existing != null)
            {
                Preserved.Add(PersistentContentScenePath);
                return;
            }

            SceneAsset source =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    PersistentContentSourceScenePath);
            if (source == null)
            {
                throw new InvalidOperationException(
                    $"M02 requires the official Persistent Content source scene at '{PersistentContentSourceScenePath}'. No fallback and no M01 scene reuse are allowed.");
            }

            if (!AssetDatabase.CopyAsset(
                    PersistentContentSourceScenePath,
                    PersistentContentScenePath))
            {
                throw new InvalidOperationException(
                    $"Failed to create '{PersistentContentScenePath}' from the official package source scene.");
            }

            AssetDatabase.ImportAsset(
                PersistentContentScenePath,
                ImportAssetOptions.ForceUpdate);
            Created.Add(PersistentContentScenePath);
        }

        private static void RemoveGeneratedSceneAuthorities()
        {
            (string ScenePath, string ScaffoldRootName)[] scenes =
            {
                (ScenesFolder + "/M02_Boot.unity", "M02_Boot_Root"),
                (ScenesFolder + "/M02_RouteA.unity", "M02_RouteA_Root"),
                (ScenesFolder + "/M02_RouteB.unity", "M02_RouteB_Root")
            };

            for (int index = 0; index < scenes.Length; index++)
            {
                string scenePath = scenes[index].ScenePath;
                SceneAsset sceneAsset =
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (sceneAsset == null)
                {
                    Preserved.Add(scenePath + " (missing)");
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(
                    scenePath,
                    OpenSceneMode.Single);

                GameObject scaffoldRoot =
                    scene.GetRootGameObjects()
                        .FirstOrDefault(
                            root => string.Equals(
                                root.name,
                                scenes[index].ScaffoldRootName,
                                StringComparison.Ordinal));

                if (scaffoldRoot == null)
                {
                    Preserved.Add(scenePath + " (unrecognized hierarchy)");
                    Debug.LogWarning(
                        $"M02 foundation preserved '{scenePath}' because generated root '{scenes[index].ScaffoldRootName}' was not found.");
                    continue;
                }

                bool changed = false;
                GameObject[] roots = scene.GetRootGameObjects();

                for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                {
                    GameObject candidate = roots[rootIndex];
                    if (candidate == null)
                    {
                        continue;
                    }

                    bool generatedCamera =
                        string.Equals(
                            candidate.name,
                            "Main Camera",
                            StringComparison.Ordinal) &&
                        candidate.GetComponent<Camera>() != null;

                    bool generatedEventSystem =
                        string.Equals(
                            candidate.name,
                            "EventSystem",
                            StringComparison.Ordinal) &&
                        candidate.GetComponent<EventSystem>() != null;

                    if (!generatedCamera && !generatedEventSystem)
                    {
                        continue;
                    }

                    UnityEngine.Object.DestroyImmediate(candidate);
                    changed = true;
                }

                if (!changed)
                {
                    Preserved.Add(scenePath);
                    continue;
                }

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene))
                {
                    throw new InvalidOperationException(
                        $"Failed to save '{scenePath}' after removing generated scene authorities.");
                }

                Modified.Add(scenePath);
            }
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];

            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private static void RestorePreviousSceneSetup(
            SceneSetup[] previousSetup)
        {
            if (previousSetup != null && previousSetup.Length > 0)
            {
                EditorSceneManager.RestoreSceneManagerSetup(previousSetup);
                return;
            }

            EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);
        }

        private enum SceneRole
        {
            Boot,
            Route,
            Activity
        }
    }
}
