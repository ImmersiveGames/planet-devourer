using System;
using System.Collections.Generic;
using Immersive.Framework.Authoring;
using Immersive.Framework.GameFlow;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FirstGame.FrameworkModels.ActivityReadiness.Editor
{
    internal static class M03ActivityReadinessReentryTool
    {
        private const string LogPrefix = "[FIRSTGAME_M03_REENTRY_TOOL]";

        private const string ModelRoot =
            "Assets/_Project/FrameworkModels/M03_ActivityReadiness";

        private const string PreparationActivityPath =
            ModelRoot + "/Activities/Activity_M03_Preparation.asset";

        private const string IntermissionActivityPath =
            ModelRoot + "/Activities/Activity_M03_Intermission.asset";

        private const string IntermissionProfilePath =
            ModelRoot + "/Profiles/ActivityContent_M03_Intermission.asset";

        private const string IntermissionScenePath =
            ModelRoot + "/Scenes/M03_Intermission_Add.unity";

        private const string RouteScenePath =
            ModelRoot + "/Scenes/M03_Route.unity";

        private const string NavigationPrefabPath =
            ModelRoot + "/Prefabs/PF_M03_ActivityNavigation.prefab";

        private const string RouteUiMountName =
            "UI Mount (Configure Manually)";

        private const string NavigationInstanceName =
            "PF_M03_ActivityNavigation";

        [MenuItem(
            "Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Create or Configure Re-entry",
            priority = 2310)]
        private static void CreateOrConfigureMenu()
        {
            Run("CreateOrConfigureReentry", CreateOrConfigure);
        }

        [MenuItem(
            "Tools/Immersive Framework/FIRSTGAME/M03 Activity Readiness/Validate Re-entry",
            priority = 2311)]
        private static void ValidateMenu()
        {
            Run("ValidateReentry", Validate);
        }

        private static void CreateOrConfigure()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning(
                    $"{LogPrefix} operation='CreateOrConfigureReentry' status='CancelledByUser'.");
                return;
            }

            ActivityAsset preparationActivity =
                RequireAsset<ActivityAsset>(PreparationActivityPath);

            ActivityContentProfileAsset intermissionProfile =
                CreateOrLoadAsset<ActivityContentProfileAsset>(
                    IntermissionProfilePath);

            ActivityAsset intermissionActivity =
                CreateOrLoadAsset<ActivityAsset>(
                    IntermissionActivityPath);

            ConfigureIntermissionProfile(intermissionProfile);
            ConfigureIntermissionActivity(
                intermissionActivity,
                intermissionProfile);

            EnsureIntermissionScene();
            EnsureSceneInBuildSettings(IntermissionScenePath);

            CreateOrConfigureNavigationPrefab(
                preparationActivity,
                intermissionActivity);

            // Reopen the saved prefab and apply the two target bindings explicitly.
            // This avoids relying on references written while a brand-new prefab
            // root is still an unsaved temporary object.
            ConfigureSavedNavigationTargets(
                preparationActivity,
                intermissionActivity);

            GameObject navigationPrefab =
                RequireAsset<GameObject>(
                    NavigationPrefabPath);

            ComposeRouteScene(navigationPrefab);

            // The Route scene is the concrete authoring instance and therefore
            // owns the operational Activity targets. Configure the scene instance
            // explicitly instead of depending on prefab external-reference
            // persistence.
            ConfigureRouteSceneNavigationTargets();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                $"{LogPrefix} operation='CreateOrConfigureReentry' status='Succeeded' " +
                "intermissionActivity='Configured' intermissionProfile='Configured' " +
                "intermissionScene='Configured' navigationTemplate='Configured' routeSceneBindings='Configured'.");
        }

        private static void ConfigureIntermissionProfile(
            ActivityContentProfileAsset profile)
        {
            SerializedObject serialized = new SerializedObject(profile);

            SerializedProperty profileId =
                RequireProperty(serialized, "profileId");
            profileId.stringValue = "m03.intermission.profile";

            SerializedProperty scenes =
                RequireProperty(serialized, "scenes");
            scenes.arraySize = 1;

            SerializedProperty entry =
                scenes.GetArrayElementAtIndex(0);

            RequireRelative(entry, "contentId").stringValue =
                "m03.intermission.scene";

            RequireRelative(entry, "scenePath").stringValue =
                IntermissionScenePath;

            RequireRelative(entry, "sceneName").stringValue =
                "M03_Intermission_Add";

            RequireRelative(entry, "requiredness").intValue = 10;
            RequireRelative(entry, "loadMode").intValue = 0;
            RequireRelative(entry, "releasePolicy").intValue = 0;

            SerializedProperty description =
                serialized.FindProperty("description");
            if (description != null)
            {
                description.stringValue =
                    "Neutral M03 Activity used only to execute a real exit and re-entry of the readiness Activity.";
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(profile);
        }

        private static void ConfigureIntermissionActivity(
            ActivityAsset activity,
            ActivityContentProfileAsset profile)
        {
            SerializedObject serialized = new SerializedObject(activity);

            SerializedProperty activityId =
                RequireProperty(serialized, "activityId");
            if (string.IsNullOrWhiteSpace(activityId.stringValue))
            {
                activityId.stringValue =
                    Guid.NewGuid().ToString("N");
            }

            RequireProperty(serialized, "activityName").stringValue =
                "Activity M03 Intermission";

            SerializedProperty description =
                serialized.FindProperty("description");
            if (description != null)
            {
                description.stringValue =
                    "Neutral happy-path Activity used to release and re-enter Activity M03 Preparation.";
            }

            RequireProperty(
                serialized,
                "activityContentProfile").objectReferenceValue = profile;

            SerializedProperty visualTransitionMode =
                serialized.FindProperty("visualTransitionMode");
            if (visualTransitionMode != null)
            {
                visualTransitionMode.intValue = 0;
            }

            SerializedProperty transitionGateMode =
                serialized.FindProperty("transitionGateMode");
            if (transitionGateMode != null)
            {
                transitionGateMode.intValue = 10;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(activity);
        }

        private static void EnsureIntermissionScene()
        {
            Scene scene;

            if (System.IO.File.Exists(IntermissionScenePath))
            {
                scene = EditorSceneManager.OpenScene(
                    IntermissionScenePath,
                    OpenSceneMode.Single);
            }
            else
            {
                scene = EditorSceneManager.NewScene(
                    NewSceneSetup.EmptyScene,
                    NewSceneMode.Single);
            }

            GameObject root =
                FindSceneObject(scene, "M03_Intermission_Add_Root");

            if (root == null)
            {
                root = new GameObject(
                    "M03_Intermission_Add_Root");
                SceneManager.MoveGameObjectToScene(root, scene);
            }

            GameObject visual =
                FindChild(root.transform, "Intermission Visual")?.gameObject;

            if (visual == null)
            {
                visual = GameObject.CreatePrimitive(
                    PrimitiveType.Cylinder);
                visual.name = "Intermission Visual";
                SceneManager.MoveGameObjectToScene(visual, scene);
                visual.transform.SetParent(root.transform, false);
                visual.transform.localPosition =
                    new Vector3(0f, 0.75f, 0f);
                visual.transform.localScale =
                    new Vector3(1.4f, 0.25f, 1.4f);
            }

            Transform labelTransform =
                FindChild(root.transform, "Label");

            TextMesh label;
            if (labelTransform == null)
            {
                GameObject labelObject =
                    new GameObject("Label");
                SceneManager.MoveGameObjectToScene(
                    labelObject,
                    scene);
                labelObject.transform.SetParent(
                    root.transform,
                    false);
                labelObject.transform.localPosition =
                    new Vector3(0f, 2.1f, 0f);
                label = labelObject.AddComponent<TextMesh>();
            }
            else
            {
                label =
                    labelTransform.GetComponent<TextMesh>() ??
                    labelTransform.gameObject.AddComponent<TextMesh>();
            }

            label.text =
                "M03 Intermission\nReturn to Preparation to prove re-entry";
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.characterSize = 0.08f;
            label.fontSize = 64;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(
                scene,
                IntermissionScenePath);
        }

        private static GameObject CreateOrConfigureNavigationPrefab(
            ActivityAsset preparationActivity,
            ActivityAsset intermissionActivity)
        {
            GameObject root;
            bool loadedPrefabContents;

            if (AssetDatabase.LoadAssetAtPath<GameObject>(
                    NavigationPrefabPath) != null)
            {
                root = PrefabUtility.LoadPrefabContents(
                    NavigationPrefabPath);
                loadedPrefabContents = true;
            }
            else
            {
                root = new GameObject(
                    NavigationInstanceName,
                    typeof(RectTransform),
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster));
                loadedPrefabContents = false;
            }

            try
            {
                root.name = NavigationInstanceName;

                Canvas canvas =
                    GetOrAddComponent<Canvas>(root);
                canvas.renderMode =
                    RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 200;

                CanvasScaler scaler =
                    GetOrAddComponent<CanvasScaler>(root);
                scaler.uiScaleMode =
                    CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution =
                    new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;

                GetOrAddComponent<GraphicRaycaster>(root);

                RectTransform rootRect =
                    root.GetComponent<RectTransform>();
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;
                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;

                RectTransform panel =
                    EnsurePanel(root.transform);

                EnsureTitle(panel);

                EnsureActivityButton(
                    panel,
                    "Leave Preparation",
                    "Leave Preparation",
                    intermissionActivity,
                    "m03.leave-preparation");

                EnsureActivityButton(
                    panel,
                    "Return to Preparation",
                    "Return to Preparation",
                    preparationActivity,
                    "m03.return-to-preparation");

                GameObject saved =
                    PrefabUtility.SaveAsPrefabAsset(
                        root,
                        NavigationPrefabPath);

                if (saved == null)
                {
                    throw new InvalidOperationException(
                        $"Could not save navigation prefab: {NavigationPrefabPath}");
                }

                return saved;
            }
            finally
            {
                if (loadedPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
        }

        private static void ConfigureSavedNavigationTargets(
            ActivityAsset preparationActivity,
            ActivityAsset intermissionActivity)
        {
            GameObject root =
                PrefabUtility.LoadPrefabContents(
                    NavigationPrefabPath);

            try
            {
                Transform panel =
                    FindChild(
                        root.transform,
                        "Navigation Panel");

                if (panel == null)
                {
                    throw new InvalidOperationException(
                        "Navigation Panel was not found in the saved navigation prefab.");
                }

                ConfigureSavedNavigationButton(
                    panel,
                    "Leave Preparation",
                    intermissionActivity,
                    "m03.leave-preparation");

                ConfigureSavedNavigationButton(
                    panel,
                    "Return to Preparation",
                    preparationActivity,
                    "m03.return-to-preparation");

                PrefabUtility.SaveAsPrefabAsset(
                    root,
                    NavigationPrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(
                    root);
            }
        }

        private static void ConfigureSavedNavigationButton(
            Transform panel,
            string objectName,
            ActivityAsset targetActivity,
            string reason)
        {
            Transform buttonTransform =
                FindDirectChild(
                    panel,
                    objectName);

            if (buttonTransform == null)
            {
                throw new InvalidOperationException(
                    $"Navigation button '{objectName}' was not found.");
            }

            ActivityRequestTrigger trigger =
                buttonTransform.GetComponent<ActivityRequestTrigger>();

            Button button =
                buttonTransform.GetComponent<Button>();

            if (trigger == null || button == null)
            {
                throw new InvalidOperationException(
                    $"Navigation button '{objectName}' is missing ActivityRequestTrigger or Button.");
            }

            SerializedObject serializedTrigger =
                new SerializedObject(trigger);

            RequireProperty(
                serializedTrigger,
                "targetActivity").objectReferenceValue =
                targetActivity;

            RequireProperty(
                serializedTrigger,
                "reason").stringValue =
                reason;

            serializedTrigger.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(trigger);

            EnsurePersistentListener(
                button.onClick,
                trigger,
                nameof(ActivityRequestTrigger.RequestActivity),
                trigger.RequestActivity);
        }

        private static RectTransform EnsurePanel(
            Transform root)
        {
            Transform existing =
                FindChild(root, "Navigation Panel");

            GameObject panelObject;
            if (existing == null)
            {
                panelObject = new GameObject(
                    "Navigation Panel",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(VerticalLayoutGroup),
                    typeof(ContentSizeFitter));

                panelObject.transform.SetParent(
                    root,
                    false);
            }
            else
            {
                panelObject = existing.gameObject;
            }

            RectTransform rect =
                panelObject.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition =
                new Vector2(-32f, 32f);
            rect.sizeDelta =
                new Vector2(430f, 0f);

            Image image =
                GetOrAddComponent<Image>(panelObject);
            image.color =
                new Color(0.04f, 0.055f, 0.08f, 0.92f);

            VerticalLayoutGroup layout =
                GetOrAddComponent<VerticalLayoutGroup>(
                    panelObject);
            layout.padding =
                new RectOffset(18, 18, 18, 18);
            layout.spacing = 12f;
            layout.childAlignment =
                TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter =
                GetOrAddComponent<ContentSizeFitter>(
                    panelObject);
            fitter.horizontalFit =
                ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit =
                ContentSizeFitter.FitMode.PreferredSize;

            return rect;
        }

        private static void EnsureTitle(
            RectTransform panel)
        {
            Text title =
                EnsureText(
                    panel,
                    "Title",
                    "M03 Activity Readiness",
                    30,
                    FontStyle.Bold);

            LayoutElement layout =
                GetOrAddComponent<LayoutElement>(
                    title.gameObject);
            layout.preferredHeight = 48f;
        }

        private static void EnsureActivityButton(
            RectTransform panel,
            string objectName,
            string label,
            ActivityAsset targetActivity,
            string reason)
        {
            Transform existing =
                FindChild(panel, objectName);

            GameObject buttonObject;
            if (existing == null)
            {
                buttonObject = new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(Button),
                    typeof(LayoutElement));

                buttonObject.transform.SetParent(
                    panel,
                    false);
            }
            else
            {
                buttonObject = existing.gameObject;
            }

            Image image =
                GetOrAddComponent<Image>(buttonObject);
            image.color =
                new Color(0.14f, 0.22f, 0.34f, 1f);

            Button button =
                GetOrAddComponent<Button>(buttonObject);
            button.targetGraphic = image;

            LayoutElement layout =
                GetOrAddComponent<LayoutElement>(
                    buttonObject);
            layout.preferredHeight = 58f;

            Text text =
                EnsureText(
                    buttonObject.transform,
                    "Label",
                    label,
                    25,
                    FontStyle.Normal);

            RectTransform textRect =
                text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin =
                new Vector2(12f, 4f);
            textRect.offsetMax =
                new Vector2(-12f, -4f);

            ActivityRequestTrigger trigger =
                GetOrAddComponent<ActivityRequestTrigger>(
                    buttonObject);

            SerializedObject serializedTrigger =
                new SerializedObject(trigger);

            RequireProperty(
                serializedTrigger,
                "targetActivity").objectReferenceValue =
                targetActivity;

            RequireProperty(
                serializedTrigger,
                "reason").stringValue =
                reason;

            serializedTrigger.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(trigger);

            EnsurePersistentListener(
                button.onClick,
                trigger,
                nameof(ActivityRequestTrigger.RequestActivity),
                trigger.RequestActivity);
        }

        private static Text EnsureText(
            Transform parent,
            string objectName,
            string value,
            int fontSize,
            FontStyle fontStyle)
        {
            Transform existing =
                FindChild(parent, objectName);

            GameObject textObject;
            if (existing == null)
            {
                textObject = new GameObject(
                    objectName,
                    typeof(RectTransform),
                    typeof(Text));

                textObject.transform.SetParent(
                    parent,
                    false);
            }
            else
            {
                textObject = existing.gameObject;
            }

            Text text =
                GetOrAddComponent<Text>(textObject);

            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment =
                TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;

            if (text.font == null)
            {
                text.font =
                    Resources.GetBuiltinResource<Font>(
                        "LegacyRuntime.ttf");
            }

            return text;
        }

        private static void ComposeRouteScene(
            GameObject navigationPrefab)
        {
            Scene scene =
                EditorSceneManager.OpenScene(
                    RouteScenePath,
                    OpenSceneMode.Single);

            Transform uiMount =
                RequireSceneTransform(
                    scene,
                    RouteUiMountName);

            GameObject existing =
                FindSceneObject(
                    scene,
                    NavigationInstanceName);

            if (existing == null)
            {
                GameObject instance =
                    PrefabUtility.InstantiatePrefab(
                        navigationPrefab,
                        scene) as GameObject;

                if (instance == null)
                {
                    throw new InvalidOperationException(
                        "Could not instantiate PF_M03_ActivityNavigation.");
                }

                instance.name =
                    NavigationInstanceName;
                instance.transform.SetParent(
                    uiMount,
                    false);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureRouteSceneNavigationTargets()
        {
            Scene scene =
                EditorSceneManager.OpenScene(
                    RouteScenePath,
                    OpenSceneMode.Single);

            GameObject navigationInstance =
                FindSceneObject(
                    scene,
                    NavigationInstanceName);

            if (navigationInstance == null)
            {
                throw new InvalidOperationException(
                    "M03_Route has no PF_M03_ActivityNavigation instance.");
            }

            Transform panel =
                FindChild(
                    navigationInstance.transform,
                    "Navigation Panel");

            if (panel == null)
            {
                throw new InvalidOperationException(
                    "Navigation Panel was not found in the Route scene instance.");
            }

            ConfigureSceneNavigationButton(
                navigationInstance,
                panel,
                "Leave Preparation",
                IntermissionActivityPath,
                "m03.leave-preparation");

            ConfigureSceneNavigationButton(
                navigationInstance,
                panel,
                "Return to Preparation",
                PreparationActivityPath,
                "m03.return-to-preparation");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            // Verify against the saved scene state, not only the in-memory object.
            VerifyConfiguredSceneNavigationButton(
                panel,
                "Leave Preparation",
                IntermissionActivityPath,
                "m03.leave-preparation");

            VerifyConfiguredSceneNavigationButton(
                panel,
                "Return to Preparation",
                PreparationActivityPath,
                "m03.return-to-preparation");
        }

        private static void ConfigureSceneNavigationButton(
            GameObject navigationInstance,
            Transform panel,
            string objectName,
            string targetActivityPath,
            string reason)
        {
            Transform buttonTransform =
                FindDirectChild(
                    panel,
                    objectName);

            if (buttonTransform == null)
            {
                throw new InvalidOperationException(
                    $"Route navigation button '{objectName}' was not found.");
            }

            ActivityRequestTrigger trigger =
                buttonTransform.GetComponent<ActivityRequestTrigger>();

            Button button =
                buttonTransform.GetComponent<Button>();

            if (trigger == null || button == null)
            {
                throw new InvalidOperationException(
                    $"Route navigation button '{objectName}' is missing ActivityRequestTrigger or Button.");
            }

            ActivityAsset targetActivity =
                AssetDatabase.LoadAssetAtPath<ActivityAsset>(
                    targetActivityPath);

            if (targetActivity == null)
            {
                throw new InvalidOperationException(
                    $"Activity target could not be loaded: {targetActivityPath}");
            }

            if (!EditorUtility.IsPersistent(targetActivity))
            {
                throw new InvalidOperationException(
                    $"Activity target is not a persistent asset: {targetActivityPath}");
            }

            // First apply the values to the concrete scene component.
            SerializedObject serializedTrigger =
                new SerializedObject(trigger);

            serializedTrigger.Update();

            RequireProperty(
                serializedTrigger,
                "targetActivity").objectReferenceValue =
                targetActivity;

            RequireProperty(
                serializedTrigger,
                "reason").stringValue =
                reason;

            serializedTrigger.ApplyModifiedProperties();

            EnsurePersistentListener(
                button.onClick,
                trigger,
                nameof(ActivityRequestTrigger.RequestActivity),
                trigger.RequestActivity);

            EditorUtility.SetDirty(trigger);
            EditorUtility.SetDirty(button);

            // Then record the two fields as explicit prefab-instance overrides.
            UnityEngine.Object outermostRoot =
                PrefabUtility.GetOutermostPrefabInstanceRoot(
                    navigationInstance);

            ActivityRequestTrigger sourceTrigger =
                PrefabUtility.GetCorrespondingObjectFromSource(
                    trigger);

            if (outermostRoot != null &&
                sourceTrigger != null)
            {
                PropertyModification[] existing =
                    PrefabUtility.GetPropertyModifications(
                        outermostRoot) ??
                    Array.Empty<PropertyModification>();

                List<PropertyModification> modifications =
                    new List<PropertyModification>(
                        existing.Length + 2);

                foreach (PropertyModification modification in existing)
                {
                    if (modification == null)
                    {
                        continue;
                    }

                    bool isManagedProperty =
                        modification.target == sourceTrigger &&
                        (string.Equals(
                             modification.propertyPath,
                             "targetActivity",
                             StringComparison.Ordinal) ||
                         string.Equals(
                             modification.propertyPath,
                             "reason",
                             StringComparison.Ordinal));

                    if (!isManagedProperty)
                    {
                        modifications.Add(modification);
                    }
                }

                modifications.Add(
                    new PropertyModification
                    {
                        target = sourceTrigger,
                        propertyPath = "targetActivity",
                        objectReference = targetActivity
                    });

                modifications.Add(
                    new PropertyModification
                    {
                        target = sourceTrigger,
                        propertyPath = "reason",
                        value = reason
                    });

                PrefabUtility.SetPropertyModifications(
                    outermostRoot,
                    modifications.ToArray());
            }
            else
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(
                    trigger);
            }

            Debug.Log(
                $"{LogPrefix} operation='ConfigureSceneNavigationButton' " +
                $"button='{objectName}' target='{targetActivityPath}' " +
                $"targetPersistent='True' reason='{reason}'.");
        }

        private static void VerifyConfiguredSceneNavigationButton(
            Transform panel,
            string objectName,
            string expectedTargetPath,
            string expectedReason)
        {
            Transform buttonTransform =
                FindDirectChild(
                    panel,
                    objectName);

            ActivityRequestTrigger trigger =
                buttonTransform != null
                    ? buttonTransform.GetComponent<ActivityRequestTrigger>()
                    : null;

            if (trigger == null)
            {
                throw new InvalidOperationException(
                    $"Configured Route button '{objectName}' has no ActivityRequestTrigger.");
            }

            SerializedObject serialized =
                new SerializedObject(trigger);

            serialized.Update();

            SerializedProperty target =
                RequireProperty(
                    serialized,
                    "targetActivity");

            SerializedProperty reason =
                RequireProperty(
                    serialized,
                    "reason");

            string actualPath =
                AssetDatabase.GetAssetPath(
                    target.objectReferenceValue);

            if (!string.Equals(
                    actualPath,
                    expectedTargetPath,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Route button '{objectName}' target did not persist. " +
                    $"expected='{expectedTargetPath}' actual='{actualPath}'.");
            }

            if (!string.Equals(
                    reason.stringValue,
                    expectedReason,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Route button '{objectName}' reason did not persist.");
            }
        }

        private static void Validate()
        {
            List<string> issues =
                new List<string>();

            ActivityAsset preparation =
                AssetDatabase.LoadAssetAtPath<ActivityAsset>(
                    PreparationActivityPath);

            ActivityAsset intermission =
                AssetDatabase.LoadAssetAtPath<ActivityAsset>(
                    IntermissionActivityPath);

            ActivityContentProfileAsset profile =
                AssetDatabase.LoadAssetAtPath<ActivityContentProfileAsset>(
                    IntermissionProfilePath);

            if (preparation == null)
            {
                issues.Add(
                    "Activity_M03_Preparation is missing.");
            }

            if (intermission == null)
            {
                issues.Add(
                    "Activity_M03_Intermission is missing.");
            }

            if (profile == null)
            {
                issues.Add(
                    "ActivityContent_M03_Intermission is missing.");
            }

            if (!System.IO.File.Exists(
                    IntermissionScenePath))
            {
                issues.Add(
                    "M03_Intermission_Add scene is missing.");
            }

            if (!IsSceneInBuildSettings(
                    IntermissionScenePath))
            {
                issues.Add(
                    "M03_Intermission_Add is not registered in Build Settings.");
            }

            GameObject navigation =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    NavigationPrefabPath);

            if (navigation == null)
            {
                issues.Add(
                    "PF_M03_ActivityNavigation is missing.");
            }
            else
            {
                ValidateNavigationTemplate(
                    navigation,
                    issues);
            }

            ValidateRouteScene(
                issues);

            if (issues.Count == 0)
            {
                Debug.Log(
                    $"{LogPrefix} operation='ValidateReentry' status='Passed' " +
                    "intermissionActivity='Configured' intermissionScene='Configured' " +
                    "navigationTemplate='Configured' routeSceneBindings='Configured'.");
                return;
            }

            Debug.LogError(
                $"{LogPrefix} operation='ValidateReentry' status='Failed' " +
                $"issues='{issues.Count}'\n- " +
                string.Join("\n- ", issues));
        }

        private static void ValidateNavigationTemplate(
            GameObject navigation,
            List<string> issues)
        {
            Transform panel =
                FindChild(
                    navigation.transform,
                    "Navigation Panel");

            if (panel == null)
            {
                issues.Add(
                    "Navigation prefab has no Navigation Panel.");
                return;
            }

            ValidateNavigationTemplateButton(
                panel,
                "Leave Preparation",
                issues);

            ValidateNavigationTemplateButton(
                panel,
                "Return to Preparation",
                issues);
        }

        private static void ValidateNavigationTemplateButton(
            Transform panel,
            string objectName,
            List<string> issues)
        {
            Transform buttonTransform =
                FindDirectChild(
                    panel,
                    objectName);

            if (buttonTransform == null)
            {
                issues.Add(
                    $"Navigation prefab has no '{objectName}' button.");
                return;
            }

            ActivityRequestTrigger trigger =
                buttonTransform.GetComponent<ActivityRequestTrigger>();

            Button button =
                buttonTransform.GetComponent<Button>();

            if (trigger == null)
            {
                issues.Add(
                    $"Navigation prefab button '{objectName}' has no ActivityRequestTrigger.");
            }

            if (button == null)
            {
                issues.Add(
                    $"Navigation prefab button '{objectName}' has no Button.");
            }
        }

        private static void ValidateRouteScene(
            List<string> issues)
        {
            Scene scene =
                SceneManager.GetSceneByPath(
                    RouteScenePath);

            bool openedTemporarily =
                !scene.IsValid() ||
                !scene.isLoaded;

            if (openedTemporarily)
            {
                scene =
                    EditorSceneManager.OpenScene(
                        RouteScenePath,
                        OpenSceneMode.Additive);
            }

            try
            {
                GameObject navigation =
                    FindSceneObject(
                        scene,
                        NavigationInstanceName);

                if (navigation == null)
                {
                    issues.Add(
                        "M03_Route has no PF_M03_ActivityNavigation instance.");
                    return;
                }

                Transform panel =
                    FindChild(
                        navigation.transform,
                        "Navigation Panel");

                if (panel == null)
                {
                    issues.Add(
                        "Route navigation instance has no Navigation Panel.");
                    return;
                }

                ValidateRouteSceneButton(
                    panel,
                    "Leave Preparation",
                    IntermissionActivityPath,
                    "m03.leave-preparation",
                    issues);

                ValidateRouteSceneButton(
                    panel,
                    "Return to Preparation",
                    PreparationActivityPath,
                    "m03.return-to-preparation",
                    issues);
            }
            finally
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
        }

        private static void ValidateRouteSceneButton(
            Transform panel,
            string objectName,
            string expectedPath,
            string expectedReason,
            List<string> issues)
        {
            Transform buttonTransform =
                FindDirectChild(
                    panel,
                    objectName);

            if (buttonTransform == null)
            {
                issues.Add(
                    $"Route navigation has no '{objectName}' button.");
                return;
            }

            ActivityRequestTrigger trigger =
                buttonTransform.GetComponent<ActivityRequestTrigger>();

            Button button =
                buttonTransform.GetComponent<Button>();

            if (trigger == null || button == null)
            {
                issues.Add(
                    $"Route navigation button '{objectName}' is missing ActivityRequestTrigger or Button.");
                return;
            }

            SerializedObject serialized =
                new SerializedObject(trigger);

            serialized.Update();

            SerializedProperty target =
                serialized.FindProperty("targetActivity");

            SerializedProperty reason =
                serialized.FindProperty("reason");

            string actualPath =
                target != null
                    ? AssetDatabase.GetAssetPath(
                        target.objectReferenceValue)
                    : string.Empty;

            if (!string.Equals(
                    actualPath,
                    expectedPath,
                    StringComparison.Ordinal))
            {
                issues.Add(
                    $"Route navigation button '{objectName}' target is invalid. " +
                    $"expected='{expectedPath}' actual='{actualPath}'.");
            }

            if (reason == null ||
                !string.Equals(
                    reason.stringValue,
                    expectedReason,
                    StringComparison.Ordinal))
            {
                issues.Add(
                    $"Route navigation button '{objectName}' reason is invalid.");
            }

            if (!HasPersistentListener(
                    button.onClick,
                    trigger,
                    nameof(ActivityRequestTrigger.RequestActivity)))
            {
                issues.Add(
                    $"Route navigation button '{objectName}' is not wired to RequestActivity.");
            }
        }

        private static T CreateOrLoadAsset<T>(
            string path)
            where T : ScriptableObject
        {
            T asset =
                AssetDatabase.LoadAssetAtPath<T>(
                    path);

            if (asset != null)
            {
                return asset;
            }

            asset =
                ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(
                asset,
                path);

            return asset;
        }

        private static T RequireAsset<T>(
            string path)
            where T : UnityEngine.Object
        {
            T asset =
                AssetDatabase.LoadAssetAtPath<T>(
                    path);

            if (asset == null)
            {
                throw new InvalidOperationException(
                    $"Required asset not found: {path}");
            }

            return asset;
        }

        private static void EnsureSceneInBuildSettings(
            string scenePath)
        {
            if (IsSceneInBuildSettings(scenePath))
            {
                return;
            }

            List<EditorBuildSettingsScene> scenes =
                new List<EditorBuildSettingsScene>(
                    EditorBuildSettings.scenes)
                {
                    new EditorBuildSettingsScene(
                        scenePath,
                        true)
                };

            EditorBuildSettings.scenes =
                scenes.ToArray();
        }

        private static bool IsSceneInBuildSettings(
            string scenePath)
        {
            foreach (
                EditorBuildSettingsScene scene
                in EditorBuildSettings.scenes)
            {
                if (string.Equals(
                        scene.path,
                        scenePath,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void EnsurePersistentListener(
            UnityEvent unityEvent,
            UnityEngine.Object target,
            string methodName,
            UnityAction action)
        {
            if (HasPersistentListener(
                    unityEvent,
                    target,
                    methodName))
            {
                return;
            }

            UnityEventTools.AddPersistentListener(
                unityEvent,
                action);
        }

        private static bool HasPersistentListener(
            UnityEvent unityEvent,
            UnityEngine.Object target,
            string methodName)
        {
            for (
                int index = 0;
                index <
                unityEvent.GetPersistentEventCount();
                index++)
            {
                if (
                    unityEvent.GetPersistentTarget(index) ==
                    target &&
                    string.Equals(
                        unityEvent.GetPersistentMethodName(index),
                        methodName,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static SerializedProperty RequireProperty(
            SerializedObject serializedObject,
            string propertyName)
        {
            SerializedProperty property =
                serializedObject.FindProperty(
                    propertyName);

            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Serialized property '{propertyName}' was not found on " +
                    $"'{serializedObject.targetObject.GetType().Name}'.");
            }

            return property;
        }

        private static SerializedProperty RequireRelative(
            SerializedProperty property,
            string relativeName)
        {
            SerializedProperty relative =
                property.FindPropertyRelative(
                    relativeName);

            if (relative == null)
            {
                throw new InvalidOperationException(
                    $"Relative serialized property '{relativeName}' was not found.");
            }

            return relative;
        }

        private static T GetOrAddComponent<T>(
            GameObject gameObject)
            where T : Component
        {
            T component =
                gameObject.GetComponent<T>();

            return component != null
                ? component
                : gameObject.AddComponent<T>();
        }

        private static Transform RequireSceneTransform(
            Scene scene,
            string name)
        {
            GameObject gameObject =
                FindSceneObject(scene, name);

            if (gameObject == null)
            {
                throw new InvalidOperationException(
                    $"Scene object not found: {name}");
            }

            return gameObject.transform;
        }

        private static GameObject FindSceneObject(
            Scene scene,
            string name)
        {
            if (!scene.IsValid() ||
                !scene.isLoaded)
            {
                return null;
            }

            foreach (
                GameObject root
                in scene.GetRootGameObjects())
            {
                Transform found =
                    FindChild(
                        root.transform,
                        name);

                if (found != null)
                {
                    return found.gameObject;
                }
            }

            return null;
        }

        private static Transform FindDirectChild(
            Transform parent,
            string name)
        {
            for (
                int index = 0;
                index < parent.childCount;
                index++)
            {
                Transform child =
                    parent.GetChild(index);

                if (string.Equals(
                        child.name,
                        name,
                        StringComparison.Ordinal))
                {
                    return child;
                }
            }

            return null;
        }

        private static Transform FindChild(
            Transform parent,
            string name)
        {
            if (string.Equals(
                    parent.name,
                    name,
                    StringComparison.Ordinal))
            {
                return parent;
            }

            for (
                int index = 0;
                index < parent.childCount;
                index++)
            {
                Transform found =
                    FindChild(
                        parent.GetChild(index),
                        name);

                if (found != null)
                {
                    return found;
                }
            }

            return null;
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
                Debug.LogError(
                    $"{LogPrefix} operation='{operation}' status='Failed' " +
                    $"exception='{exception.GetType().Name}' " +
                    $"message='{exception.Message}'.\n{exception}");
            }
        }
    }
}
