using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkModels.M02.Editor
{
    /// <summary>
    /// Materializes only the application-level scene foundation required by M02.
    /// It does not assign authoring references, install lifecycle participants,
    /// configure Build Profiles, or create fallback runtime authorities.
    /// </summary>
    internal static class M02ApplicationFoundationResolver
    {
        private const string MenuPath =
            "Tools/Immersive Framework/FIRSTGAME/M02/Resolve Application Foundation";

        private const string Root =
            "Assets/_Project/FrameworkModels/M02_LifecycleEvents";
        private const string ScenesFolder = Root + "/Scenes";

        private const string PersistentContentSourceScenePath =
            "Packages/com.immersive.framework/Editor/SceneTemplates/PersistentContent/PersistentContentTemplateSource.unity";
        private const string PersistentContentScenePath =
            ScenesFolder + "/M02_PersistentContent.unity";

        private static readonly List<string> Created = new List<string>();
        private static readonly List<string> Preserved = new List<string>();
        private static readonly List<string> Modified = new List<string>();

        [MenuItem(MenuPath, false, 111)]
        private static void ResolveApplicationFoundation()
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
                EnsureFolder(ScenesFolder);
                CreatePersistentContentSceneIfMissing();
                RemoveGeneratedSceneAuthorities();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            finally
            {
                RestorePreviousSceneSetup(previousSetup);
            }

            string summary =
                $"M02 application foundation resolved. " +
                $"created={Created.Count} modified={Modified.Count} preserved={Preserved.Count}. " +
                "Assign M02_PersistentContent as the Game Application Content Scene, then configure the M02 authoring graph manually.";

            Debug.Log(summary);
            EditorUtility.DisplayDialog(
                "M02 Application Foundation",
                summary,
                "OK");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateResolveApplicationFoundation()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
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
                    $"M02 requires the official Persistent Content source scene at '{PersistentContentSourceScenePath}'. No fallback scene was created.");
            }

            if (!AssetDatabase.CopyAsset(
                    PersistentContentSourceScenePath,
                    PersistentContentScenePath))
            {
                throw new InvalidOperationException(
                    $"Failed to create '{PersistentContentScenePath}' from the official Persistent Content source scene.");
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

                Scene scene =
                    EditorSceneManager.OpenScene(
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
                        $"M02 application foundation preserved '{scenePath}' because the generated root '{scenes[index].ScaffoldRootName}' was not found.");
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
                        string.Equals(candidate.name, "Main Camera", StringComparison.Ordinal) &&
                        candidate.GetComponent<Camera>() != null;

                    bool generatedEventSystem =
                        string.Equals(candidate.name, "EventSystem", StringComparison.Ordinal) &&
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
                        $"Failed to save '{scenePath}' after removing generated persistent authorities.");
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

        private static void RestorePreviousSceneSetup(SceneSetup[] previousSetup)
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
    }
}
