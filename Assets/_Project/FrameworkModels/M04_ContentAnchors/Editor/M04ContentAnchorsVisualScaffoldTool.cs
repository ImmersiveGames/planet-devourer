using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkModels.ContentAnchors.Editor
{
    /// <summary>
    /// Creates physical and visual placeholders for M04.
    ///
    /// Deliberately does not reference Immersive.Framework namespaces,
    /// add MonoBehaviours, configure framework assets, assign owners,
    /// wire events, or change build settings.
    /// </summary>
    internal static class M04ContentAnchorsVisualScaffoldTool
    {
        private const string LogPrefix =
            "[FIRSTGAME_M04_VISUAL_SCAFFOLD]";

        private const string MenuRoot =
            "Tools/Immersive Framework/FIRSTGAME/M04 Content Anchors/";

        private const string RouteScenePath =
            "Assets/_Project/FrameworkModels/M04_ContentAnchors/Scenes/M04_Route.unity";

        private const string ActivityAScenePath =
            "Assets/_Project/FrameworkModels/M04_ContentAnchors/Scenes/M04_ActivityA_Add.unity";

        private const string ActivityBScenePath =
            "Assets/_Project/FrameworkModels/M04_ContentAnchors/Scenes/M04_ActivityB_Add.unity";

        private const string ScaffoldRootName =
            "M04 Manual Authoring Visuals";

        private const string FrameworkMountName =
            "Framework Component Mount (Add Manually)";

        private const string BindingsMountName =
            "Bindings Mount (Configure Manually)";

        [MenuItem(MenuRoot + "Create or Refresh Visual Scaffold")]
        private static void CreateOrRefreshMenu()
        {
            Run(
                "CreateOrRefreshVisualScaffold",
                CreateOrRefresh);
        }

        [MenuItem(MenuRoot + "Validate Physical Scaffold")]
        private static void ValidateMenu()
        {
            Run(
                "ValidatePhysicalScaffold",
                Validate);
        }

        private static void CreateOrRefresh()
        {
            RequireScene(RouteScenePath);
            RequireScene(ActivityAScenePath);
            RequireScene(ActivityBScenePath);

            ConfigureRouteScene();
            ConfigureActivityScene(
                ActivityAScenePath,
                "ACTIVITY A",
                new Vector3(-2.8f, 0f, 0f));

            ConfigureActivityScene(
                ActivityBScenePath,
                "ACTIVITY B",
                new Vector3(2.8f, 0f, 0f));

            AssetDatabase.SaveAssets();

            Debug.Log(
                $"{LogPrefix} operation='CreateOrRefreshVisualScaffold' " +
                "status='Succeeded' frameworkComponentsAdded='0' " +
                "assetsConfigured='0' buildSettingsChanged='0'.");
        }

        private static void ConfigureRouteScene()
        {
            Scene scene =
                EditorSceneManager.OpenScene(
                    RouteScenePath,
                    OpenSceneMode.Single);

            Transform root =
                EnsureSceneRoot(
                    scene,
                    ScaffoldRootName);

            EnsureCandidate(
                root,
                "Route Root Candidate",
                "ROUTE ROOT CANDIDATE\nAdd RouteContentAnchor manually",
                PrimitiveType.Cylinder,
                new Vector3(-3f, 1f, 2f),
                new Vector3(1.4f, 1.4f, 1.4f));

            Transform navigation =
                EnsureChild(
                    root,
                    "Navigation Candidates (Configure Manually)");

            EnsureNavigationCandidate(
                navigation,
                "Activity A Request Candidate",
                "ACTIVITY A\nAdd official trigger manually",
                new Vector3(1.8f, 0.6f, 2f));

            EnsureNavigationCandidate(
                navigation,
                "Activity B Request Candidate",
                "ACTIVITY B\nAdd official trigger manually",
                new Vector3(4.2f, 0.6f, 2f));

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureActivityScene(
            string scenePath,
            string activityLabel,
            Vector3 offset)
        {
            Scene scene =
                EditorSceneManager.OpenScene(
                    scenePath,
                    OpenSceneMode.Single);

            Transform root =
                EnsureSceneRoot(
                    scene,
                    ScaffoldRootName);

            EnsureCandidate(
                root,
                "Activity Root Candidate",
                $"{activityLabel} ROOT CANDIDATE\nAdd ActivityContentAnchor manually",
                PrimitiveType.Cube,
                offset + new Vector3(-2f, 0.9f, 0f),
                new Vector3(1.4f, 1.4f, 1.4f));

            EnsureCandidate(
                root,
                "Activity Slot Candidate",
                $"{activityLabel} SLOT CANDIDATE\nAdd ActivityContentAnchor manually",
                PrimitiveType.Capsule,
                offset + new Vector3(0f, 1f, 0f),
                new Vector3(1f, 1.2f, 1f));

            EnsureCandidate(
                root,
                "Activity Point Candidate",
                $"{activityLabel} POINT CANDIDATE\nAdd ActivityContentAnchor manually",
                PrimitiveType.Sphere,
                offset + new Vector3(2f, 1f, 0f),
                Vector3.one);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static Transform EnsureCandidate(
            Transform parent,
            string objectName,
            string label,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale)
        {
            Transform existing =
                FindDirectChild(
                    parent,
                    objectName);

            Transform candidate;
            if (existing != null)
            {
                candidate = existing;
            }
            else
            {
                GameObject created =
                    GameObject.CreatePrimitive(
                        primitiveType);

                Undo.RegisterCreatedObjectUndo(
                    created,
                    $"Create {objectName}");

                created.name = objectName;
                candidate = created.transform;
                candidate.SetParent(
                    parent,
                    false);

                candidate.localPosition =
                    localPosition;

                candidate.localScale =
                    localScale;
            }

            EnsureLabel(
                candidate,
                label);

            EnsureChild(
                candidate,
                FrameworkMountName);

            EnsureChild(
                candidate,
                BindingsMountName);

            return candidate;
        }

        private static Transform EnsureNavigationCandidate(
            Transform parent,
            string objectName,
            string label,
            Vector3 localPosition)
        {
            return EnsureCandidate(
                parent,
                objectName,
                label,
                PrimitiveType.Cube,
                localPosition,
                new Vector3(1.8f, 0.3f, 0.8f));
        }

        private static void EnsureLabel(
            Transform parent,
            string text)
        {
            Transform existing =
                FindDirectChild(
                    parent,
                    "Label");

            TextMesh label;
            if (existing == null)
            {
                GameObject labelObject =
                    new GameObject(
                        "Label",
                        typeof(TextMesh));

                Undo.RegisterCreatedObjectUndo(
                    labelObject,
                    "Create scaffold label");

                labelObject.transform.SetParent(
                    parent,
                    false);

                labelObject.transform.localPosition =
                    new Vector3(0f, 1.3f, 0f);

                label = labelObject.GetComponent<TextMesh>();
                label.anchor = TextAnchor.MiddleCenter;
                label.alignment = TextAlignment.Center;
                label.characterSize = 0.08f;
                label.fontSize = 48;
            }
            else
            {
                label =
                    existing.GetComponent<TextMesh>();

                if (label == null)
                {
                    label =
                        Undo.AddComponent<TextMesh>(
                            existing.gameObject);
                }
            }

            if (!string.Equals(
                    label.text,
                    text,
                    StringComparison.Ordinal))
            {
                Undo.RecordObject(
                    label,
                    "Update scaffold label");

                label.text = text;
                EditorUtility.SetDirty(label);
            }
        }

        private static Transform EnsureSceneRoot(
            Scene scene,
            string rootName)
        {
            GameObject[] roots =
                scene.GetRootGameObjects();

            for (
                int index = 0;
                index < roots.Length;
                index++)
            {
                GameObject candidate =
                    roots[index];

                if (candidate != null &&
                    string.Equals(
                        candidate.name,
                        rootName,
                        StringComparison.Ordinal))
                {
                    return candidate.transform;
                }
            }

            GameObject created =
                new GameObject(
                    rootName);

            Undo.RegisterCreatedObjectUndo(
                created,
                $"Create {rootName}");

            SceneManager.MoveGameObjectToScene(
                created,
                scene);

            return created.transform;
        }

        private static Transform EnsureChild(
            Transform parent,
            string name)
        {
            Transform existing =
                FindDirectChild(
                    parent,
                    name);

            if (existing != null)
            {
                return existing;
            }

            GameObject created =
                new GameObject(
                    name);

            Undo.RegisterCreatedObjectUndo(
                created,
                $"Create {name}");

            created.transform.SetParent(
                parent,
                false);

            return created.transform;
        }

        private static Transform FindDirectChild(
            Transform parent,
            string name)
        {
            if (parent == null)
            {
                return null;
            }

            for (
                int index = 0;
                index < parent.childCount;
                index++)
            {
                Transform child =
                    parent.GetChild(index);

                if (child != null &&
                    string.Equals(
                        child.name,
                        name,
                        StringComparison.Ordinal))
                {
                    return child;
                }
            }

            return null;
        }

        private static void Validate()
        {
            var issues =
                new List<string>();

            ValidateRouteScene(
                issues);

            ValidateActivityScene(
                ActivityAScenePath,
                issues);

            ValidateActivityScene(
                ActivityBScenePath,
                issues);

            if (issues.Count > 0)
            {
                Debug.LogError(
                    $"{LogPrefix} operation='ValidatePhysicalScaffold' " +
                    $"status='Failed' issues='{issues.Count}'\n- " +
                    string.Join(
                        "\n- ",
                        issues));

                return;
            }

            Debug.Log(
                $"{LogPrefix} operation='ValidatePhysicalScaffold' " +
                "status='Passed' frameworkComponentsExpected='Manual' " +
                "assetConfigurationExpected='Manual'.");
        }

        private static void ValidateRouteScene(
            List<string> issues)
        {
            Scene scene =
                OpenForValidation(
                    RouteScenePath,
                    out bool openedTemporarily);

            try
            {
                Transform root =
                    FindSceneRoot(
                        scene,
                        ScaffoldRootName);

                RequireCandidate(
                    root,
                    "Route Root Candidate",
                    issues,
                    RouteScenePath);

                Transform navigation =
                    root != null
                        ? FindDirectChild(
                            root,
                            "Navigation Candidates (Configure Manually)")
                        : null;

                if (navigation == null)
                {
                    issues.Add(
                        $"{RouteScenePath} has no Navigation Candidates hierarchy.");
                }
                else
                {
                    RequireCandidate(
                        navigation,
                        "Activity A Request Candidate",
                        issues,
                        RouteScenePath);

                    RequireCandidate(
                        navigation,
                        "Activity B Request Candidate",
                        issues,
                        RouteScenePath);
                }
            }
            finally
            {
                CloseIfTemporary(
                    scene,
                    openedTemporarily);
            }
        }

        private static void ValidateActivityScene(
            string scenePath,
            List<string> issues)
        {
            Scene scene =
                OpenForValidation(
                    scenePath,
                    out bool openedTemporarily);

            try
            {
                Transform root =
                    FindSceneRoot(
                        scene,
                        ScaffoldRootName);

                RequireCandidate(
                    root,
                    "Activity Root Candidate",
                    issues,
                    scenePath);

                RequireCandidate(
                    root,
                    "Activity Slot Candidate",
                    issues,
                    scenePath);

                RequireCandidate(
                    root,
                    "Activity Point Candidate",
                    issues,
                    scenePath);
            }
            finally
            {
                CloseIfTemporary(
                    scene,
                    openedTemporarily);
            }
        }

        private static void RequireCandidate(
            Transform parent,
            string objectName,
            List<string> issues,
            string scenePath)
        {
            if (parent == null)
            {
                issues.Add(
                    $"{scenePath} has no '{ScaffoldRootName}' root.");

                return;
            }

            Transform candidate =
                FindDirectChild(
                    parent,
                    objectName);

            if (candidate == null)
            {
                issues.Add(
                    $"{scenePath} has no '{objectName}'.");

                return;
            }

            if (FindDirectChild(
                    candidate,
                    FrameworkMountName) == null)
            {
                issues.Add(
                    $"{scenePath}/{objectName} has no manual framework mount.");
            }

            if (FindDirectChild(
                    candidate,
                    BindingsMountName) == null)
            {
                issues.Add(
                    $"{scenePath}/{objectName} has no manual bindings mount.");
            }

            if (FindDirectChild(
                    candidate,
                    "Label") == null)
            {
                issues.Add(
                    $"{scenePath}/{objectName} has no label.");
            }
        }

        private static Transform FindSceneRoot(
            Scene scene,
            string rootName)
        {
            if (!scene.IsValid() ||
                !scene.isLoaded)
            {
                return null;
            }

            GameObject[] roots =
                scene.GetRootGameObjects();

            for (
                int index = 0;
                index < roots.Length;
                index++)
            {
                GameObject root =
                    roots[index];

                if (root != null &&
                    string.Equals(
                        root.name,
                        rootName,
                        StringComparison.Ordinal))
                {
                    return root.transform;
                }
            }

            return null;
        }

        private static Scene OpenForValidation(
            string scenePath,
            out bool openedTemporarily)
        {
            Scene scene =
                SceneManager.GetSceneByPath(
                    scenePath);

            openedTemporarily =
                !scene.IsValid() ||
                !scene.isLoaded;

            if (openedTemporarily)
            {
                scene =
                    EditorSceneManager.OpenScene(
                        scenePath,
                        OpenSceneMode.Additive);
            }

            return scene;
        }

        private static void CloseIfTemporary(
            Scene scene,
            bool openedTemporarily)
        {
            if (openedTemporarily &&
                scene.IsValid() &&
                scene.isLoaded)
            {
                EditorSceneManager.CloseScene(
                    scene,
                    true);
            }
        }

        private static void RequireScene(
            string scenePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    scenePath) == null)
            {
                throw new InvalidOperationException(
                    $"Required M04 scene is missing: {scenePath}");
            }
        }

        private static void Run(
            string operation,
            Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Debug.LogException(
                    new InvalidOperationException(
                        $"{LogPrefix} operation='{operation}' status='Failed'.",
                        exception));
            }
        }
    }
}
