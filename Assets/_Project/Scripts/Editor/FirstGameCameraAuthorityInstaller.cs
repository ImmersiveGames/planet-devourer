using System;
using System.Collections.Generic;
using Immersive.Framework.Authoring;
using Immersive.Framework.Camera;
using Immersive.Framework.CameraAuthoring;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.Editor
{
    internal static class FirstGameCameraAuthorityInstaller
    {
        private const string GlobalScenePath =
            "Assets/_Project/Scenes/Menu/FG_UIGlobal.unity";

        private const string GameplayScenePath =
            "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";

        private const string ApplicationPath =
            "Assets/_Project/ScriptableObjects/ImmersiveFramework/" +
            "FG_GameApplication.asset";

        private const string OutputRootName =
            "FG Session Camera Output";

        private const string LegacyMainCameraName =
            "Main Camera";

        private const string SessionTargetName =
            "Session Camera Target";

        private const string SessionRigName =
            "Session Camera Rig";

        private const string ValidationControlsTypeName =
            "FirstGame.Gameplay.Diagnostics." +
            "FirstGameC9MCameraValidationControls";

        [MenuItem(
            "Immersive Framework/FIRSTGAME/" +
            "Install Persistent Camera Authority")]
        private static void Install()
        {
            string applicationGuid =
                RequireAssetGuid(ApplicationPath);

            Scene globalScene = EditorSceneManager.OpenScene(
                GlobalScenePath,
                OpenSceneMode.Single);

            GameApplicationAsset application =
                LoadPersistentApplication(
                    applicationGuid,
                    "global-materialization");

            string applicationName =
                application.ApplicationName;

            RebuildGlobalCameraAuthority(
                globalScene,
                application);

            SaveSceneOrThrow(
                globalScene,
                GlobalScenePath,
                "persistent camera authority");

            Scene gameplayScene = EditorSceneManager.OpenScene(
                GameplayScenePath,
                OpenSceneMode.Single);

            MigrateGameplay(gameplayScene);

            SaveSceneOrThrow(
                gameplayScene,
                GameplayScenePath,
                "gameplay camera migration");

            ValidatePersistedGlobalCameraAuthority(
                applicationGuid);

            EditorSceneManager.OpenScene(
                GameplayScenePath,
                OpenSceneMode.Single);

            Debug.Log(
                "[FIRSTGAME_CAMERA_AUTHORITY_SETUP] " +
                "status='Succeeded' " +
                $"application='{applicationName}' " +
                $"applicationPath='{ApplicationPath}' " +
                $"applicationGuid='{applicationGuid}' " +
                "sessionAssigned='True' " +
                "sessionRebuilt='True' " +
                "output='camera.output.main' " +
                "precedence='Player:50,Activity:100,Route:200,Session:300'.");
        }

        private static void RebuildGlobalCameraAuthority(
            Scene scene,
            GameApplicationAsset application)
        {
            GameObject outputRoot =
                ResolveOrCreateOutputRoot(scene);

            outputRoot.name = OutputRootName;

            UnityEngine.Camera unityCamera =
                EnsureComponent<UnityEngine.Camera>(outputRoot);

            CinemachineBrain brain =
                EnsureComponent<CinemachineBrain>(outputRoot);

            CameraOutputSessionBinding output =
                EnsureSingleOutputBinding(
                    scene,
                    outputRoot);

            ConfigureOutput(
                output,
                unityCamera,
                brain);

            RemoveLegacyMainCameraRoot(
                scene,
                outputRoot);

            Transform target =
                EnsureChild(
                    outputRoot.transform,
                    SessionTargetName).transform;

            target.localPosition =
                new Vector3(0f, 8f, 0f);

            CameraRigComposer composer =
                EnsureSessionRig(
                    outputRoot.transform,
                    target);

            SessionCameraOverrideBinding session =
                RebuildSessionOverride(
                    scene,
                    outputRoot);

            ConfigureSessionOverride(
                session,
                application,
                output,
                composer,
                target);

            ValidateGlobalState(
                scene,
                output,
                session,
                "before-save",
                expectedApplicationGuid: string.Empty,
                requirePersistentIdentity: false);

            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static GameObject ResolveOrCreateOutputRoot(
            Scene scene)
        {
            List<CameraOutputSessionBinding> outputs =
                FindInScene<CameraOutputSessionBinding>(scene);

            if (outputs.Count > 1)
            {
                throw new InvalidOperationException(
                    $"FG_UIGlobal contains '{outputs.Count}' " +
                    "CameraOutputSessionBinding components. " +
                    "Resolve the duplicate outputs before rebuilding.");
            }

            if (outputs.Count == 1)
            {
                return outputs[0].gameObject;
            }

            GameObject namedRoot =
                FindInSceneByName(
                    scene,
                    OutputRootName);

            if (namedRoot != null)
            {
                return namedRoot;
            }

            GameObject legacyMainCamera =
                FindRootByName(
                    scene,
                    LegacyMainCameraName);

            if (legacyMainCamera != null &&
                legacyMainCamera.GetComponent<UnityEngine.Camera>() != null)
            {
                return legacyMainCamera;
            }

            var created =
                new GameObject(OutputRootName);

            SceneManager.MoveGameObjectToScene(
                created,
                scene);

            return created;
        }

        private static CameraOutputSessionBinding
            EnsureSingleOutputBinding(
                Scene scene,
                GameObject outputRoot)
        {
            List<CameraOutputSessionBinding> outputs =
                FindInScene<CameraOutputSessionBinding>(scene);

            CameraOutputSessionBinding selected =
                outputRoot.GetComponent<
                    CameraOutputSessionBinding>();

            if (selected == null)
            {
                if (outputs.Count != 0)
                {
                    throw new InvalidOperationException(
                        "FG_UIGlobal has a camera output outside the " +
                        "selected persistent output root.");
                }

                selected =
                    outputRoot.AddComponent<
                        CameraOutputSessionBinding>();
            }

            foreach (CameraOutputSessionBinding candidate in outputs)
            {
                if (candidate != null &&
                    candidate != selected)
                {
                    throw new InvalidOperationException(
                        "FG_UIGlobal has more than one persistent " +
                        "camera output.");
                }
            }

            return selected;
        }

        private static void ConfigureOutput(
            CameraOutputSessionBinding output,
            UnityEngine.Camera unityCamera,
            CinemachineBrain brain)
        {
            Set(
                output,
                "outputId",
                "camera.output.main");

            Set(
                output,
                "unityCamera",
                unityCamera);

            Set(
                output,
                "cinemachineBrain",
                brain);

            Set(
                output,
                "initializeOnAwake",
                true);

            Set(
                output,
                "logDiagnostics",
                true);
        }

        private static CameraRigComposer EnsureSessionRig(
            Transform outputRoot,
            Transform target)
        {
            GameObject rigObject =
                EnsureChild(
                    outputRoot,
                    SessionRigName);

            CameraRigComposer composer =
                EnsureComponent<CameraRigComposer>(
                    rigObject);

            CinemachineCamera cinemachineCamera =
                EnsureComponent<CinemachineCamera>(
                    rigObject);

            cinemachineCamera.enabled = false;

            Set(
                composer,
                "cinemachineCamera",
                cinemachineCamera);

            Set(
                composer,
                "createCinemachineCameraIfMissing",
                false);

            Set(
                composer,
                "logApplyRebuildDiagnostics",
                false);

            Set(
                composer,
                "targetSourceKind",
                30);

            Set(
                composer,
                "explicitFollowTarget",
                target);

            Set(
                composer,
                "explicitLookAtTarget",
                target);

            Set(
                composer,
                "followOffset",
                new Vector3(0f, 14f, -18f));

            return composer;
        }

        private static SessionCameraOverrideBinding
            RebuildSessionOverride(
                Scene scene,
                GameObject outputRoot)
        {
            List<SessionCameraOverrideBinding> existing =
                FindInScene<SessionCameraOverrideBinding>(scene);

            foreach (SessionCameraOverrideBinding binding in existing)
            {
                if (binding != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        binding);
                }
            }

            SessionCameraOverrideBinding rebuilt =
                outputRoot.AddComponent<
                    SessionCameraOverrideBinding>();

            if (rebuilt == null)
            {
                throw new InvalidOperationException(
                    "Failed to rebuild SessionCameraOverrideBinding.");
            }

            return rebuilt;
        }

        private static void ConfigureSessionOverride(
            SessionCameraOverrideBinding session,
            GameApplicationAsset application,
            CameraOutputSessionBinding output,
            CameraRigComposer composer,
            Transform target)
        {
            var serialized =
                new SerializedObject(session);

            serialized.Update();

            RequireProperty(
                serialized,
                "assignedGameApplication").objectReferenceValue =
                application;

            RequireProperty(
                serialized,
                "persistentOutputSession").objectReferenceValue =
                output;

            RequireProperty(
                serialized,
                "scopeId").stringValue =
                "firstgame.session.camera";

            RequireProperty(
                serialized,
                "requestId").stringValue =
                "firstgame.camera.request.session";

            RequireProperty(
                serialized,
                "rigComposer").objectReferenceValue =
                composer;

            RequireProperty(
                serialized,
                "targetSource").objectReferenceValue =
                target;

            RequireProperty(
                serialized,
                "precedence").intValue =
                300;

            RequireProperty(
                serialized,
                "tieBreakerId").stringValue =
                "firstgame.session";

            RequireProperty(
                serialized,
                "logDiagnostics").boolValue =
                true;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(session);
        }

        private static void RemoveLegacyMainCameraRoot(
            Scene scene,
            GameObject outputRoot)
        {
            GameObject legacy =
                FindRootByName(
                    scene,
                    LegacyMainCameraName);

            if (legacy == null ||
                legacy == outputRoot)
            {
                return;
            }

            if (legacy.GetComponent<UnityEngine.Camera>() == null)
            {
                throw new InvalidOperationException(
                    "FG_UIGlobal contains a root named 'Main Camera' " +
                    "that is not a physical camera. It was not removed.");
            }

            UnityEngine.Object.DestroyImmediate(
                legacy);
        }

        private static void MigrateGameplay(
            Scene scene)
        {
            RemoveGameplayPhysicalOutputs(scene);

            RouteCameraOverrideBinding routeOverride =
                FindExactlyOne<RouteCameraOverrideBinding>(
                    scene,
                    "Route camera override");

            ActivityCameraOverrideBinding activityOverride =
                FindExactlyOne<ActivityCameraOverrideBinding>(
                    scene,
                    "Activity camera override");

            LocalPlayerCameraRequestBinding playerBinding =
                FindExactlyOne<LocalPlayerCameraRequestBinding>(
                    scene,
                    "Local Player camera request");

            Set(
                routeOverride,
                "precedence",
                200);

            Set(
                activityOverride,
                "precedence",
                100);

            Set(
                playerBinding,
                "outputSession",
                null);

            Set(
                playerBinding,
                "precedence",
                50);

            MonoBehaviour controls =
                FindValidationControls(scene);

            Set(
                controls,
                "outputSession",
                null);

            Set(
                controls,
                "playerBinding",
                playerBinding);

            Set(
                controls,
                "activityOverride",
                activityOverride);

            Set(
                controls,
                "routeOverride",
                routeOverride);

            Set(
                controls,
                "sessionOverride",
                null);

            Set(
                controls,
                "showControls",
                true);

            ValidateGameplayCameraShape(scene);

            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void RemoveGameplayPhysicalOutputs(
            Scene scene)
        {
            var remove =
                new HashSet<GameObject>();

            foreach (CameraOutputSessionBinding output in
                     FindInScene<CameraOutputSessionBinding>(scene))
            {
                remove.Add(output.gameObject);
            }

            foreach (CinemachineBrain brain in
                     FindInScene<CinemachineBrain>(scene))
            {
                remove.Add(brain.gameObject);
            }

            foreach (UnityEngine.Camera camera in
                     FindInScene<UnityEngine.Camera>(scene))
            {
                remove.Add(camera.gameObject);
            }

            foreach (GameObject target in remove)
            {
                if (target != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        target);
                }
            }
        }

        private static void ValidateGameplayCameraShape(
            Scene scene)
        {
            int physicalCameraCount =
                FindInScene<UnityEngine.Camera>(scene).Count;

            int brainCount =
                FindInScene<CinemachineBrain>(scene).Count;

            int outputCount =
                FindInScene<CameraOutputSessionBinding>(scene).Count;

            int sessionOverrideCount =
                FindInScene<SessionCameraOverrideBinding>(scene).Count;

            if (physicalCameraCount != 0 ||
                brainCount != 0 ||
                outputCount != 0 ||
                sessionOverrideCount != 0)
            {
                throw new InvalidOperationException(
                    "FG_Gameplay still contains persistent camera " +
                    "authority after migration. " +
                    $"cameras='{physicalCameraCount}' " +
                    $"brains='{brainCount}' " +
                    $"outputs='{outputCount}' " +
                    $"sessionOverrides='{sessionOverrideCount}'.");
            }
        }

        private static void ValidatePersistedGlobalCameraAuthority(
            string expectedApplicationGuid)
        {
            Scene scene = EditorSceneManager.OpenScene(
                GlobalScenePath,
                OpenSceneMode.Single);

            // Reload after the scene operation. UnityEngine.Object references
            // are not used as durable identity across Editor scene changes.
            LoadPersistentApplication(
                expectedApplicationGuid,
                "post-save-validation");

            CameraOutputSessionBinding output =
                FindExactlyOne<CameraOutputSessionBinding>(
                    scene,
                    "persistent camera output");

            SessionCameraOverrideBinding session =
                FindExactlyOne<SessionCameraOverrideBinding>(
                    scene,
                    "Session camera override");

            ValidateGlobalState(
                scene,
                output,
                session,
                "after-reopen",
                expectedApplicationGuid,
                requirePersistentIdentity: true);
        }

        private static void ValidateGlobalState(
            Scene scene,
            CameraOutputSessionBinding output,
            SessionCameraOverrideBinding session,
            string stage,
            string expectedApplicationGuid,
            bool requirePersistentIdentity)
        {
            if (session.gameObject != output.gameObject)
            {
                throw new InvalidOperationException(
                    $"Session override is not on the persistent output " +
                    $"root at '{stage}'.");
            }

            var serialized =
                new SerializedObject(session);

            serialized.Update();

            SerializedProperty applicationProperty =
                RequireProperty(
                    serialized,
                    "assignedGameApplication");

            if (applicationProperty.objectReferenceValue == null)
            {
                throw new InvalidOperationException(
                    $"Serialized Session GameApplicationAsset is missing " +
                    $"at '{stage}'.");
            }

            SerializedProperty outputProperty =
                RequireProperty(
                    serialized,
                    "persistentOutputSession");

            if (outputProperty.objectReferenceValue != output)
            {
                throw new InvalidOperationException(
                    $"Serialized Session persistent output reference is " +
                    $"invalid at '{stage}'.");
            }

            SerializedProperty precedenceProperty =
                RequireProperty(
                    serialized,
                    "precedence");

            if (precedenceProperty.intValue != 300)
            {
                throw new InvalidOperationException(
                    $"Serialized Session precedence is " +
                    $"'{precedenceProperty.intValue}' instead of '300' " +
                    $"at '{stage}'.");
            }

            if (requirePersistentIdentity)
            {
                GameApplicationAsset serializedApplication =
                    applicationProperty.objectReferenceValue
                        as GameApplicationAsset;

                if (serializedApplication == null)
                {
                    throw new InvalidOperationException(
                        $"Reloaded serialized Session GameApplicationAsset " +
                        $"is missing at '{stage}'.");
                }

                ValidateApplicationIdentity(
                    serializedApplication,
                    expectedApplicationGuid,
                    $"serialized-session:{stage}");

                if (session.AssignedGameApplication == null)
                {
                    throw new InvalidOperationException(
                        $"Reloaded Session GameApplicationAsset is " +
                        $"missing at '{stage}'.");
                }

                ValidateApplicationIdentity(
                    session.AssignedGameApplication,
                    expectedApplicationGuid,
                    $"managed-session:{stage}");

                if (session.PersistentOutputSession != output)
                {
                    throw new InvalidOperationException(
                        $"Reloaded Session persistent output reference " +
                        $"is invalid at '{stage}'.");
                }

                if (session.Precedence != 300)
                {
                    throw new InvalidOperationException(
                        $"Reloaded Session precedence is " +
                        $"'{session.Precedence}' instead of '300' " +
                        $"at '{stage}'.");
                }
            }

            int cameraCount =
                FindInScene<UnityEngine.Camera>(scene).Count;

            int brainCount =
                FindInScene<CinemachineBrain>(scene).Count;

            int outputCount =
                FindInScene<CameraOutputSessionBinding>(scene).Count;

            int sessionCount =
                FindInScene<SessionCameraOverrideBinding>(scene).Count;

            if (cameraCount != 1 ||
                brainCount != 1 ||
                outputCount != 1 ||
                sessionCount != 1)
            {
                throw new InvalidOperationException(
                    $"FG_UIGlobal camera authority shape is invalid " +
                    $"at '{stage}'. " +
                    $"cameras='{cameraCount}' " +
                    $"brains='{brainCount}' " +
                    $"outputs='{outputCount}' " +
                    $"sessionOverrides='{sessionCount}'.");
            }

            GameObject legacyMainCamera =
                FindRootByName(
                    scene,
                    LegacyMainCameraName);

            if (legacyMainCamera != null &&
                legacyMainCamera != output.gameObject)
            {
                throw new InvalidOperationException(
                    $"Legacy Main Camera still exists at '{stage}'.");
            }
        }

        private static string RequireAssetGuid(
            string path)
        {
            string guid =
                AssetDatabase.AssetPathToGUID(path);

            if (string.IsNullOrWhiteSpace(guid))
            {
                throw new InvalidOperationException(
                    $"Required asset has no GUID at '{path}'.");
            }

            return guid;
        }

        private static GameApplicationAsset LoadPersistentApplication(
            string expectedGuid,
            string stage)
        {
            string resolvedPath =
                AssetDatabase.GUIDToAssetPath(expectedGuid);

            if (!string.Equals(
                    resolvedPath,
                    ApplicationPath,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"FG_GameApplication GUID resolves to '{resolvedPath}' " +
                    $"instead of '{ApplicationPath}' at '{stage}'.");
            }

            GameApplicationAsset application =
                AssetDatabase.LoadAssetAtPath<GameApplicationAsset>(
                    resolvedPath);

            if (application == null)
            {
                throw new InvalidOperationException(
                    $"FG_GameApplication could not be loaded at " +
                    $"'{resolvedPath}' during '{stage}'.");
            }

            ValidateApplicationIdentity(
                application,
                expectedGuid,
                stage);

            return application;
        }

        private static void ValidateApplicationIdentity(
            GameApplicationAsset application,
            string expectedGuid,
            string stage)
        {
            if (application == null)
            {
                throw new InvalidOperationException(
                    $"GameApplicationAsset is missing at '{stage}'.");
            }

            if (!EditorUtility.IsPersistent(application))
            {
                throw new InvalidOperationException(
                    $"GameApplicationAsset is not a persistent asset at " +
                    $"'{stage}'.");
            }

            string path =
                AssetDatabase.GetAssetPath(application);

            if (!string.Equals(
                    path,
                    ApplicationPath,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"GameApplicationAsset points to '{path}' instead of " +
                    $"'{ApplicationPath}' at '{stage}'.");
            }

            string guid =
                AssetDatabase.AssetPathToGUID(path);

            if (!string.Equals(
                    guid,
                    expectedGuid,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"GameApplicationAsset GUID is '{guid}' instead of " +
                    $"'{expectedGuid}' at '{stage}'.");
            }
        }

        private static void SaveSceneOrThrow(
            Scene scene,
            string path,
            string operation)
        {
            EditorSceneManager.MarkSceneDirty(scene);

            if (!EditorSceneManager.SaveScene(
                    scene,
                    path))
            {
                throw new InvalidOperationException(
                    $"Scene '{path}' could not be saved during " +
                    $"'{operation}'.");
            }
        }

        private static MonoBehaviour FindValidationControls(
            Scene scene)
        {
            MonoBehaviour found = null;

            foreach (MonoBehaviour candidate in
                     Resources.FindObjectsOfTypeAll<MonoBehaviour>())
            {
                if (candidate == null ||
                    candidate.gameObject.scene != scene ||
                    !string.Equals(
                        candidate.GetType().FullName,
                        ValidationControlsTypeName,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                if (found != null)
                {
                    throw new InvalidOperationException(
                        "FG_Gameplay contains more than one " +
                        "FirstGameC9MCameraValidationControls.");
                }

                found = candidate;
            }

            return found
                ?? throw new InvalidOperationException(
                    "FG_Gameplay is missing " +
                    "FirstGameC9MCameraValidationControls.");
        }

        private static T FindExactlyOne<T>(
            Scene scene,
            string label)
            where T : Component
        {
            List<T> components =
                FindInScene<T>(scene);

            if (components.Count != 1)
            {
                throw new InvalidOperationException(
                    $"Scene '{scene.name}' requires exactly one " +
                    $"{label}, but '{components.Count}' were found.");
            }

            return components[0];
        }

        private static List<T> FindInScene<T>(
            Scene scene)
            where T : Component
        {
            var results =
                new List<T>();

            foreach (T candidate in
                     Resources.FindObjectsOfTypeAll<T>())
            {
                if (candidate != null &&
                    candidate.gameObject.scene == scene)
                {
                    results.Add(candidate);
                }
            }

            return results;
        }

        private static GameObject FindRootByName(
            Scene scene,
            string name)
        {
            foreach (GameObject root in
                     scene.GetRootGameObjects())
            {
                if (string.Equals(
                        root.name,
                        name,
                        StringComparison.Ordinal))
                {
                    return root;
                }
            }

            return null;
        }

        private static GameObject FindInSceneByName(
            Scene scene,
            string name)
        {
            foreach (GameObject root in
                     scene.GetRootGameObjects())
            {
                if (string.Equals(
                        root.name,
                        name,
                        StringComparison.Ordinal))
                {
                    return root;
                }

                foreach (Transform child in
                         root.GetComponentsInChildren<Transform>(
                             true))
                {
                    if (string.Equals(
                            child.name,
                            name,
                            StringComparison.Ordinal))
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }

        private static GameObject EnsureChild(
            Transform parent,
            string name)
        {
            for (int index = 0;
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
                    return child.gameObject;
                }
            }

            var childObject =
                new GameObject(name);

            childObject.transform.SetParent(
                parent,
                false);

            return childObject;
        }

        private static T EnsureComponent<T>(
            GameObject target)
            where T : Component
        {
            T component =
                target.GetComponent<T>();

            return component != null
                ? component
                : target.AddComponent<T>();
        }

        private static SerializedProperty RequireProperty(
            SerializedObject serialized,
            string property)
        {
            return serialized.FindProperty(property)
                ?? throw new InvalidOperationException(
                    $"Property '{property}' is missing on " +
                    $"'{serialized.targetObject.GetType().Name}'.");
        }

        private static void Set(
            UnityEngine.Object target,
            string property,
            object value)
        {
            if (target == null)
            {
                throw new ArgumentNullException(
                    nameof(target));
            }

            var serialized =
                new SerializedObject(target);

            serialized.Update();

            SerializedProperty item =
                serialized.FindProperty(property)
                ?? throw new InvalidOperationException(
                    $"Property '{property}' is missing on " +
                    $"'{target.GetType().Name}'.");

            switch (value)
            {
                case UnityEngine.Object unityObject:
                    item.objectReferenceValue =
                        unityObject;
                    break;

                case string text:
                    item.stringValue =
                        text;
                    break;

                case int integer:
                    item.intValue =
                        integer;
                    break;

                case bool boolean:
                    item.boolValue =
                        boolean;
                    break;

                case Vector3 vector:
                    item.vector3Value =
                        vector;
                    break;

                case null:
                    item.objectReferenceValue =
                        null;
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported value for '{property}'.");
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
