using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FirstGame.Diagnostics.Editor
{
    public static class LifecycleCanvasPrefabInstaller
    {
        private const string PrefabPath = "Assets/_Project/Prefabs/UI/Canvas-Lifecyle.prefab";
        private const string ActivityScenePath =
            "Assets/_Project/Scenes/RotesContents/Sample_Environment.unity";

        private const string CanvasName = "Canvas-Lifecyle";
        private const string SceneIdentity = "Sample_Environment";
        private const string RouteFallbackIdentity = "Sample Fields";

        private const string SceneLifecycleClassName = "SceneLifecycleEvents";
        private const string RouteLifecycleClassName = "RouteContentLifecycleEvents";
        private const string ActivityLifecycleClassName = "ActivityContentLifecycleEvents";
        private const string RouteBindingClassName = "RouteContentBinding";
        private const string ActivityBindingClassName = "ActivityLocalVisibilityAdapter";

        [MenuItem("FIRSTGAME/Diagnostics/Apply Lifecycle Canvas Report", priority = 1100)]
        public static void Apply()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE_AUTHORING] Apply is unavailable while entering or running Play Mode.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            try
            {
                int prefabSources = ApplyPrefab();
                int activitySources = ApplyActivityScene();
                AssetDatabase.SaveAssets();

                Debug.Log(
                    "[FIRSTGAME_LIFECYCLE_AUTHORING] status='Applied' " +
                    $"prefab='{PrefabPath}' activityScene='{ActivityScenePath}' " +
                    "mode='OnDemand' " +
                    $"prefabSources='{prefabSources}' activitySources='{activitySources}'.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        [MenuItem("FIRSTGAME/Diagnostics/Validate Lifecycle Canvas Report", priority = 1101)]
        public static void Validate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE_AUTHORING] Validation is unavailable while entering or running Play Mode.");
                return;
            }

            var issues = new List<string>();

            try
            {
                ValidatePrefab(issues);
                ValidateActivityScene(issues);

                if (issues.Count == 0)
                {
                    Debug.Log(
                        "[FIRSTGAME_LIFECYCLE_AUTHORING] status='Valid' issues='0' " +
                        "mode='OnDemand' routeIdentity='AuthoredAsset' activityIdentity='AuthoredAsset'.");
                    return;
                }

                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE_AUTHORING] status='Invalid' " +
                    $"issues='{issues.Count}'.\n- {string.Join("\n- ", issues)}");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private static int ApplyPrefab()
        {
            EnsureAssetExists<GameObject>(PrefabPath);
            GameObject prefabRoot = null;

            try
            {
                prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);
                EnsureCanvasRoot(prefabRoot);
                RemoveLegacyHistory(prefabRoot);

                Transform panel = FindRequiredTransform(prefabRoot, "Panel");
                Transform titleContainer = FindRequiredTransform(prefabRoot, "Titulo");
                Transform infos = FindRequiredTransform(prefabRoot, "Infos");
                Transform reportContainer = FindReportContainer(prefabRoot);
                Transform lastEventLabel = FindOptionalTransform(prefabRoot, "LastEventLabel");

                TMP_Text titleText = FindTitleText(titleContainer);
                TMP_Text reportText = FindReportText(prefabRoot);

                ConfigureReportLayout(
                    panel,
                    titleContainer,
                    infos,
                    reportContainer,
                    lastEventLabel,
                    titleText,
                    reportText);

                LifecycleCanvasPresenter presenter =
                    prefabRoot.GetComponent<LifecycleCanvasPresenter>() ??
                    prefabRoot.AddComponent<LifecycleCanvasPresenter>();
                ConfigurePresenter(presenter, reportText);

                List<LifecycleSource> lifecycleSources = FindLifecycleSources(prefabRoot);
                if (lifecycleSources.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Prefab '{PrefabPath}' has no supported lifecycle event components.");
                }

                foreach (LifecycleSource source in lifecycleSources)
                {
                    LifecycleCanvasEventReporter reporter =
                        source.Component.GetComponent<LifecycleCanvasEventReporter>() ??
                        source.Component.gameObject.AddComponent<LifecycleCanvasEventReporter>();

                    string identity = ResolveAuthoredIdentity(source, reporter);
                    ConfigureReporter(reporter, presenter, source.Scope, identity);
                    WireLifecycleEvents(prefabRoot, source, reporter);
                }

                DisableCanvasRaycastTargets(prefabRoot);
                EditorUtility.SetDirty(prefabRoot);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, PrefabPath);
                AssetDatabase.ImportAsset(PrefabPath, ImportAssetOptions.ForceUpdate);
                return lifecycleSources.Count;
            }
            finally
            {
                if (prefabRoot != null)
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }

        private static int ApplyActivityScene()
        {
            EnsureAssetExists<SceneAsset>(ActivityScenePath);

            Scene scene = SceneManager.GetSceneByPath(ActivityScenePath);
            bool openedByInstaller = !scene.IsValid() || !scene.isLoaded;

            if (openedByInstaller)
            {
                scene = EditorSceneManager.OpenScene(ActivityScenePath, OpenSceneMode.Additive);
            }

            try
            {
                LifecycleCanvasPresenter presenter = FindScenePresenter(scene);
                if (presenter == null)
                {
                    throw new InvalidOperationException(
                        $"Scene '{ActivityScenePath}' has no '{CanvasName}' instance with " +
                        "LifecycleCanvasPresenter. Apply the prefab report and reopen the scene.");
                }

                List<Component> activityBindings = FindComponentsInScene(scene, ActivityBindingClassName);
                if (activityBindings.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Scene '{ActivityScenePath}' has no {ActivityBindingClassName} components.");
                }

                Type lifecycleType = ResolveMonoBehaviourType(ActivityLifecycleClassName);
                GameObject canvasRoot = presenter.gameObject;
                int configured = 0;

                foreach (Component activityBinding in activityBindings)
                {
                    UnityEngine.Object activityAsset = GetRequiredObjectReference(
                        activityBinding,
                        "activity",
                        $"Activity binding '{BuildTransformPath(activityBinding.transform)}'");

                    string activityIdentity = activityAsset.name.Trim();
                    Component activityLifecycle = activityBinding.gameObject
                        .GetComponents<Component>()
                        .FirstOrDefault(component =>
                            component != null && component.GetType().Name == ActivityLifecycleClassName);

                    if (activityLifecycle == null)
                    {
                        activityLifecycle = activityBinding.gameObject.AddComponent(lifecycleType);
                    }

                    LifecycleCanvasEventReporter reporter =
                        activityBinding.GetComponent<LifecycleCanvasEventReporter>() ??
                        activityBinding.gameObject.AddComponent<LifecycleCanvasEventReporter>();

                    ConfigureReporter(
                        reporter,
                        presenter,
                        LifecycleCanvasScope.Activity,
                        activityIdentity);

                    var source = LifecycleSource.ForActivity(activityLifecycle);
                    WireLifecycleEvents(canvasRoot, source, reporter);

                    EditorUtility.SetDirty(activityBinding.gameObject);
                    EditorUtility.SetDirty(activityLifecycle);
                    EditorUtility.SetDirty(reporter);
                    configured++;
                }

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene))
                {
                    throw new InvalidOperationException(
                        $"Unity failed to save scene '{ActivityScenePath}'.");
                }

                return configured;
            }
            finally
            {
                if (openedByInstaller && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void ValidatePrefab(ICollection<string> issues)
        {
            EnsureAssetExists<GameObject>(PrefabPath);
            GameObject prefabRoot = null;

            try
            {
                prefabRoot = PrefabUtility.LoadPrefabContents(PrefabPath);
                EnsureCanvasRoot(prefabRoot);

                TMP_Text reportText = FindOptionalText(prefabRoot, "LifecycleReportValue");
                LifecycleCanvasPresenter presenter =
                    prefabRoot.GetComponent<LifecycleCanvasPresenter>();

                if (reportText == null)
                {
                    issues.Add("Prefab is missing LifecycleReportValue.");
                }

                if (presenter == null)
                {
                    issues.Add("Canvas root has no LifecycleCanvasPresenter.");
                }
                else
                {
                    SerializedProperty property =
                        new SerializedObject(presenter).FindProperty("reportValue");
                    if (property == null || property.objectReferenceValue != reportText)
                    {
                        issues.Add("Presenter does not reference LifecycleReportValue.");
                    }
                }

                Transform infos = FindOptionalTransform(prefabRoot, "Infos");
                if (infos == null || infos.gameObject.activeSelf)
                {
                    issues.Add("Original Infos must exist and remain inactive.");
                }

                List<LifecycleSource> sources = FindLifecycleSources(prefabRoot);
                if (sources.Count == 0)
                {
                    issues.Add("Prefab has no supported lifecycle source.");
                }

                foreach (LifecycleSource source in sources)
                {
                    ValidateLifecycleSource(
                        issues,
                        prefabRoot,
                        source,
                        presenter,
                        ResolveAuthoredIdentity(source, source.Component.GetComponent<LifecycleCanvasEventReporter>()));
                }
            }
            finally
            {
                if (prefabRoot != null)
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }

        private static void ValidateActivityScene(ICollection<string> issues)
        {
            EnsureAssetExists<SceneAsset>(ActivityScenePath);

            Scene scene = SceneManager.GetSceneByPath(ActivityScenePath);
            bool openedByValidator = !scene.IsValid() || !scene.isLoaded;

            if (openedByValidator)
            {
                scene = EditorSceneManager.OpenScene(ActivityScenePath, OpenSceneMode.Additive);
            }

            try
            {
                LifecycleCanvasPresenter presenter = FindScenePresenter(scene);
                if (presenter == null)
                {
                    issues.Add($"Scene '{ActivityScenePath}' has no LifecycleCanvasPresenter instance.");
                    return;
                }

                List<Component> activityBindings = FindComponentsInScene(scene, ActivityBindingClassName);
                if (activityBindings.Count == 0)
                {
                    issues.Add($"Scene '{ActivityScenePath}' has no ActivityLocalVisibilityAdapter.");
                    return;
                }

                foreach (Component activityBinding in activityBindings)
                {
                    string path = BuildTransformPath(activityBinding.transform);
                    UnityEngine.Object activityAsset = GetOptionalObjectReference(activityBinding, "activity");
                    if (activityAsset == null)
                    {
                        issues.Add($"Activity binding '{path}' has no authored Activity asset.");
                        continue;
                    }

                    Component activityLifecycle = activityBinding.gameObject
                        .GetComponents<Component>()
                        .FirstOrDefault(component =>
                            component != null && component.GetType().Name == ActivityLifecycleClassName);

                    if (activityLifecycle == null)
                    {
                        issues.Add($"Activity binding '{path}' has no ActivityContentLifecycleEvents.");
                        continue;
                    }

                    var source = LifecycleSource.ForActivity(activityLifecycle);

                    ValidateLifecycleSource(
                        issues,
                        presenter.gameObject,
                        source,
                        presenter,
                        activityAsset.name.Trim());
                }
            }
            finally
            {
                if (openedByValidator && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void ValidateLifecycleSource(
            ICollection<string> issues,
            GameObject canvasRoot,
            LifecycleSource source,
            LifecycleCanvasPresenter presenter,
            string expectedIdentity)
        {
            LifecycleCanvasEventReporter reporter =
                source.Component.GetComponent<LifecycleCanvasEventReporter>();
            string path = BuildTransformPath(source.Component.transform);

            if (reporter == null)
            {
                issues.Add($"Lifecycle source '{path}' has no LifecycleCanvasEventReporter.");
                return;
            }

            if (reporter.Presenter != presenter)
            {
                issues.Add($"Lifecycle source '{path}' does not reference the expected presenter.");
            }

            if (reporter.Scope != source.Scope)
            {
                issues.Add(
                    $"Lifecycle source '{path}' scope is '{reporter.Scope}', expected '{source.Scope}'.");
            }

            if (!string.Equals(reporter.Identity, expectedIdentity, StringComparison.Ordinal))
            {
                issues.Add(
                    $"Lifecycle source '{path}' identity is '{reporter.Identity}', " +
                    $"expected authored identity '{expectedIdentity}'.");
            }

            ValidateListener(
                issues,
                source.Component,
                source.FirstEventField,
                reporter,
                source.FirstReporterMethod);
            ValidateListener(
                issues,
                source.Component,
                source.SecondEventField,
                reporter,
                source.SecondReporterMethod);

            ValidateNoDirectTextListener(
                issues,
                source.Component,
                source.FirstEventField,
                canvasRoot);
            ValidateNoDirectTextListener(
                issues,
                source.Component,
                source.SecondEventField,
                canvasRoot);
        }

        private static LifecycleCanvasPresenter FindScenePresenter(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (string.Equals(root.name, CanvasName, StringComparison.Ordinal))
                {
                    LifecycleCanvasPresenter exact = root.GetComponent<LifecycleCanvasPresenter>();
                    if (exact != null)
                    {
                        return exact;
                    }
                }
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                LifecycleCanvasPresenter presenter =
                    root.GetComponentInChildren<LifecycleCanvasPresenter>(true);
                if (presenter != null)
                {
                    return presenter;
                }
            }

            return null;
        }

        private static List<Component> FindComponentsInScene(Scene scene, string className)
        {
            var results = new List<Component>();

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Component component in root.GetComponentsInChildren<Component>(true))
                {
                    if (component != null &&
                        string.Equals(component.GetType().Name, className, StringComparison.Ordinal))
                    {
                        results.Add(component);
                    }
                }
            }

            return results;
        }

        private static Type ResolveMonoBehaviourType(string className)
        {
            Type type = TypeCache.GetTypesDerivedFrom<MonoBehaviour>()
                .FirstOrDefault(candidate =>
                    string.Equals(candidate.Name, className, StringComparison.Ordinal));

            if (type == null)
            {
                throw new InvalidOperationException(
                    $"Could not resolve framework component type '{className}'.");
            }

            return type;
        }

        private static List<LifecycleSource> FindLifecycleSources(GameObject root)
        {
            var sources = new List<LifecycleSource>();

            foreach (Component component in root.GetComponentsInChildren<Component>(true))
            {
                if (component == null || component is LifecycleCanvasEventReporter)
                {
                    continue;
                }

                switch (component.GetType().Name)
                {
                    case SceneLifecycleClassName:
                        sources.Add(LifecycleSource.ForScene(component));
                        break;
                    case RouteLifecycleClassName:
                        sources.Add(LifecycleSource.ForRoute(component));
                        break;
                    case ActivityLifecycleClassName:
                        sources.Add(LifecycleSource.ForActivity(component));
                        break;
                }
            }

            return sources;
        }

        private static string ResolveAuthoredIdentity(
            LifecycleSource source,
            LifecycleCanvasEventReporter existingReporter)
        {
            switch (source.Scope)
            {
                case LifecycleCanvasScope.Scene:
                    return SceneIdentity;

                case LifecycleCanvasScope.Route:
                {
                    Component binding = FindComponentNearSource(source.Component, RouteBindingClassName);
                    UnityEngine.Object routeAsset = GetOptionalObjectReference(binding, "route");
                    if (routeAsset != null && !string.IsNullOrWhiteSpace(routeAsset.name))
                    {
                        return routeAsset.name.Trim();
                    }

                    if (existingReporter != null && !string.IsNullOrWhiteSpace(existingReporter.Identity))
                    {
                        return existingReporter.Identity.Trim();
                    }

                    return RouteFallbackIdentity;
                }

                case LifecycleCanvasScope.Activity:
                {
                    Component binding = FindComponentNearSource(source.Component, ActivityBindingClassName);
                    UnityEngine.Object activityAsset = GetOptionalObjectReference(binding, "activity");
                    if (activityAsset != null && !string.IsNullOrWhiteSpace(activityAsset.name))
                    {
                        return activityAsset.name.Trim();
                    }

                    if (existingReporter != null && !string.IsNullOrWhiteSpace(existingReporter.Identity))
                    {
                        return existingReporter.Identity.Trim();
                    }

                    return source.Component.gameObject.name;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Scope), source.Scope, null);
            }
        }

        private static Component FindComponentNearSource(Component source, string className)
        {
            if (source == null)
            {
                return null;
            }

            Component direct = source.gameObject.GetComponents<Component>()
                .FirstOrDefault(component =>
                    component != null && component.GetType().Name == className);
            if (direct != null)
            {
                return direct;
            }

            Transform parent = source.transform.parent;
            while (parent != null)
            {
                Component parentMatch = parent.GetComponents<Component>()
                    .FirstOrDefault(component =>
                        component != null && component.GetType().Name == className);
                if (parentMatch != null)
                {
                    return parentMatch;
                }

                parent = parent.parent;
            }

            return source.GetComponentsInChildren<Component>(true)
                .FirstOrDefault(component =>
                    component != null && component.GetType().Name == className);
        }

        private static UnityEngine.Object GetRequiredObjectReference(
            Component component,
            string propertyName,
            string description)
        {
            UnityEngine.Object value = GetOptionalObjectReference(component, propertyName);
            if (value == null)
            {
                throw new InvalidOperationException(
                    $"{description} has no required object reference '{propertyName}'.");
            }

            return value;
        }

        private static UnityEngine.Object GetOptionalObjectReference(
            Component component,
            string propertyName)
        {
            if (component == null)
            {
                return null;
            }

            var serialized = new SerializedObject(component);
            SerializedProperty exactProperty = serialized.FindProperty(propertyName);
            if (exactProperty != null &&
                exactProperty.propertyType == SerializedPropertyType.ObjectReference &&
                exactProperty.objectReferenceValue != null)
            {
                return exactProperty.objectReferenceValue;
            }

            SerializedProperty iterator = serialized.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyType != SerializedPropertyType.ObjectReference ||
                    iterator.objectReferenceValue == null ||
                    iterator.name.IndexOf(propertyName, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                return iterator.objectReferenceValue;
            }

            return null;
        }

        private static void ConfigurePresenter(
            LifecycleCanvasPresenter presenter,
            TMP_Text reportText)
        {
            var serialized = new SerializedObject(presenter);
            SetRequiredReference(serialized, "reportValue", reportText);
            SetBool(serialized, "writeConsoleLog", true);
            SetBool(serialized, "includeFrame", true);
            SetBool(serialized, "includeUnscaledTime", true);
            SetBool(serialized, "includeSourcePath", true);
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureReporter(
            LifecycleCanvasEventReporter reporter,
            LifecycleCanvasPresenter presenter,
            LifecycleCanvasScope scope,
            string identity)
        {
            var serialized = new SerializedObject(reporter);
            SetRequiredReference(serialized, "presenter", presenter);

            SerializedProperty scopeProperty = serialized.FindProperty("scope");
            SerializedProperty identityProperty = serialized.FindProperty("identity");
            if (scopeProperty == null || identityProperty == null)
            {
                throw new MissingFieldException(
                    typeof(LifecycleCanvasEventReporter).FullName,
                    "scope/identity");
            }

            scopeProperty.enumValueIndex = (int)scope;
            identityProperty.stringValue = identity.Trim();
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureReportLayout(
            Transform panel,
            Transform titleContainer,
            Transform infos,
            Transform reportContainer,
            Transform lastEventLabel,
            TMP_Text titleText,
            TMP_Text reportText)
        {
            panel.gameObject.SetActive(true);
            infos.gameObject.SetActive(false);

            if (lastEventLabel != null)
            {
                lastEventLabel.gameObject.SetActive(false);
            }

            reportContainer.gameObject.name = "LifecycleReport";
            reportContainer.gameObject.SetActive(true);
            reportText.gameObject.name = "LifecycleReportValue";
            reportText.gameObject.SetActive(true);

            titleText.text = "LIFECYCLE CALLBACK CHECK";
            titleText.raycastTarget = false;

            RectTransform panelRect = RequireRectTransform(panel);
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(0f, 320f);

            RectTransform titleRect = RequireRectTransform(titleContainer);
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(0f, 70f);

            RectTransform reportRect = RequireRectTransform(reportContainer);
            reportRect.anchorMin = Vector2.zero;
            reportRect.anchorMax = Vector2.one;
            reportRect.offsetMin = new Vector2(30f, 20f);
            reportRect.offsetMax = new Vector2(-30f, -78f);

            RectTransform textRect = RequireRectTransform(reportText.transform);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            reportText.text =
                "<b>WAITING FOR LIFECYCLE CALLBACK</b>\n" +
                "A Route, Activity or Scene will appear only after its callback is received.";
            reportText.enableAutoSizing = true;
            reportText.fontSizeMin = 14f;
            reportText.fontSizeMax = 28f;
            reportText.alignment = TextAlignmentOptions.TopLeft;
            reportText.overflowMode = TextOverflowModes.Overflow;
            reportText.richText = true;
            reportText.raycastTarget = false;
            reportText.margin = new Vector4(0f, 0f, 0f, 0f);
        }

        private static void RemoveLegacyHistory(GameObject root)
        {
            Transform legacy = FindOptionalTransform(root, "LifecycleDebugHistory");
            if (legacy != null)
            {
                UnityEngine.Object.DestroyImmediate(legacy.gameObject);
            }
        }

        private static void DisableCanvasRaycastTargets(GameObject root)
        {
            foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                text.raycastTarget = false;
            }
        }

        private static void WireLifecycleEvents(
            GameObject canvasRoot,
            LifecycleSource source,
            LifecycleCanvasEventReporter reporter)
        {
            switch (source.Scope)
            {
                case LifecycleCanvasScope.Scene:
                    WireNoArgumentEvent(
                        canvasRoot,
                        source.Component,
                        source.FirstEventField,
                        reporter,
                        source.FirstReporterMethod,
                        reporter.ReportAvailable);
                    WireNoArgumentEvent(
                        canvasRoot,
                        source.Component,
                        source.SecondEventField,
                        reporter,
                        source.SecondReporterMethod,
                        reporter.ReportReleasing);
                    break;

                case LifecycleCanvasScope.Route:
                case LifecycleCanvasScope.Activity:
                    WireNoArgumentEvent(
                        canvasRoot,
                        source.Component,
                        source.FirstEventField,
                        reporter,
                        source.FirstReporterMethod,
                        reporter.ReportEntered);
                    WireNoArgumentEvent(
                        canvasRoot,
                        source.Component,
                        source.SecondEventField,
                        reporter,
                        source.SecondReporterMethod,
                        reporter.ReportExited);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(source.Scope), source.Scope, null);
            }
        }

        private static void WireNoArgumentEvent(
            GameObject canvasRoot,
            Component source,
            string fieldName,
            LifecycleCanvasEventReporter reporter,
            string callbackName,
            UnityAction callback)
        {
            UnityEvent unityEvent = GetUnityEvent(source, fieldName);

            for (int index = unityEvent.GetPersistentEventCount() - 1; index >= 0; index--)
            {
                UnityEngine.Object target = unityEvent.GetPersistentTarget(index);
                string methodName = unityEvent.GetPersistentMethodName(index);

                bool existingReporterCallback =
                    target == reporter &&
                    string.Equals(methodName, callbackName, StringComparison.Ordinal);

                bool directCanvasTextCallback =
                    target is TMP_Text text &&
                    canvasRoot != null &&
                    text.transform.IsChildOf(canvasRoot.transform) &&
                    IsTextWriteMethod(methodName);

                if (existingReporterCallback || directCanvasTextCallback)
                {
                    UnityEventTools.RemovePersistentListener(unityEvent, index);
                }
            }

            UnityEventTools.AddPersistentListener(unityEvent, callback);
            EditorUtility.SetDirty(source);
        }

        private static void ValidateListener(
            ICollection<string> issues,
            Component source,
            string fieldName,
            UnityEngine.Object expectedTarget,
            string expectedMethod)
        {
            UnityEvent unityEvent = GetUnityEvent(source, fieldName);

            for (int index = 0; index < unityEvent.GetPersistentEventCount(); index++)
            {
                if (unityEvent.GetPersistentTarget(index) == expectedTarget &&
                    string.Equals(
                        unityEvent.GetPersistentMethodName(index),
                        expectedMethod,
                        StringComparison.Ordinal))
                {
                    return;
                }
            }

            issues.Add(
                $"Event '{source.GetType().Name}.{fieldName}' does not call " +
                $"'{expectedTarget.GetType().Name}.{expectedMethod}'.");
        }

        private static void ValidateNoDirectTextListener(
            ICollection<string> issues,
            Component source,
            string fieldName,
            GameObject canvasRoot)
        {
            UnityEvent unityEvent = GetUnityEvent(source, fieldName);

            for (int index = 0; index < unityEvent.GetPersistentEventCount(); index++)
            {
                UnityEngine.Object target = unityEvent.GetPersistentTarget(index);
                string methodName = unityEvent.GetPersistentMethodName(index);

                if (target is TMP_Text text &&
                    canvasRoot != null &&
                    text.transform.IsChildOf(canvasRoot.transform) &&
                    IsTextWriteMethod(methodName))
                {
                    issues.Add(
                        $"Event '{source.GetType().Name}.{fieldName}' still updates " +
                        $"TMP text '{text.gameObject.name}' directly.");
                }
            }
        }

        private static bool IsTextWriteMethod(string methodName)
        {
            return string.Equals(methodName, "set_text", StringComparison.Ordinal) ||
                   string.Equals(methodName, "SetText", StringComparison.Ordinal) ||
                   string.Equals(methodName, "SetTextInternal", StringComparison.Ordinal);
        }

        private static UnityEvent GetUnityEvent(Component source, string fieldName)
        {
            FieldInfo field = source.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field == null)
            {
                throw new MissingFieldException(source.GetType().FullName, fieldName);
            }

            if (!(field.GetValue(source) is UnityEvent unityEvent))
            {
                throw new InvalidOperationException(
                    $"Field '{source.GetType().FullName}.{fieldName}' is not a UnityEvent.");
            }

            return unityEvent;
        }

        private static void SetRequiredReference(
            SerializedObject serialized,
            string propertyName,
            UnityEngine.Object value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                throw new MissingFieldException(
                    serialized.targetObject.GetType().FullName,
                    propertyName);
            }

            property.objectReferenceValue = value;
        }

        private static void SetBool(
            SerializedObject serialized,
            string propertyName,
            bool value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                throw new MissingFieldException(
                    serialized.targetObject.GetType().FullName,
                    propertyName);
            }

            property.boolValue = value;
        }

        private static TMP_Text FindTitleText(Transform titleContainer)
        {
            TMP_Text title = titleContainer.GetComponentInChildren<TMP_Text>(true);
            if (title == null)
            {
                throw new InvalidOperationException("Title container has no TMP text.");
            }

            return title;
        }

        private static TMP_Text FindReportText(GameObject root)
        {
            TMP_Text report = FindOptionalText(root, "LifecycleReportValue") ??
                              FindOptionalText(root, "LastEventValue");
            if (report == null)
            {
                throw new InvalidOperationException(
                    "Canvas has neither LifecycleReportValue nor LastEventValue.");
            }

            return report;
        }

        private static Transform FindReportContainer(GameObject root)
        {
            Transform report = FindOptionalTransform(root, "LifecycleReport") ??
                               FindOptionalTransform(root, "Events");
            if (report == null)
            {
                throw new InvalidOperationException(
                    "Canvas has neither LifecycleReport nor Events.");
            }

            return report;
        }

        private static RectTransform RequireRectTransform(Transform value)
        {
            if (!(value is RectTransform rectTransform))
            {
                throw new InvalidOperationException(
                    $"Object '{value.name}' must use RectTransform.");
            }

            return rectTransform;
        }

        private static Transform FindRequiredTransform(GameObject root, string objectName)
        {
            Transform value = FindOptionalTransform(root, objectName);
            if (value == null)
            {
                throw new InvalidOperationException(
                    $"Prefab '{PrefabPath}' has no object named '{objectName}'.");
            }

            return value;
        }

        private static Transform FindOptionalTransform(GameObject root, string objectName)
        {
            return root.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(item =>
                    string.Equals(item.gameObject.name, objectName, StringComparison.Ordinal));
        }

        private static TMP_Text FindOptionalText(GameObject root, string objectName)
        {
            return root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(text =>
                    string.Equals(text.gameObject.name, objectName, StringComparison.Ordinal));
        }

        private static void EnsureAssetExists<T>(string assetPath)
            where T : UnityEngine.Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(assetPath) == null)
            {
                throw new InvalidOperationException(
                    $"Required asset was not found at '{assetPath}'.");
            }
        }

        private static void EnsureCanvasRoot(GameObject prefabRoot)
        {
            if (prefabRoot == null ||
                !string.Equals(prefabRoot.name, CanvasName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Prefab root must be named '{CanvasName}'.");
            }
        }

        private static string BuildTransformPath(Transform current)
        {
            if (current == null)
            {
                return "<missing>";
            }

            string path = current.name;
            while (current.parent != null)
            {
                current = current.parent;
                path = current.name + "/" + path;
            }

            return path;
        }

        private sealed class LifecycleSource
        {
            private LifecycleSource(
                Component component,
                LifecycleCanvasScope scope,
                string firstEventField,
                string secondEventField,
                string firstReporterMethod,
                string secondReporterMethod)
            {
                Component = component;
                Scope = scope;
                FirstEventField = firstEventField;
                SecondEventField = secondEventField;
                FirstReporterMethod = firstReporterMethod;
                SecondReporterMethod = secondReporterMethod;
            }

            public Component Component { get; }
            public LifecycleCanvasScope Scope { get; }
            public string FirstEventField { get; }
            public string SecondEventField { get; }
            public string FirstReporterMethod { get; }
            public string SecondReporterMethod { get; }

            public static LifecycleSource ForScene(Component component)
            {
                return new LifecycleSource(
                    component,
                    LifecycleCanvasScope.Scene,
                    "available",
                    "releasing",
                    nameof(LifecycleCanvasEventReporter.ReportAvailable),
                    nameof(LifecycleCanvasEventReporter.ReportReleasing));
            }

            public static LifecycleSource ForRoute(Component component)
            {
                return new LifecycleSource(
                    component,
                    LifecycleCanvasScope.Route,
                    "entered",
                    "exited",
                    nameof(LifecycleCanvasEventReporter.ReportEntered),
                    nameof(LifecycleCanvasEventReporter.ReportExited));
            }

            public static LifecycleSource ForActivity(Component component)
            {
                return new LifecycleSource(
                    component,
                    LifecycleCanvasScope.Activity,
                    "entered",
                    "exited",
                    nameof(LifecycleCanvasEventReporter.ReportEntered),
                    nameof(LifecycleCanvasEventReporter.ReportExited));
            }
        }
    }
}
