using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Immersive.Framework.Authoring;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FirstGame.FrameworkModels.M01.Editor
{
    internal static class M01InitialScaffoldGenerator
    {
        private const string Root =
            "Assets/_Project/FrameworkModels/M01_RouteActivity";

        private const string ApplicationFolder = Root + "/Application";
        private const string RoutesFolder = Root + "/Routes";
        private const string ActivitiesFolder = Root + "/Activities";
        private const string ProfilesFolder = Root + "/Profiles";
        private const string ScenesFolder = Root + "/Scenes";
        private const string PrefabsFolder = Root + "/Prefabs";
        private const string MaterialsFolder = Root + "/Materials";

        private static readonly List<string> Created = new List<string>();
        private static readonly List<string> Preserved = new List<string>();

        [MenuItem(
            "Tools/Immersive Framework/FIRSTGAME/M01/Create Missing Scaffold",
            false,
            101)]
        private static void CreateMissingScaffold()
        {
            SceneSetup[] previousSetup =
                EditorSceneManager.GetSceneManagerSetup();

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Created.Clear();
            Preserved.Clear();

            EnsureFolder(ApplicationFolder);
            EnsureFolder(RoutesFolder);
            EnsureFolder(ActivitiesFolder);
            EnsureFolder(ProfilesFolder);
            EnsureFolder(ScenesFolder);
            EnsureFolder(PrefabsFolder);
            EnsureFolder(MaterialsFolder);

            CreateAuthoringAssets();
            MaterialSet materials = CreateMaterials();
            CreatePrefabs();

            try
            {
                CreateScenes(materials);
            }
            finally
            {
                RestorePreviousSceneSetup(previousSetup);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string summary =
                $"M01 scaffold finished. created={Created.Count} preserved={Preserved.Count}. " +
                "No Game Application, Route, Activity, Profile or scene references were assigned.";

            Debug.Log(summary);
            EditorUtility.DisplayDialog(
                "M01 Scaffold",
                summary +
                "\n\nNext: follow README.md and configure the authoring graph manually.",
                "OK");
        }

        [MenuItem(
            "Tools/Immersive Framework/FIRSTGAME/M01/Select Model Folder",
            false,
            102)]
        private static void SelectModelFolder()
        {
            UnityEngine.Object folder =
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Root);
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }

        private static void CreateAuthoringAssets()
        {
            CreateAsset<GameApplicationAsset>(
                ApplicationFolder + "/GA_M01_RouteActivity.asset",
                serializedObject =>
                {
                    SetString(
                        serializedObject,
                        "applicationName",
                        "M01 Route and Activity");
                });

            CreateAsset<RouteAsset>(
                RoutesFolder + "/Route_M01_Menu.asset",
                serializedObject =>
                {
                    SetString(serializedObject, "routeId", NewStableId());
                    SetString(serializedObject, "routeName", "M01 Menu");
                    SetString(
                        serializedObject,
                        "description",
                        "Menu Route for the M01 Route and Activity model. Configure its Primary Scene manually and leave First Activity empty.");
                });

            CreateAsset<RouteAsset>(
                RoutesFolder + "/Route_M01_Gameplay.asset",
                serializedObject =>
                {
                    SetString(serializedObject, "routeId", NewStableId());
                    SetString(serializedObject, "routeName", "M01 Gameplay");
                    SetString(
                        serializedObject,
                        "description",
                        "Gameplay Route for the M01 Route and Activity model. Configure its Primary Scene and First Activity manually.");
                });

            CreateAsset<ActivityAsset>(
                ActivitiesFolder + "/Activity_M01_A.asset",
                serializedObject =>
                {
                    SetString(serializedObject, "activityId", NewStableId());
                    SetString(serializedObject, "activityName", "M01 Activity A");
                    SetString(
                        serializedObject,
                        "description",
                        "First Activity in M01. Configure its Activity Content Profile manually; Player participation remains No Slots.");
                });

            CreateAsset<ActivityAsset>(
                ActivitiesFolder + "/Activity_M01_B.asset",
                serializedObject =>
                {
                    SetString(serializedObject, "activityId", NewStableId());
                    SetString(serializedObject, "activityName", "M01 Activity B");
                    SetString(
                        serializedObject,
                        "description",
                        "Second Activity in M01. Configure its Activity Content Profile manually; Player participation remains No Slots.");
                });

            CreateAsset<ActivityContentProfileAsset>(
                ProfilesFolder + "/ActivityContent_M01_A.asset",
                serializedObject =>
                {
                    SetString(
                        serializedObject,
                        "description",
                        "Activity-owned scene declaration for M01 Activity A. Add M01_ActivityA_Add manually.");
                });

            CreateAsset<ActivityContentProfileAsset>(
                ProfilesFolder + "/ActivityContent_M01_B.asset",
                serializedObject =>
                {
                    SetString(
                        serializedObject,
                        "description",
                        "Activity-owned scene declaration for M01 Activity B. Add M01_ActivityB_Add manually.");
                });
        }

        private static void CreateScenes(MaterialSet materials)
        {
            CreateSceneIfMissing(
                ScenesFolder + "/M01_Boot.unity",
                () => BuildBootScene(materials));

            CreateSceneIfMissing(
                ScenesFolder + "/M01_Menu.unity",
                () => BuildMenuScene(materials));

            CreateSceneIfMissing(
                ScenesFolder + "/M01_Gameplay.unity",
                () => BuildGameplayScene(materials));

            CreateSceneIfMissing(
                ScenesFolder + "/M01_ActivityA_Add.unity",
                () => BuildActivityAScene(materials));

            CreateSceneIfMissing(
                ScenesFolder + "/M01_ActivityB_Add.unity",
                () => BuildActivityBScene(materials));
        }

        private static void CreatePrefabs()
        {
            CreatePrefabIfMissing(
                PrefabsFolder + "/PF_M01_RouteNavigation.prefab",
                () => CreateNavigationPanel(
                    "PF_M01_RouteNavigation",
                    new[] { "Open Gameplay", "Back to Menu" }));

            CreatePrefabIfMissing(
                PrefabsFolder + "/PF_M01_ActivityNavigation.prefab",
                () => CreateNavigationPanel(
                    "PF_M01_ActivityNavigation",
                    new[] { "Activity A", "Activity B" }));

            CreatePrefabIfMissing(
                PrefabsFolder + "/PF_M01_CurrentContextDisplay.prefab",
                CreateContextDisplay);
        }

        private static void BuildBootScene(MaterialSet materials)
        {
            CreateSceneEnvironment(
                "M01_Boot_Root",
                "M01 — Boot Shell",
                "Bootstrap and application selection remain manual.",
                materials.Neutral,
                createUiMount: true);

            CreatePrimitive(
                PrimitiveType.Sphere,
                "BootMarker",
                new Vector3(0f, 1.4f, 0f),
                new Vector3(1.8f, 1.8f, 1.8f),
                materials.Menu);
        }

        private static void BuildMenuScene(MaterialSet materials)
        {
            CreateSceneEnvironment(
                "M01_Menu_Root",
                "M01 — Menu Route",
                "Mount PF_M01_RouteNavigation and configure Route requests.",
                materials.Menu,
                createUiMount: true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "MenuDestinationMarker",
                new Vector3(0f, 1.1f, 0f),
                new Vector3(4f, 2f, 0.6f),
                materials.Menu);
        }

        private static void BuildGameplayScene(MaterialSet materials)
        {
            CreateSceneEnvironment(
                "M01_Gameplay_Root",
                "M01 — Gameplay Route",
                "This environment must remain while Activity A and B alternate.",
                materials.Gameplay,
                createUiMount: true);

            for (int index = -2; index <= 2; index++)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"GameplayPillar_{index + 3}",
                    new Vector3(index * 2.25f, 1.25f, 2.8f),
                    new Vector3(0.8f, 2.5f, 0.8f),
                    materials.Gameplay);
            }
        }

        private static void BuildActivityAScene(MaterialSet materials)
        {
            GameObject root = new GameObject("M01_ActivityA_Content");

            for (int index = 0; index < 3; index++)
            {
                GameObject sphere = CreatePrimitive(
                    PrimitiveType.Sphere,
                    $"ActivityA_Orb_{index + 1}",
                    new Vector3(-2.5f + index * 2.5f, 1.25f, 0f),
                    Vector3.one * 1.2f,
                    materials.ActivityA);
                sphere.transform.SetParent(root.transform, true);
            }
        }

        private static void BuildActivityBScene(MaterialSet materials)
        {
            GameObject root = new GameObject("M01_ActivityB_Content");

            Vector3[] positions =
            {
                new Vector3(-2f, 0.75f, -1.5f),
                new Vector3(2f, 0.75f, -1.5f),
                new Vector3(-2f, 0.75f, 1.5f),
                new Vector3(2f, 0.75f, 1.5f)
            };

            for (int index = 0; index < positions.Length; index++)
            {
                GameObject cube = CreatePrimitive(
                    PrimitiveType.Cube,
                    $"ActivityB_Block_{index + 1}",
                    positions[index],
                    new Vector3(1.3f, 1.5f, 1.3f),
                    materials.ActivityB);
                cube.transform.SetParent(root.transform, true);
            }
        }

        private static void CreateSceneEnvironment(
            string rootName,
            string title,
            string subtitle,
            Material worldMaterial,
            bool createUiMount)
        {
            GameObject root = new GameObject(rootName);

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(root.transform, false);
            cameraObject.transform.position = new Vector3(0f, 6.5f, -11f);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.045f, 0.06f, 1f);
            cameraObject.transform.LookAt(new Vector3(0f, 1f, 0f));

            GameObject lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(root.transform, false);
            lightObject.transform.rotation =
                Quaternion.Euler(50f, -35f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;

            GameObject floor = CreatePrimitive(
                PrimitiveType.Cube,
                "Floor",
                new Vector3(0f, -0.25f, 0f),
                new Vector3(14f, 0.5f, 10f),
                worldMaterial);
            floor.transform.SetParent(root.transform, true);

            CreateSceneCanvas(root.transform, title, subtitle, createUiMount);
        }

        private static void CreateSceneCanvas(
            Transform parent,
            string title,
            string subtitle,
            bool createUiMount)
        {
            GameObject canvasObject = new GameObject(
                "M01_UI",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 1f;

            CreateSceneText(
                canvasObject.transform,
                "Title",
                title,
                44,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -70f),
                new Vector2(1100f, 70f));

            CreateSceneText(
                canvasObject.transform,
                "Subtitle",
                subtitle,
                24,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -128f),
                new Vector2(1300f, 52f));

            if (createUiMount)
            {
                GameObject mount = new GameObject(
                    "M01_UI_Mount",
                    typeof(RectTransform));
                mount.transform.SetParent(canvasObject.transform, false);
                RectTransform mountRect = mount.GetComponent<RectTransform>();
                mountRect.anchorMin = new Vector2(0.5f, 0f);
                mountRect.anchorMax = new Vector2(0.5f, 0f);
                mountRect.pivot = new Vector2(0.5f, 0f);
                mountRect.anchoredPosition = new Vector2(0f, 60f);
                mountRect.sizeDelta = new Vector2(720f, 360f);
            }

            CreateEventSystem(parent);
        }

        private static void CreateEventSystem(Transform parent)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(parent, false);
            eventSystemObject.AddComponent<EventSystem>();

            Type inputSystemModule =
                Type.GetType(
                    "UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");

            if (inputSystemModule != null &&
                typeof(BaseInputModule).IsAssignableFrom(inputSystemModule))
            {
                eventSystemObject.AddComponent(inputSystemModule);
                return;
            }

            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static GameObject CreateNavigationPanel(
            string name,
            IReadOnlyList<string> buttonLabels)
        {
            GameObject root = new GameObject(
                name,
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(520f, 260f);

            Image background = root.GetComponent<Image>();
            background.color = new Color(0.06f, 0.075f, 0.1f, 0.92f);

            VerticalLayoutGroup layout =
                root.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 24, 24);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            ContentSizeFitter fitter =
                root.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (string label in buttonLabels)
            {
                CreateButton(root.transform, label);
            }

            return root;
        }

        private static GameObject CreateContextDisplay()
        {
            GameObject root = new GameObject(
                "PF_M01_CurrentContextDisplay",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(620f, 160f);

            Image background = root.GetComponent<Image>();
            background.color = new Color(0.06f, 0.075f, 0.1f, 0.92f);

            VerticalLayoutGroup layout =
                root.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(22, 22, 18, 18);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter =
                root.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CreatePanelText(root.transform, "Current Route: <configure>", 26);
            CreatePanelText(root.transform, "Current Activity: <configure>", 26);

            return root;
        }

        private static void CreateButton(Transform parent, string label)
        {
            GameObject buttonObject = new GameObject(
                SanitizeName(label),
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.16f, 0.25f, 0.38f, 1f);

            LayoutElement layout = buttonObject.GetComponent<LayoutElement>();
            layout.preferredHeight = 72f;

            Button button = buttonObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(0.23f, 0.36f, 0.53f, 1f);
            colors.pressedColor = new Color(0.11f, 0.18f, 0.28f, 1f);
            button.colors = colors;

            GameObject textObject = new GameObject(
                "Label",
                typeof(RectTransform),
                typeof(Text));
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            text.font = GetBuiltinFont();
            text.text = label;
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        private static void CreatePanelText(
            Transform parent,
            string value,
            int fontSize)
        {
            GameObject textObject = new GameObject(
                SanitizeName(value),
                typeof(RectTransform),
                typeof(Text),
                typeof(LayoutElement));
            textObject.transform.SetParent(parent, false);

            LayoutElement layout = textObject.GetComponent<LayoutElement>();
            layout.preferredHeight = 42f;

            Text text = textObject.GetComponent<Text>();
            text.font = GetBuiltinFont();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = Color.white;
        }

        private static void CreateSceneText(
            Transform parent,
            string name,
            string value,
            int fontSize,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            GameObject textObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            Text text = textObject.GetComponent<Text>();
            text.font = GetBuiltinFont();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        private static Font GetBuiltinFont()
        {
            Font font =
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        private static GameObject CreatePrimitive(
            PrimitiveType primitiveType,
            string name,
            Vector3 position,
            Vector3 scale,
            Material material)
        {
            GameObject gameObject =
                GameObject.CreatePrimitive(primitiveType);
            gameObject.name = name;
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }

            return gameObject;
        }

        private static MaterialSet CreateMaterials()
        {
            return new MaterialSet(
                GetOrCreateMaterial(
                    MaterialsFolder + "/M_M01_Neutral.mat",
                    new Color(0.32f, 0.36f, 0.42f, 1f)),
                GetOrCreateMaterial(
                    MaterialsFolder + "/M_M01_Menu.mat",
                    new Color(0.12f, 0.38f, 0.68f, 1f)),
                GetOrCreateMaterial(
                    MaterialsFolder + "/M_M01_Gameplay.mat",
                    new Color(0.16f, 0.50f, 0.30f, 1f)),
                GetOrCreateMaterial(
                    MaterialsFolder + "/M_M01_ActivityA.mat",
                    new Color(0.92f, 0.45f, 0.12f, 1f)),
                GetOrCreateMaterial(
                    MaterialsFolder + "/M_M01_ActivityB.mat",
                    new Color(0.55f, 0.24f, 0.78f, 1f)));
        }

        private static Material GetOrCreateMaterial(
            string path,
            Color color)
        {
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                Preserved.Add(path);
                return existing;
            }

            EnsurePathAvailableForCreation(path);

            Shader shader =
                Shader.Find("Universal Render Pipeline/Lit") ??
                Shader.Find("Standard");

            if (shader == null)
            {
                Debug.LogError(
                    $"M01 scaffold could not find a supported material shader for '{path}'.");
                return null;
            }

            Material material = new Material(shader)
            {
                name = Path.GetFileNameWithoutExtension(path)
            };

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else
            {
                material.color = color;
            }

            AssetDatabase.CreateAsset(material, path);
            Created.Add(path);
            return material;
        }

        private static void CreateSceneIfMissing(
            string path,
            Action buildScene)
        {
            SceneAsset existing =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            if (existing != null)
            {
                Preserved.Add(path);
                return;
            }

            EnsurePathAvailableForCreation(path);

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            buildScene();
            EditorSceneManager.MarkSceneDirty(scene);

            if (!EditorSceneManager.SaveScene(scene, path, false))
            {
                throw new InvalidOperationException(
                    $"M01 scaffold failed to save scene '{path}'.");
            }

            Created.Add(path);
        }

        private static void CreatePrefabIfMissing(
            string path,
            Func<GameObject> createRoot)
        {
            GameObject existing =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (existing != null)
            {
                Preserved.Add(path);
                return;
            }

            EnsurePathAvailableForCreation(path);

            GameObject root = createRoot();
            try
            {
                PrefabUtility.SaveAsPrefabAsset(root, path);
                Created.Add(path);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void CreateAsset<T>(
            string path,
            Action<SerializedObject> configure)
            where T : ScriptableObject
        {
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                Preserved.Add(path);
                return;
            }

            EnsurePathAvailableForCreation(path);

            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = Path.GetFileNameWithoutExtension(path);

            SerializedObject serializedObject = new SerializedObject(asset);
            configure(serializedObject);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(asset, path);
            Created.Add(path);
        }

        private static void EnsurePathAvailableForCreation(string path)
        {
            UnityEngine.Object existing =
                AssetDatabase.LoadMainAssetAtPath(path);

            if (existing != null || File.Exists(path))
            {
                throw new InvalidOperationException(
                    $"M01 scaffold found an incompatible existing file at '{path}'.");
            }
        }

        private static void SetString(
            SerializedObject serializedObject,
            string propertyName,
            string value)
        {
            SerializedProperty property =
                serializedObject.FindProperty(propertyName);

            if (property == null)
            {
                throw new InvalidOperationException(
                    $"M01 scaffold could not find serialized property '{propertyName}' on '{serializedObject.targetObject.GetType().FullName}'.");
            }

            property.stringValue = value ?? string.Empty;
        }

        private static string NewStableId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];

            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }

        private static void RestorePreviousSceneSetup(
            IReadOnlyList<SceneSetup> previousSetup)
        {
            if (previousSetup == null || previousSetup.Count == 0)
            {
                return;
            }

            if (previousSetup.Any(
                    setup => string.IsNullOrWhiteSpace(setup.path)))
            {
                return;
            }

            EditorSceneManager.RestoreSceneManagerSetup(
                previousSetup.ToArray());
        }

        private static string SanitizeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Item";
            }

            char[] invalid = Path.GetInvalidFileNameChars();
            string result = value;
            foreach (char character in invalid)
            {
                result = result.Replace(character, '_');
            }

            return result.Replace(' ', '_');
        }

        private readonly struct MaterialSet
        {
            public MaterialSet(
                Material neutral,
                Material menu,
                Material gameplay,
                Material activityA,
                Material activityB)
            {
                Neutral = neutral;
                Menu = menu;
                Gameplay = gameplay;
                ActivityA = activityA;
                ActivityB = activityB;
            }

            public Material Neutral { get; }
            public Material Menu { get; }
            public Material Gameplay { get; }
            public Material ActivityA { get; }
            public Material ActivityB { get; }
        }
    }
}
