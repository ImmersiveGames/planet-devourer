using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkModels.Editor
{
    internal static class FirstGameModelScaffoldGenerator
    {
        private const string FrameworkModelsRoot = "Assets/_Project/FrameworkModels";
        private static readonly List<string> Created = new List<string>();
        private static readonly List<string> Preserved = new List<string>();
        private static readonly List<string> MissingTypes = new List<string>();
        private static readonly List<string> Errors = new List<string>();

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/Scaffolds/Create Missing M02-M16", false, 90)]
        private static void CreateAllMissingScaffolds()
        {
            SceneSetup[] previousSetup = EditorSceneManager.GetSceneManagerSetup();
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Created.Clear(); Preserved.Clear(); MissingTypes.Clear(); Errors.Clear();
            try
            {
                foreach (ModelSpec model in Models) CreateModel(model);
            }
            finally
            {
                RestorePreviousSceneSetup(previousSetup);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            string summary = $"FIRSTGAME M02-M16 scaffold finished. created={Created.Count}, preserved={Preserved.Count}, missingTypes={MissingTypes.Count}, errors={Errors.Count}.";
            Debug.Log(summary);
            foreach (string missing in MissingTypes) Debug.LogWarning(missing);
            foreach (string error in Errors) Debug.LogError(error);
            EditorUtility.DisplayDialog("FIRSTGAME Scaffolds", summary + "\n\nNo cross-asset references, framework components, bootstrap, Build Profile or ProjectSettings values were assigned.\n\nSee each model README before configuration.", "OK");
        }

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/Scaffolds/Select FrameworkModels", false, 91)]
        private static void SelectFrameworkModels()
        {
            UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(FrameworkModelsRoot);
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }

        private static void CreateModel(ModelSpec model)
        {
            string root = FrameworkModelsRoot + "/" + model.RootFolder;
            EnsureFolder(root);
            EnsureFolder(root + "/Application");
            EnsureFolder(root + "/Routes");
            EnsureFolder(root + "/Activities");
            EnsureFolder(root + "/Profiles");
            EnsureFolder(root + "/Recipes");
            EnsureFolder(root + "/Scenes");
            EnsureFolder(root + "/Prefabs");
            EnsureFolder(root + "/Materials");

            MaterialSet materials = CreateMaterials(model, root + "/Materials");
            foreach (AssetSpec asset in model.Assets) CreateAuthoringAsset(model, root, asset);
            foreach (string prefab in model.Prefabs) CreatePrefabIfMissing(root + "/Prefabs/" + prefab + ".prefab", prefab, materials.Accent);
            foreach (string scene in model.Scenes) CreateSceneIfMissing(root + "/Scenes/" + scene + ".unity", model, scene, materials);
        }

        private static void CreateAuthoringAsset(ModelSpec model, string root, AssetSpec spec)
        {
            string path = root + "/" + spec.Folder + "/" + spec.Name + ".asset";
            if (AssetDatabase.LoadMainAssetAtPath(path) != null) { Preserved.Add(path); return; }
            Type type = ResolveScriptableObjectType(spec.TypeCandidates);
            if (type == null)
            {
                MissingTypes.Add($"{model.Code}: skipped optional asset '{spec.Name}' because none of these types are available: {string.Join(", ", spec.TypeCandidates)}");
                return;
            }
            try
            {
                ScriptableObject asset = ScriptableObject.CreateInstance(type);
                asset.name = spec.Name;
                AssetDatabase.CreateAsset(asset, path);
                SerializedObject serialized = new SerializedObject(asset);
                SetFirstString(serialized, spec.DisplayName, "applicationName", "routeName", "activityName", "displayName", "profileName", "actorName", "slotName");
                SetFirstString(serialized, spec.Description, "description", "authoringDescription", "notes");
                if (spec.Kind == "Route") SetFirstString(serialized, Guid.NewGuid().ToString("N"), "routeId");
                if (spec.Kind == "Activity") SetFirstString(serialized, Guid.NewGuid().ToString("N"), "activityId");
                if (spec.Kind == "PlayerSlot") SetFirstString(serialized, Guid.NewGuid().ToString("N"), "playerSlotId", "slotId", "profileId");
                if (spec.Kind == "Actor") SetFirstString(serialized, Guid.NewGuid().ToString("N"), "actorId", "actorProfileId", "profileId");
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);
                Created.Add(path);
            }
            catch (Exception exception)
            {
                Errors.Add($"{model.Code}: failed to create {path}: {exception.Message}");
            }
        }

        private static Type ResolveScriptableObjectType(string[] candidates)
        {
            TypeCache.TypeCollection types =
                TypeCache.GetTypesDerivedFrom<ScriptableObject>();

            for (int candidateIndex = 0; candidateIndex < candidates.Length; candidateIndex++)
            {
                for (int typeIndex = 0; typeIndex < types.Count; typeIndex++)
                {
                    Type type = types[typeIndex];
                    if (!type.IsAbstract &&
                        string.Equals(
                            type.FullName,
                            candidates[candidateIndex],
                            StringComparison.Ordinal))
                    {
                        return type;
                    }
                }
            }

            for (int candidateIndex = 0; candidateIndex < candidates.Length; candidateIndex++)
            {
                for (int typeIndex = 0; typeIndex < types.Count; typeIndex++)
                {
                    Type type = types[typeIndex];
                    if (!type.IsAbstract &&
                        string.Equals(
                            type.Name,
                            candidates[candidateIndex],
                            StringComparison.Ordinal))
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        private static void SetFirstString(SerializedObject serialized, string value, params string[] propertyNames)
        {
            if (string.IsNullOrEmpty(value)) return;
            for (int i = 0; i < propertyNames.Length; i++)
            {
                SerializedProperty property = serialized.FindProperty(propertyNames[i]);
                if (property != null && property.propertyType == SerializedPropertyType.String)
                {
                    property.stringValue = value;
                    return;
                }
            }
        }

        private static MaterialSet CreateMaterials(ModelSpec model, string folder)
        {
            float hue = Mathf.Repeat((model.Ordinal - 2) * 0.071f, 1f);
            Color baseColor = Color.HSVToRGB(hue, 0.48f, 0.68f);
            Color accentColor = Color.HSVToRGB(Mathf.Repeat(hue + 0.11f, 1f), 0.66f, 0.92f);
            Material baseMaterial = CreateMaterialIfMissing(folder + "/MAT_" + model.Code + "_Base.mat", baseColor);
            Material accentMaterial = CreateMaterialIfMissing(folder + "/MAT_" + model.Code + "_Accent.mat", accentColor);
            return new MaterialSet(baseMaterial, accentMaterial);
        }

        private static Material CreateMaterialIfMissing(string path, Color color)
        {
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) { Preserved.Add(path); return existing; }
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Unlit/Color");
            if (shader == null) { Errors.Add("No compatible shader found for " + path); return null; }
            Material material = new Material(shader) { color = color, name = Path.GetFileNameWithoutExtension(path) };
            AssetDatabase.CreateAsset(material, path);
            Created.Add(path);
            return material;
        }

        private static void CreatePrefabIfMissing(string path, string prefabName, Material material)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) { Preserved.Add(path); return; }
            GameObject root = new GameObject(prefabName);
            try
            {
                GameObject visual = GameObject.CreatePrimitive(PrimitiveForName(prefabName));
                visual.name = "Visual Placeholder";
                visual.transform.SetParent(root.transform, false);
                visual.transform.localPosition = Vector3.up * 0.65f;
                visual.transform.localScale = new Vector3(1.4f, 1.2f, 1.4f);
                Renderer renderer = visual.GetComponent<Renderer>();
                if (renderer != null && material != null) renderer.sharedMaterial = material;
                GameObject label = CreateWorldLabel(prefabName, new Vector3(0f, 1.8f, 0f), 0.07f);
                label.transform.SetParent(root.transform, false);
                new GameObject("Framework Components (Configure Manually)").transform.SetParent(root.transform, false);
                new GameObject("Bindings (Configure Manually)").transform.SetParent(root.transform, false);
                PrefabUtility.SaveAsPrefabAsset(root, path);
                Created.Add(path);
            }
            catch (Exception exception)
            {
                Errors.Add($"Failed to create prefab {path}: {exception.Message}");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static PrimitiveType PrimitiveForName(string name)
        {
            int value = (name.GetHashCode() & 0x7fffffff) % 3;
            return value == 0 ? PrimitiveType.Cube : value == 1 ? PrimitiveType.Sphere : PrimitiveType.Capsule;
        }

        private static void CreateSceneIfMissing(string path, ModelSpec model, string sceneName, MaterialSet materials)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null) { Preserved.Add(path); return; }
            try
            {
                Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                BuildScene(model, sceneName, materials);
                EditorSceneManager.SaveScene(scene, path);
                Created.Add(path);
            }
            catch (Exception exception)
            {
                Errors.Add($"Failed to create scene {path}: {exception.Message}");
            }
        }

        private static void BuildScene(ModelSpec model, string sceneName, MaterialSet materials)
        {
            bool additive = sceneName.EndsWith("_Add", StringComparison.Ordinal);
            GameObject root = new GameObject(sceneName + "_Root");
            GameObject authored = new GameObject("Authored Visual Content"); authored.transform.SetParent(root.transform, false);
            GameObject frameworkMount = new GameObject("Framework Mount (Configure Manually)"); frameworkMount.transform.SetParent(root.transform, false);
            GameObject uiMount = new GameObject("UI Mount (Configure Manually)"); uiMount.transform.SetParent(root.transform, false);

            if (!additive)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                Camera camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                cameraObject.transform.position = new Vector3(0f, 5.5f, -11f);
                cameraObject.transform.rotation = Quaternion.Euler(21f, 0f, 0f);
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.045f, 0.055f, 0.075f, 1f);
                GameObject lightObject = new GameObject("Directional Light");
                Light light = lightObject.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.1f;
                lightObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
                GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Route Ground"; ground.transform.SetParent(authored.transform, false); ground.transform.localScale = new Vector3(1.4f, 1f, 1.0f);
                Renderer groundRenderer = ground.GetComponent<Renderer>(); if (groundRenderer != null && materials.Base != null) groundRenderer.sharedMaterial = materials.Base;
            }

            int markerCount = additive ? 3 : 5;
            for (int i = 0; i < markerCount; i++)
            {
                GameObject marker = GameObject.CreatePrimitive(i % 2 == 0 ? PrimitiveType.Cube : PrimitiveType.Cylinder);
                marker.name = "Visual Marker " + (i + 1);
                marker.transform.SetParent(authored.transform, false);
                marker.transform.localPosition = new Vector3((i - (markerCount - 1) * 0.5f) * 1.7f, additive ? 1f : 0.75f, additive ? 0f : 1.4f);
                marker.transform.localScale = new Vector3(1f, 1f + i * 0.18f, 1f);
                Renderer markerRenderer = marker.GetComponent<Renderer>(); if (markerRenderer != null && materials.Accent != null) markerRenderer.sharedMaterial = materials.Accent;
            }
            GameObject title = CreateWorldLabel(model.Code + " — " + model.Title + "\n" + sceneName, new Vector3(0f, additive ? 3.0f : 3.8f, 1f), 0.09f);
            title.transform.SetParent(authored.transform, false);
        }

        private static GameObject CreateWorldLabel(string text, Vector3 localPosition, float characterSize)
        {
            GameObject label = new GameObject("Label");
            TextMesh textMesh = label.AddComponent<TextMesh>();
            textMesh.text = text; textMesh.anchor = TextAnchor.MiddleCenter; textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 64; textMesh.characterSize = characterSize; textMesh.color = Color.white;
            label.transform.localPosition = localPosition;
            return label;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string name = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
            Created.Add(path + "/");
        }

        private static void RestorePreviousSceneSetup(SceneSetup[] setup)
        {
            if (setup == null || setup.Length == 0)
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                return;
            }
            try { EditorSceneManager.RestoreSceneManagerSetup(setup); }
            catch (Exception exception) { Errors.Add("Could not restore previous scene setup: " + exception.Message); }
        }

        private sealed class MaterialSet
        {
            public MaterialSet(Material baseMaterial, Material accentMaterial) { Base = baseMaterial; Accent = accentMaterial; }
            public Material Base { get; private set; }
            public Material Accent { get; private set; }
        }

        private sealed class AssetSpec
        {
            public AssetSpec(string kind, string folder, string name, string displayName, string description, string[] typeCandidates)
            { Kind = kind; Folder = folder; Name = name; DisplayName = displayName; Description = description; TypeCandidates = typeCandidates; }
            public string Kind { get; private set; } public string Folder { get; private set; } public string Name { get; private set; }
            public string DisplayName { get; private set; } public string Description { get; private set; } public string[] TypeCandidates { get; private set; }
        }

        private sealed class ModelSpec
        {
            public ModelSpec(int ordinal, string code, string rootFolder, string title, AssetSpec[] assets, string[] scenes, string[] prefabs)
            { Ordinal = ordinal; Code = code; RootFolder = rootFolder; Title = title; Assets = assets; Scenes = scenes; Prefabs = prefabs; }
            public int Ordinal { get; private set; } public string Code { get; private set; } public string RootFolder { get; private set; } public string Title { get; private set; }
            public AssetSpec[] Assets { get; private set; } public string[] Scenes { get; private set; } public string[] Prefabs { get; private set; }
        }

        private static readonly ModelSpec[] Models =
        {
            new ModelSpec(
                2, "M02", "M02_LifecycleEvents", "Lifecycle Events",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M02_Lifecycle", "GA M02 Lifecycle", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M02_A", "Route M02 A", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M02_B", "Route M02 B", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M02_A", "Activity M02 A", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M02_B", "Activity M02 B", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M02_A", "ActivityContent M02 A", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M02_B", "ActivityContent M02 B", "Scaffold asset for M02 Lifecycle Events. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M02_Boot", "M02_RouteA", "M02_RouteB", "M02_ActivityA_Add", "M02_ActivityB_Add" },
                new[] { "PF_M02_SceneLifecycleObject", "PF_M02_RouteLifecycleObject", "PF_M02_ActivityLifecycleObject" }),
            new ModelSpec(
                3, "M03", "M03_ActivityReadiness", "Activity Readiness",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M03_Readiness", "GA M03 Readiness", "Scaffold asset for M03 Activity Readiness. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M03_Readiness", "Route M03 Readiness", "Scaffold asset for M03 Activity Readiness. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M03_Preparation", "Activity M03 Preparation", "Scaffold asset for M03 Activity Readiness. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M03_Preparation", "ActivityContent M03 Preparation", "Scaffold asset for M03 Activity Readiness. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M03_Boot", "M03_Route", "M03_Activity_Add" },
                new[] { "PF_M03_PreparationParticipant", "PF_M03_ReadinessDisplay", "PF_M03_PreparedContent" }),
            new ModelSpec(
                4, "M04", "M04_ContentAnchors", "Content Anchors",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M04_ContentAnchors", "GA M04 ContentAnchors", "Scaffold asset for M04 Content Anchors. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M04_ContentAnchors", "Route M04 ContentAnchors", "Scaffold asset for M04 Content Anchors. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M04_A", "Activity M04 A", "Scaffold asset for M04 Content Anchors. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M04_B", "Activity M04 B", "Scaffold asset for M04 Content Anchors. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M04_A", "ActivityContent M04 A", "Scaffold asset for M04 Content Anchors. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M04_B", "ActivityContent M04 B", "Scaffold asset for M04 Content Anchors. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M04_Boot", "M04_Route", "M04_ActivityA_Add", "M04_ActivityB_Add" },
                new[] { "PF_M04_RouteRootAnchor", "PF_M04_ActivityRootAnchor", "PF_M04_ActivitySlotAnchor", "PF_M04_LocalPointAnchor", "PF_M04_AnchorStatusDisplay" }),
            new ModelSpec(
                5, "M05", "M05_AnchorMaterialization", "Anchor Materialization",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M05_Materialization", "GA M05 Materialization", "Scaffold asset for M05 Anchor Materialization. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M05_Materialization", "Route M05 Materialization", "Scaffold asset for M05 Anchor Materialization. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M05_Materialization", "Activity M05 Materialization", "Scaffold asset for M05 Anchor Materialization. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M05_Materialization", "ActivityContent M05 Materialization", "Scaffold asset for M05 Anchor Materialization. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M05_Boot", "M05_Route", "M05_Activity_Add" },
                new[] { "PF_M05_Anchor", "PF_M05_MaterializedContent", "PF_M05_MaterializationBridge" }),
            new ModelSpec(
                6, "M06", "M06_SceneProvidedPlayer", "Scene-Provided Player",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M06_ScenePlayer", "GA M06 ScenePlayer", "Scaffold asset for M06 Scene-Provided Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M06_ScenePlayer", "Route M06 ScenePlayer", "Scaffold asset for M06 Scene-Provided Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M06_ScenePlayer", "Activity M06 ScenePlayer", "Scaffold asset for M06 Scene-Provided Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M06_ScenePlayer", "ActivityContent M06 ScenePlayer", "Scaffold asset for M06 Scene-Provided Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("PlayerSlot", "Profiles", "PlayerSlot_M06_Player1", "PlayerSlot M06 Player1", "Scaffold asset for M06 Scene-Provided Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.PlayerSlots.PlayerSlotProfile", "PlayerSlotProfile" }),
                    new AssetSpec("Actor", "Profiles", "Actor_M06_Default", "Actor M06 Default", "Scaffold asset for M06 Scene-Provided Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.PlayerActors.ActorProfile", "Immersive.Framework.Actor.ActorProfile", "ActorProfile" }),
                },
                new[] { "M06_Boot", "M06_Route", "M06_Activity_Add" },
                new[] { "PF_M06_SceneProvidedPlayer", "PF_M06_PlayerActor", "PF_M06_PlayerStatusDisplay" }),
            new ModelSpec(
                7, "M07", "M07_ManagerProvisionedPlayer", "Manager-Provisioned Player",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M07_ProvisionedPlayer", "GA M07 ProvisionedPlayer", "Scaffold asset for M07 Manager-Provisioned Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M07_ProvisionedPlayer", "Route M07 ProvisionedPlayer", "Scaffold asset for M07 Manager-Provisioned Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M07_ProvisionedPlayer", "Activity M07 ProvisionedPlayer", "Scaffold asset for M07 Manager-Provisioned Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M07_ProvisionedPlayer", "ActivityContent M07 ProvisionedPlayer", "Scaffold asset for M07 Manager-Provisioned Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("PlayerSlot", "Profiles", "PlayerSlot_M07_Player1", "PlayerSlot M07 Player1", "Scaffold asset for M07 Manager-Provisioned Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.PlayerSlots.PlayerSlotProfile", "PlayerSlotProfile" }),
                    new AssetSpec("Actor", "Profiles", "Actor_M07_Default", "Actor M07 Default", "Scaffold asset for M07 Manager-Provisioned Player. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.PlayerActors.ActorProfile", "Immersive.Framework.Actor.ActorProfile", "ActorProfile" }),
                },
                new[] { "M07_Boot", "M07_Route", "M07_Activity_Add" },
                new[] { "PF_M07_PlayerInputManagerHost", "PF_M07_RuntimePlayer", "PF_M07_PlayerActor", "PF_M07_JoinControl", "PF_M07_PlayerStatusDisplay" }),
            new ModelSpec(
                8, "M08", "M08_ParticipationPolicies", "Participation Policies",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M08_Participation", "GA M08 Participation", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M08_Participation", "Route M08 Participation", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M08_NoSlots", "Activity M08 NoSlots", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M08_JoinedSlots", "Activity M08 JoinedSlots", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M08_SelectedActors", "Activity M08 SelectedActors", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M08_LogicalPrepared", "Activity M08 LogicalPrepared", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M08_GameplayReady", "Activity M08 GameplayReady", "Scaffold asset for M08 Participation Policies. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                },
                new[] { "M08_Boot", "M08_Route" },
                new[] { "PF_M08_ParticipationPlayer", "PF_M08_ParticipationStatus", "PF_M08_ActivitySelector" }),
            new ModelSpec(
                9, "M09", "M09_InputGate", "Input Gate",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M09_InputGate", "GA M09 InputGate", "Scaffold asset for M09 Input Gate. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M09_InputGate", "Route M09 InputGate", "Scaffold asset for M09 Input Gate. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M09_InputGate", "Activity M09 InputGate", "Scaffold asset for M09 Input Gate. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M09_InputGate", "ActivityContent M09 InputGate", "Scaffold asset for M09 Input Gate. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M09_Boot", "M09_Route", "M09_Activity_Add" },
                new[] { "PF_M09_Player", "PF_M09_InteractionTarget", "PF_M09_GateControl", "PF_M09_InputStatus" }),
            new ModelSpec(
                10, "M10", "M10_PlayerCamera", "Player Camera",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M10_PlayerCamera", "GA M10 PlayerCamera", "Scaffold asset for M10 Player Camera. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M10_PlayerCamera", "Route M10 PlayerCamera", "Scaffold asset for M10 Player Camera. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M10_PlayerCamera", "Activity M10 PlayerCamera", "Scaffold asset for M10 Player Camera. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M10_PlayerCamera", "ActivityContent M10 PlayerCamera", "Scaffold asset for M10 Player Camera. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("CameraRig", "Recipes", "CameraRig_M10_Player", "CameraRig M10 Player", "Scaffold asset for M10 Player Camera. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Camera.CameraRigRecipe", "Immersive.Framework.Camera.CameraRigRecipeAsset", "CameraRigRecipe", "CameraRigRecipeAsset" }),
                },
                new[] { "M10_Boot", "M10_Route", "M10_Activity_Add" },
                new[] { "PF_M10_PersistentCameraOutput", "PF_M10_Player", "PF_M10_PlayerCameraRig", "PF_M10_CameraStatus" }),
            new ModelSpec(
                11, "M11", "M11_ObjectReset", "Object Reset",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M11_Reset", "GA M11 Reset", "Scaffold asset for M11 Object Reset. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M11_Reset", "Route M11 Reset", "Scaffold asset for M11 Object Reset. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M11_Reset", "Activity M11 Reset", "Scaffold asset for M11 Object Reset. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M11_Reset", "ActivityContent M11 Reset", "Scaffold asset for M11 Object Reset. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M11_Boot", "M11_Route", "M11_Activity_Add" },
                new[] { "PF_M11_TransformResettable", "PF_M11_StateResettable", "PF_M11_RuntimeSpawnedObject", "PF_M11_ResetControls", "PF_M11_ResetStatus" }),
            new ModelSpec(
                12, "M12", "M12_ActivityRestart", "Activity Restart",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M12_ActivityRestart", "GA M12 ActivityRestart", "Scaffold asset for M12 Activity Restart. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M12_ActivityRestart", "Route M12 ActivityRestart", "Scaffold asset for M12 Activity Restart. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M12_Gameplay", "Activity M12 Gameplay", "Scaffold asset for M12 Activity Restart. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M12_Gameplay", "ActivityContent M12 Gameplay", "Scaffold asset for M12 Activity Restart. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M12_Boot", "M12_Route", "M12_Activity_Add" },
                new[] { "PF_M12_RestartableObjective", "PF_M12_RestartableWorld", "PF_M12_ActivityRestartControl", "PF_M12_RestartStatus" }),
            new ModelSpec(
                13, "M13", "M13_Pause", "Pause",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M13_Pause", "GA M13 Pause", "Scaffold asset for M13 Pause. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M13_Pause", "Route M13 Pause", "Scaffold asset for M13 Pause. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M13_ApplicationPause", "Activity M13 ApplicationPause", "Scaffold asset for M13 Pause. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M13_PlayerPause", "Activity M13 PlayerPause", "Scaffold asset for M13 Pause. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M13_ApplicationPause", "ActivityContent M13 ApplicationPause", "Scaffold asset for M13 Pause. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M13_PlayerPause", "ActivityContent M13 PlayerPause", "Scaffold asset for M13 Pause. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M13_Boot", "M13_Route", "M13_ApplicationPause_Add", "M13_PlayerPause_Add" },
                new[] { "PF_M13_PauseSurface", "PF_M13_PauseControls", "PF_M13_Player", "PF_M13_PausePlayerBinding", "PF_M13_PauseStatus" }),
            new ModelSpec(
                14, "M14", "M14_TransitionLoading", "Transition and Loading",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M14_TransitionLoading", "GA M14 TransitionLoading", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M14_Menu", "Route M14 Menu", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M14_Destination", "Route M14 Destination", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M14_Light", "Activity M14 Light", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M14_Loaded", "Activity M14 Loaded", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M14_Light", "ActivityContent M14 Light", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M14_Loaded", "ActivityContent M14 Loaded", "Scaffold asset for M14 Transition and Loading. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M14_Boot", "M14_Menu", "M14_Destination", "M14_Light_Add", "M14_Loaded_Add" },
                new[] { "PF_M14_TransitionSurface", "PF_M14_LoadingSurface", "PF_M14_Navigation", "PF_M14_TransitionStatus" }),
            new ModelSpec(
                15, "M15", "M15_CameraOverrides", "Camera Overrides",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M15_CameraOverrides", "GA M15 CameraOverrides", "Scaffold asset for M15 Camera Overrides. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M15_CameraOverrides", "Route M15 CameraOverrides", "Scaffold asset for M15 Camera Overrides. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M15_PlayerCamera", "Activity M15 PlayerCamera", "Scaffold asset for M15 Camera Overrides. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M15_Cinematic", "Activity M15 Cinematic", "Scaffold asset for M15 Camera Overrides. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M15_PlayerCamera", "ActivityContent M15 PlayerCamera", "Scaffold asset for M15 Camera Overrides. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M15_Cinematic", "ActivityContent M15 Cinematic", "Scaffold asset for M15 Camera Overrides. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M15_Boot", "M15_Route", "M15_Player_Add", "M15_Cinematic_Add" },
                new[] { "PF_M15_PlayerCamera", "PF_M15_ActivityCameraOverride", "PF_M15_CameraStatus" }),
            new ModelSpec(
                16, "M16", "M16_Bgm", "BGM",
                new[]
                {
                    new AssetSpec("GameApplication", "Application", "GA_M16_Bgm", "GA M16 Bgm", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.GameApplicationAsset", "GameApplicationAsset" }),
                    new AssetSpec("Route", "Routes", "Route_M16_Bgm", "Route M16 Bgm", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.RouteAsset", "RouteAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M16_OwnMusic", "Activity M16 OwnMusic", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M16_UseRoute", "Activity M16 UseRoute", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("Activity", "Activities", "Activity_M16_Silence", "Activity M16 Silence", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityAsset", "ActivityAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M16_OwnMusic", "ActivityContent M16 OwnMusic", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M16_UseRoute", "ActivityContent M16 UseRoute", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                    new AssetSpec("ActivityContent", "Profiles", "ActivityContent_M16_Silence", "ActivityContent M16 Silence", "Scaffold asset for M16 BGM. Configure references and feature-specific contracts manually in the Unity Inspector.", new[] { "Immersive.Framework.Authoring.ActivityContentProfileAsset", "ActivityContentProfileAsset" }),
                },
                new[] { "M16_Boot", "M16_Route", "M16_OwnMusic_Add", "M16_UseRoute_Add", "M16_Silence_Add" },
                new[] { "PF_M16_BgmDirector", "PF_M16_RouteBgmBinding", "PF_M16_ActivityBgmBinding", "PF_M16_BgmStatus" }),
        };
    }
}
