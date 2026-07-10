using System.Collections.Generic;
using Immersive.Framework.Camera;
using Immersive.Framework.Camera.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.Editor.Camera
{
    /// <summary>
    /// One-shot C8B5B cleanup for Route/Activity camera bindings that are not used by FIRSTGAME.
    /// Remove this script after Unity validation confirms the scene cleanup.
    /// </summary>
    public static class FirstGameResidualCameraBindingCleanup
    {
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";
        private const string LogPrefix = "[FIRSTGAME][C8B5B Camera Cleanup]";

        [MenuItem("FIRSTGAME/Immersive Framework/Camera Cleanup/Remove Residual Route Activity Bindings")]
        public static void RemoveResidualBindings()
        {
            Scene scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);

            RemovalResult result = new RemovalResult();
            RemoveComponents(scene, ref result);

            if (result.TotalRemoved > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            Debug.Log(
                $"{LogPrefix} removedActivity='{result.ActivityBindingCount}' removedRoute='{result.RouteBindingCount}' removedOutputSource='{result.OutputSourceCount}' total='{result.TotalRemoved}' objects=[{string.Join(", ", result.ObjectNames)}].");
        }

        private static void RemoveComponents(Scene scene, ref RemovalResult result)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                RemoveComponents(root.GetComponentsInChildren<FrameworkActivityCameraBinding>(true), "FrameworkActivityCameraBinding", ref result);
                RemoveComponents(root.GetComponentsInChildren<FrameworkRouteCameraBinding>(true), "FrameworkRouteCameraBinding", ref result);
                RemoveComponents(root.GetComponentsInChildren<FrameworkCinemachineCameraOutputSource>(true), "FrameworkCinemachineCameraOutputSource", ref result);
            }
        }

        private static void RemoveComponents<T>(T[] components, string kind, ref RemovalResult result)
            where T : Component
        {
            if (components == null)
            {
                return;
            }

            for (int i = 0; i < components.Length; i++)
            {
                T component = components[i];
                if (component == null)
                {
                    continue;
                }

                result.Register(kind, component.gameObject.name);
                Undo.DestroyObjectImmediate(component);
            }
        }

        private struct RemovalResult
        {
            private List<string> objectNames;

            public int ActivityBindingCount { get; private set; }

            public int RouteBindingCount { get; private set; }

            public int OutputSourceCount { get; private set; }

            public int TotalRemoved => ActivityBindingCount + RouteBindingCount + OutputSourceCount;

            public IReadOnlyList<string> ObjectNames => objectNames ?? (IReadOnlyList<string>)System.Array.Empty<string>();

            public void Register(string kind, string objectName)
            {
                switch (kind)
                {
                    case "FrameworkActivityCameraBinding":
                        ActivityBindingCount++;
                        break;
                    case "FrameworkRouteCameraBinding":
                        RouteBindingCount++;
                        break;
                    case "FrameworkCinemachineCameraOutputSource":
                        OutputSourceCount++;
                        break;
                }

                objectNames ??= new List<string>();
                objectNames.Add($"{kind}@{objectName}");
            }
        }
    }
}
