using System;
using System.Collections.Generic;
using FirstGame.FrameworkModels.ActivityReadiness;
using Immersive.Framework.ActivityFlow;
using Immersive.Framework.RuntimeContent;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkModels.ActivityReadiness.Editor
{
    internal sealed class M03ActivityReadinessCreateTool : EditorWindow
    {
        private const string LogPrefix = "[FIRSTGAME_M03_CREATE_TOOL]";

        private const string ModelRoot = "Assets/_Project/FrameworkModels/M03_ActivityReadiness";
        private const string PreparationPrefabPath = ModelRoot + "/Prefabs/PF_M03_PreparationParticipant.prefab";
        private const string DisplayPrefabPath = ModelRoot + "/Prefabs/PF_M03_ReadinessDisplay.prefab";
        private const string PreparedContentPrefabPath = ModelRoot + "/Prefabs/PF_M03_PreparedContent.prefab";
        private const string ActivityScenePath = ModelRoot + "/Scenes/M03_Activity_Add.unity";
        private const string MaterialsFolder = ModelRoot + "/Materials";

        private const string SettingsPath = "Assets/_Project/Settings/ImmersiveFramework/Resources/ImmersiveFrameworkSettings.asset";
        private const string GameApplicationPath = ModelRoot + "/Application/GA_M03_Readiness.asset";

        private const string FrameworkMountName = "Framework Components (Configure Manually)";
        private const string BindingsMountName = "Bindings (Configure Manually)";
        private const string VisualPlaceholderName = "Visual Placeholder";

        private static readonly Color WaitingColor = new Color(1f, 0.52f, 0.08f, 1f);
        private static readonly Color ReadyColor = new Color(0.12f, 0.85f, 0.32f, 1f);
        private static readonly Color PreparedColor = new Color(0.12f, 0.72f, 1f, 1f);

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Create or Configure", priority = 2300)]
        private static void OpenWindow()
        {
            GetWindow<M03ActivityReadinessCreateTool>("M03 Readiness");
        }

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Configure Existing Prefabs", priority = 2301)]
        private static void ConfigureExistingPrefabsMenu()
        {
            Run("ConfigureExistingPrefabs", ConfigureExistingPrefabs);
        }

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Compose Existing Activity Scene", priority = 2302)]
        private static void ComposeActivitySceneMenu()
        {
            Run("ComposeActivityScene", ComposeActivityScene);
        }

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Set Active Game Application", priority = 2303)]
        private static void SetActiveApplicationMenu()
        {
            Run("SetActiveGameApplication", SetActiveGameApplication);
        }

        [MenuItem("Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Validate", priority = 2304)]
        private static void ValidateMenu()
        {
            Run("Validate", ValidateCurrentModel);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("M03 Activity Readiness", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Configures the existing M03 scaffold. It does not recreate the Game Application, Route, Activity, profile, scenes, or prefabs.",
                MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("1. Configure Existing Prefabs", GUILayout.Height(30f)))
            {
                Run("ConfigureExistingPrefabs", ConfigureExistingPrefabs);
            }

            if (GUILayout.Button("2. Compose Existing Activity Scene", GUILayout.Height(30f)))
            {
                Run("ComposeActivityScene", ComposeActivityScene);
            }

            if (GUILayout.Button("Set M03 as Active Game Application", GUILayout.Height(24f)))
            {
                Run("SetActiveGameApplication", SetActiveGameApplication);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Validate M03", GUILayout.Height(24f)))
            {
                Run("Validate", ValidateCurrentModel);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "All operations are idempotent. Existing components, references, transforms, and scene instances are preserved unless they are the explicit M03 fields managed by this tool.",
                MessageType.None);
        }

        private static void ConfigureExistingPrefabs()
        {
            RequireAsset<GameObject>(PreparationPrefabPath);
            RequireAsset<GameObject>(DisplayPrefabPath);
            RequireAsset<GameObject>(PreparedContentPrefabPath);

            EnsureFolder(MaterialsFolder);
            Material waitingMaterial = CreateOrLoadMaterial(MaterialsFolder + "/M_M03_Waiting.mat", WaitingColor);
            Material readyMaterial = CreateOrLoadMaterial(MaterialsFolder + "/M_M03_Ready.mat", ReadyColor);
            Material preparedMaterial = CreateOrLoadMaterial(MaterialsFolder + "/M_M03_Prepared.mat", PreparedColor);

            ConfigurePreparationPrefab(waitingMaterial);
            ConfigureDisplayPrefab(waitingMaterial, readyMaterial);
            ConfigurePreparedContentPrefab(preparedMaterial);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"{LogPrefix} operation='ConfigureExistingPrefabs' status='Succeeded'.");
        }

        private static void ConfigurePreparationPrefab(Material waitingMaterial)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PreparationPrefabPath);
            try
            {
                Transform frameworkMount = RequireChild(root.transform, FrameworkMountName);
                Transform bindingsMount = RequireChild(root.transform, BindingsMountName);
                Transform visual = RequireChild(root.transform, VisualPlaceholderName);

                ActivityReadinessParticipant participant = GetOrAddComponent<ActivityReadinessParticipant>(frameworkMount.gameObject);
                M03PreparationSequence sequence = GetOrAddComponent<M03PreparationSequence>(bindingsMount.gameObject);

                SetString(participant, "participantId", "m03.preparation");
                SetInt(participant, "requiredness", (int)ActivityContentExecutionRequiredness.Required);
                SetInt(participant, "order", 0);

                SetObjectReference(sequence, "readinessParticipant", participant);
                SetObjectReference(sequence, "preparationVisual", visual);
                SetFloat(sequence, "preparationDuration", 1.5f, preservePositiveExistingValue: true);
                SetVector3(sequence, "preparedLocalPosition", new Vector3(0f, 1.5f, 0f), preserveNonZeroExistingValue: true);

                EnsurePersistentListener(
                    participant.PreparationStarted,
                    sequence,
                    nameof(M03PreparationSequence.BeginPreparation),
                    sequence.BeginPreparation);
                EnsurePersistentListener(
                    participant.PreparationReleased,
                    sequence,
                    nameof(M03PreparationSequence.ReleasePreparation),
                    sequence.ReleasePreparation);

                AssignMaterial(visual.gameObject, waitingMaterial);
                PrefabUtility.SaveAsPrefabAsset(root, PreparationPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ConfigureDisplayPrefab(Material waitingMaterial, Material readyMaterial)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(DisplayPrefabPath);
            try
            {
                Transform frameworkMount = RequireChild(root.transform, FrameworkMountName);
                Transform bindingsMount = RequireChild(root.transform, BindingsMountName);
                Transform waitingVisual = RequireChild(root.transform, VisualPlaceholderName);
                TextMesh statusLabel = RequireChild(root.transform, "Label").GetComponent<TextMesh>();
                if (statusLabel == null)
                {
                    throw new InvalidOperationException("PF_M03_ReadinessDisplay/Label must contain a TextMesh.");
                }

                ActivityReadinessEvents readinessEvents = GetOrAddComponent<ActivityReadinessEvents>(frameworkMount.gameObject);
                M03ReadinessPresenter presenter = GetOrAddComponent<M03ReadinessPresenter>(bindingsMount.gameObject);

                TextMesh detailLabel = EnsureDetailLabel(root.transform, statusLabel);
                GameObject readyVisual = EnsurePrimitiveChild(root.transform, "Ready Visual", PrimitiveType.Cube);
                readyVisual.transform.localPosition = waitingVisual.localPosition;
                readyVisual.transform.localRotation = waitingVisual.localRotation;
                readyVisual.transform.localScale = waitingVisual.localScale;
                readyVisual.SetActive(false);

                waitingVisual.gameObject.SetActive(true);
                AssignMaterial(waitingVisual.gameObject, waitingMaterial);
                AssignMaterial(readyVisual, readyMaterial);

                statusLabel.text = "Waiting";
                detailLabel.text = "Waiting for Activity preparation";

                SetObjectReference(presenter, "statusLabel", statusLabel);
                SetObjectReference(presenter, "detailLabel", detailLabel);
                SetObjectReference(presenter, "waitingVisual", waitingVisual.gameObject);
                SetObjectReference(presenter, "readyVisual", readyVisual);
                // preparedContent is intentionally assigned on the scene instance.

                EnsurePersistentListener(
                    readinessEvents.Preparing,
                    presenter,
                    nameof(M03ReadinessPresenter.ShowPreparing),
                    presenter.ShowPreparing);
                EnsurePersistentListener(
                    readinessEvents.Ready,
                    presenter,
                    nameof(M03ReadinessPresenter.ShowReady),
                    presenter.ShowReady);
                EnsurePersistentListener(
                    readinessEvents.NotReady,
                    presenter,
                    nameof(M03ReadinessPresenter.ShowNotReady),
                    presenter.ShowNotReady);

                PrefabUtility.SaveAsPrefabAsset(root, DisplayPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ConfigurePreparedContentPrefab(Material preparedMaterial)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PreparedContentPrefabPath);
            try
            {
                Transform visual = RequireChild(root.transform, VisualPlaceholderName);
                AssignMaterial(visual.gameObject, preparedMaterial);

                Transform labelTransform = FindChild(root.transform, "Label");
                TextMesh label = labelTransform != null ? labelTransform.GetComponent<TextMesh>() : null;
                if (label != null)
                {
                    label.text = "Prepared Content";
                }

                PrefabUtility.SaveAsPrefabAsset(root, PreparedContentPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ComposeActivityScene()
        {
            if (!System.IO.File.Exists(ActivityScenePath))
            {
                throw new InvalidOperationException($"Scene not found: {ActivityScenePath}");
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning($"{LogPrefix} operation='ComposeActivityScene' status='CancelledByUser'.");
                return;
            }

            GameObject preparationPrefab = RequireAsset<GameObject>(PreparationPrefabPath);
            GameObject displayPrefab = RequireAsset<GameObject>(DisplayPrefabPath);
            GameObject preparedContentPrefab = RequireAsset<GameObject>(PreparedContentPrefabPath);

            Scene scene = EditorSceneManager.OpenScene(ActivityScenePath, OpenSceneMode.Single);
            Transform sceneRoot = RequireSceneTransform(scene, "M03_Activity_Add_Root");
            Transform authoredVisualContent = RequireChild(sceneRoot, "Authored Visual Content");
            Transform uiMount = RequireChild(sceneRoot, "UI Mount (Configure Manually)");

            GameObject preparationInstance = EnsurePrefabInstance(
                scene,
                preparationPrefab,
                "PF_M03_PreparationParticipant",
                authoredVisualContent,
                new Vector3(-2.5f, 0f, 0f));
            GameObject preparedContentInstance = EnsurePrefabInstance(
                scene,
                preparedContentPrefab,
                "PF_M03_PreparedContent",
                authoredVisualContent,
                new Vector3(2.5f, 0f, 0f));
            GameObject displayInstance = EnsurePrefabInstance(
                scene,
                displayPrefab,
                "PF_M03_ReadinessDisplay",
                uiMount,
                new Vector3(0f, 0f, 2.5f));

            ActivityReadinessParticipant participant = preparationInstance.GetComponentInChildren<ActivityReadinessParticipant>(true);
            M03ReadinessPresenter presenter = displayInstance.GetComponentInChildren<M03ReadinessPresenter>(true);
            if (participant == null || presenter == null)
            {
                throw new InvalidOperationException(
                    "The configured scene instances must contain ActivityReadinessParticipant and M03ReadinessPresenter.");
            }

            SetObjectReference(presenter, "preparedContent", preparedContentInstance);
            EnsurePersistentListener(
                participant.PreparationReleased,
                presenter,
                nameof(M03ReadinessPresenter.ResetPresentation),
                presenter.ResetPresentation);

            EditorUtility.SetDirty(participant);
            EditorUtility.SetDirty(presenter);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log(
                $"{LogPrefix} operation='ComposeActivityScene' status='Succeeded' " +
                "preparation='1' display='1' preparedContent='1'.");
        }

        private static void SetActiveGameApplication()
        {
            UnityEngine.Object settings = AssetDatabase.LoadMainAssetAtPath(SettingsPath);
            UnityEngine.Object gameApplication = AssetDatabase.LoadMainAssetAtPath(GameApplicationPath);
            if (settings == null || gameApplication == null)
            {
                throw new InvalidOperationException(
                    $"Settings or M03 Game Application is missing. settings='{SettingsPath}' application='{GameApplicationPath}'.");
            }

            SerializedObject serializedSettings = new SerializedObject(settings);
            SerializedProperty activeGameApplication = serializedSettings.FindProperty("activeGameApplication");
            if (activeGameApplication == null)
            {
                throw new InvalidOperationException("ImmersiveFrameworkSettings.activeGameApplication was not found.");
            }

            activeGameApplication.objectReferenceValue = gameApplication;
            serializedSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            Debug.Log($"{LogPrefix} operation='SetActiveGameApplication' status='Succeeded' application='GA_M03_Readiness'.");
        }

        private static void ValidateCurrentModel()
        {
            List<string> issues = new List<string>();

            ValidatePreparationPrefab(issues);
            ValidateDisplayPrefab(issues);
            ValidateScene(issues);
            ValidateActiveApplication(issues);

            if (issues.Count == 0)
            {
                Debug.Log(
                    $"{LogPrefix} operation='Validate' status='Passed' " +
                    "preparationPrefab='Configured' displayPrefab='Configured' activityScene='Composed' activeApplication='M03'.");
                return;
            }

            Debug.LogError(
                $"{LogPrefix} operation='Validate' status='Failed' issues='{issues.Count}'\n- " +
                string.Join("\n- ", issues));
        }

        private static void ValidatePreparationPrefab(List<string> issues)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PreparationPrefabPath);
            if (prefab == null)
            {
                issues.Add("Preparation prefab is missing.");
                return;
            }

            ActivityReadinessParticipant participant = prefab.GetComponentInChildren<ActivityReadinessParticipant>(true);
            M03PreparationSequence sequence = prefab.GetComponentInChildren<M03PreparationSequence>(true);
            if (participant == null)
            {
                issues.Add("Preparation prefab has no ActivityReadinessParticipant.");
            }
            if (sequence == null)
            {
                issues.Add("Preparation prefab has no M03PreparationSequence.");
            }
            if (participant != null && string.IsNullOrWhiteSpace(participant.ParticipantId))
            {
                issues.Add("Preparation participant has no Participant Id.");
            }
            if (participant != null && participant.Requiredness != ActivityContentExecutionRequiredness.Required)
            {
                issues.Add("Preparation participant is not Required.");
            }
            if (participant != null && sequence != null)
            {
                if (!HasPersistentListener(participant.PreparationStarted, sequence, nameof(M03PreparationSequence.BeginPreparation)))
                {
                    issues.Add("PreparationStarted is not wired to BeginPreparation.");
                }
                if (!HasPersistentListener(participant.PreparationReleased, sequence, nameof(M03PreparationSequence.ReleasePreparation)))
                {
                    issues.Add("PreparationReleased is not wired to ReleasePreparation.");
                }
            }
        }

        private static void ValidateDisplayPrefab(List<string> issues)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(DisplayPrefabPath);
            if (prefab == null)
            {
                issues.Add("Readiness display prefab is missing.");
                return;
            }

            ActivityReadinessEvents readinessEvents = prefab.GetComponentInChildren<ActivityReadinessEvents>(true);
            M03ReadinessPresenter presenter = prefab.GetComponentInChildren<M03ReadinessPresenter>(true);
            if (readinessEvents == null)
            {
                issues.Add("Readiness display prefab has no ActivityReadinessEvents.");
            }
            if (presenter == null)
            {
                issues.Add("Readiness display prefab has no M03ReadinessPresenter.");
            }
            if (readinessEvents != null && presenter != null)
            {
                if (!HasPersistentListener(readinessEvents.Preparing, presenter, nameof(M03ReadinessPresenter.ShowPreparing)))
                {
                    issues.Add("Preparing is not wired to ShowPreparing.");
                }
                if (!HasPersistentListener(readinessEvents.Ready, presenter, nameof(M03ReadinessPresenter.ShowReady)))
                {
                    issues.Add("Ready is not wired to ShowReady.");
                }
                if (!HasPersistentListener(readinessEvents.NotReady, presenter, nameof(M03ReadinessPresenter.ShowNotReady)))
                {
                    issues.Add("NotReady is not wired to ShowNotReady.");
                }

                SerializedObject serializedPresenter = new SerializedObject(presenter);
                ValidateObjectReference(serializedPresenter, "statusLabel", issues);
                ValidateObjectReference(serializedPresenter, "detailLabel", issues);
                ValidateObjectReference(serializedPresenter, "waitingVisual", issues);
                ValidateObjectReference(serializedPresenter, "readyVisual", issues);
            }
        }

        private static void ValidateScene(List<string> issues)
        {
            Scene scene = SceneManager.GetSceneByPath(ActivityScenePath);
            bool openedTemporarily = !scene.IsValid() || !scene.isLoaded;
            if (openedTemporarily)
            {
                scene = EditorSceneManager.OpenScene(ActivityScenePath, OpenSceneMode.Additive);
            }

            try
            {
                GameObject preparation = FindSceneObject(scene, "PF_M03_PreparationParticipant");
                GameObject display = FindSceneObject(scene, "PF_M03_ReadinessDisplay");
                GameObject prepared = FindSceneObject(scene, "PF_M03_PreparedContent");
                if (preparation == null) issues.Add("Activity scene has no preparation prefab instance.");
                if (display == null) issues.Add("Activity scene has no readiness display prefab instance.");
                if (prepared == null) issues.Add("Activity scene has no prepared content prefab instance.");

                if (preparation != null && display != null && prepared != null)
                {
                    ActivityReadinessParticipant participant = preparation.GetComponentInChildren<ActivityReadinessParticipant>(true);
                    M03ReadinessPresenter presenter = display.GetComponentInChildren<M03ReadinessPresenter>(true);
                    if (participant == null || presenter == null)
                    {
                        issues.Add("Activity scene instances do not expose the expected participant/presenter components.");
                    }
                    else
                    {
                        SerializedObject serializedPresenter = new SerializedObject(presenter);
                        SerializedProperty preparedContent = serializedPresenter.FindProperty("preparedContent");
                        if (preparedContent == null || preparedContent.objectReferenceValue != prepared)
                        {
                            issues.Add("Scene presenter is not bound to PF_M03_PreparedContent.");
                        }
                        if (!HasPersistentListener(participant.PreparationReleased, presenter, nameof(M03ReadinessPresenter.ResetPresentation)))
                        {
                            issues.Add("Scene participant release is not wired to ResetPresentation.");
                        }
                    }
                }
            }
            finally
            {
                if (openedTemporarily && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void ValidateActiveApplication(List<string> issues)
        {
            UnityEngine.Object settings = AssetDatabase.LoadMainAssetAtPath(SettingsPath);
            UnityEngine.Object gameApplication = AssetDatabase.LoadMainAssetAtPath(GameApplicationPath);
            if (settings == null || gameApplication == null)
            {
                issues.Add("Settings or M03 Game Application is missing.");
                return;
            }

            SerializedObject serializedSettings = new SerializedObject(settings);
            SerializedProperty activeGameApplication = serializedSettings.FindProperty("activeGameApplication");
            if (activeGameApplication == null || activeGameApplication.objectReferenceValue != gameApplication)
            {
                issues.Add("M03 is not the active Game Application.");
            }
        }

        private static TextMesh EnsureDetailLabel(Transform root, TextMesh source)
        {
            Transform existing = FindChild(root, "Detail Label");
            GameObject gameObject;
            if (existing == null)
            {
                gameObject = new GameObject("Detail Label");
                MoveToScene(gameObject, root.gameObject.scene);
                gameObject.transform.SetParent(root, false);
                gameObject.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            }
            else
            {
                gameObject = existing.gameObject;
            }

            TextMesh detail = GetOrAddComponent<TextMesh>(gameObject);
            detail.anchor = TextAnchor.MiddleCenter;
            detail.alignment = TextAlignment.Center;
            detail.characterSize = source.characterSize * 0.72f;
            detail.fontSize = source.fontSize;
            detail.fontStyle = source.fontStyle;
            detail.color = source.color;
            detail.text = "Waiting for Activity preparation";
            return detail;
        }

        private static GameObject EnsurePrimitiveChild(Transform parent, string name, PrimitiveType primitiveType)
        {
            Transform existing = FindChild(parent, name);
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject created = GameObject.CreatePrimitive(primitiveType);
            created.name = name;
            MoveToScene(created, parent.gameObject.scene);
            created.transform.SetParent(parent, false);

            Collider collider = created.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }

            return created;
        }

        private static GameObject EnsurePrefabInstance(
            Scene scene,
            GameObject prefab,
            string expectedName,
            Transform parent,
            Vector3 localPosition)
        {
            GameObject existing = FindSceneObject(scene, expectedName);
            if (existing != null)
            {
                return existing;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (instance == null)
            {
                throw new InvalidOperationException($"Could not instantiate prefab '{prefab.name}'.");
            }

            instance.name = expectedName;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = localPosition;
            return instance;
        }

        private static Material CreateOrLoadMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                if (shader == null)
                {
                    throw new InvalidOperationException("No supported Lit shader was found.");
                }

                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void AssignMaterial(GameObject root, Material material)
        {
            Renderer renderer = root.GetComponent<Renderer>() ?? root.GetComponentInChildren<Renderer>(true);
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
                EditorUtility.SetDirty(renderer);
            }
        }

        private static void EnsurePersistentListener(
            UnityEvent unityEvent,
            UnityEngine.Object target,
            string methodName,
            UnityAction action)
        {
            if (HasPersistentListener(unityEvent, target, methodName))
            {
                return;
            }

            UnityEventTools.AddPersistentListener(unityEvent, action);
        }

        private static bool HasPersistentListener(UnityEvent unityEvent, UnityEngine.Object target, string methodName)
        {
            if (unityEvent == null || target == null)
            {
                return false;
            }

            for (int index = 0; index < unityEvent.GetPersistentEventCount(); index++)
            {
                if (unityEvent.GetPersistentTarget(index) == target &&
                    string.Equals(unityEvent.GetPersistentMethodName(index), methodName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ValidateObjectReference(SerializedObject serializedObject, string propertyName, List<string> issues)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null || property.objectReferenceValue == null)
            {
                issues.Add($"Presenter reference '{propertyName}' is missing.");
            }
        }

        private static void SetString(UnityEngine.Object target, string propertyName, string value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = RequireProperty(serializedObject, propertyName);
            property.stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(UnityEngine.Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = RequireProperty(serializedObject, propertyName);
            property.intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(
            UnityEngine.Object target,
            string propertyName,
            float value,
            bool preservePositiveExistingValue)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = RequireProperty(serializedObject, propertyName);
            if (!preservePositiveExistingValue || property.floatValue <= 0f)
            {
                property.floatValue = value;
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector3(
            UnityEngine.Object target,
            string propertyName,
            Vector3 value,
            bool preserveNonZeroExistingValue)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = RequireProperty(serializedObject, propertyName);
            if (!preserveNonZeroExistingValue || property.vector3Value == Vector3.zero)
            {
                property.vector3Value = value;
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = RequireProperty(serializedObject, propertyName);
            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static SerializedProperty RequireProperty(SerializedObject serializedObject, string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Serialized property '{propertyName}' was not found on '{serializedObject.targetObject.GetType().Name}'.");
            }
            return property;
        }

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static T RequireAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new InvalidOperationException($"Required asset not found: {path}");
            }
            return asset;
        }

        private static Transform RequireSceneTransform(Scene scene, string name)
        {
            GameObject gameObject = FindSceneObject(scene, name);
            if (gameObject == null)
            {
                throw new InvalidOperationException($"Scene object not found: {name}");
            }
            return gameObject.transform;
        }

        private static Transform RequireChild(Transform parent, string name)
        {
            Transform child = FindChild(parent, name);
            if (child == null)
            {
                throw new InvalidOperationException($"Child '{name}' was not found under '{parent.name}'.");
            }
            return child;
        }

        private static Transform FindChild(Transform parent, string name)
        {
            if (string.Equals(parent.name, name, StringComparison.Ordinal))
            {
                return parent;
            }

            for (int index = 0; index < parent.childCount; index++)
            {
                Transform found = FindChild(parent.GetChild(index), name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static GameObject FindSceneObject(Scene scene, string name)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform found = FindChild(root.transform, name);
                if (found != null)
                {
                    return found.gameObject;
                }
            }

            return null;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            string folderName = System.IO.Path.GetFileName(folderPath);
            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(folderName))
            {
                throw new InvalidOperationException($"Invalid folder path: {folderPath}");
            }

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void MoveToScene(GameObject gameObject, Scene scene)
        {
            if (scene.IsValid() && gameObject.scene != scene)
            {
                SceneManager.MoveGameObjectToScene(gameObject, scene);
            }
        }

        private static void Run(string operation, Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"{LogPrefix} operation='{operation}' status='Failed' " +
                    $"exception='{exception.GetType().Name}' message='{exception.Message}'.\n{exception}");
            }
        }
    }
}
