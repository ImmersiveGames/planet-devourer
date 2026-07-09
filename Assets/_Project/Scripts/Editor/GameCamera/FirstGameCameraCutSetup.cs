using Immersive.Framework.ActivityFlow;
using Immersive.Framework.Actors;
using Immersive.Framework.Authoring;
using Immersive.Framework.Camera;
using Immersive.Framework.Camera.Cinemachine;
using Immersive.Framework.RouteLifecycle;
using Immersive.Framework.PlayerSlots;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Editor.GameCamera
{
    /// <summary>
    /// Editor-only, idempotent FIRSTGAME camera setup for the first Cinemachine Route/Activity cut.
    /// It configures scene-authored bindings without using runtime global searches.
    /// </summary>
    public static class FirstGameCameraCutSetup
    {
        private const string MenuScenePath = "Assets/_Project/Scenes/Menu/FG_Menu.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";

        private const string MenuRoutePath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Routes/FG_MenuRoute.asset";
        private const string GameplayRoutePath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Routes/FG_GameplayRoute.asset";

        private const string ActivityAPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_A.asset";
        private const string ActivityBPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_B.asset";
        private const string ActivityCPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_C_RouteFallback.asset";
        private const string ActivityDPath = "Assets/_Project/ScriptableObjects/ImmersiveFramework/Activity/FG_Activity_D_StopBgm.asset";

        private const string ExpectedActorIdRaw = "firstgame.player";
        private const string ExpectedActorIdDiagnostic = "Actor:firstgame.player";
        private const string ExpectedPlayerSlotIdRaw = "player.1";
        private const string ExpectedPlayerSlotIdDiagnostic = "PlayerSlot:player.1";
        private const string ExpectedGameplayActionMap = "Player";
        private const string PlayerInputTypeFullName = "UnityEngine.InputSystem.PlayerInput";
        private const string PlayerInputActionsPropertyName = "m_Actions";
        private const string CanonicalIdentitySource = "PlayerActorDeclaration+PlayerSlotDeclaration";

        private static bool hasBlockingIssue;

        [MenuItem("FIRSTGAME/Immersive Framework/Configure Route-Activity Camera")]
        public static void ConfigureCameraCut()
        {
            hasBlockingIssue = false;

            ConfigureMenuScene();
            ConfigureGameplayScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (hasBlockingIssue)
            {
                Debug.LogError("[FIRSTGAME_CAMERA_SETUP] Route/Activity camera setup completed with blocking issues. Fix the logged errors before Play.");
                return;
            }

            Debug.Log("[FIRSTGAME_CAMERA_SETUP] Route/Activity camera setup configured.");
        }

        private static void ConfigureMenuScene()
        {
            Scene scene = EditorSceneManager.OpenScene(MenuScenePath, OpenSceneMode.Single);
            RouteAsset menuRoute = LoadRequiredAsset<RouteAsset>(MenuRoutePath);

            if (menuRoute == null)
            {
                return;
            }

            GameObject mainCamera = EnsureMainCamera(scene, new Vector3(0f, 1f, -10f), Quaternion.identity);
            EnsureComponent<UnityEngine.Camera>(mainCamera);
            EnsureComponent<CinemachineBrain>(mainCamera);
            RemoveForbiddenCameraComponents(mainCamera, removeCamera: false, removeBrain: false);

            GameObject root = EnsureRoot(scene, "FirstGameCameraRoot");
            RouteContentBinding routeContentBinding = EnsureComponent<RouteContentBinding>(root);
            FrameworkCameraDirector director = EnsureComponent<FrameworkCameraDirector>(root);
            FrameworkCinemachineRigApplier rigApplier = EnsureComponent<FrameworkCinemachineRigApplier>(root);
            FrameworkRouteCameraBinding routeCameraBinding = EnsureComponent<FrameworkRouteCameraBinding>(root);

            GameObject menuRig = EnsureCinemachineRig(scene, "MenuRoute_CameraRig", new Vector3(0f, 1f, -10f), Quaternion.identity);

            SetSerialized(routeContentBinding, "route", menuRoute);
            SetSerialized(routeContentBinding, "localContentId", "firstgame.menu.route.camera");
            SetSerialized(routeContentBinding, "requiredness", 10);

            SetSerialized(director, "defaultCameraRig", menuRig);
            SetSerialized(director, "defaultAnchors", (Object)null);
            SetSerialized(director, "routePriority", 20);
            SetSerialized(director, "activityPriority", 100);
            SetSerialized(director, "setRigActiveState", true);
            SetSerialized(director, "rigApplier", rigApplier);
            SetSerialized(director, "logTransitions", true);

            SetSerialized(routeCameraBinding, "routeCameraRig", menuRig);
            SetSerialized(routeCameraBinding, "routeAnchors", (Object)null);
            SetSerialized(routeCameraBinding, "director", director);
            SetSerialized(routeCameraBinding, "startupActivityCameraBinding", (Object)null);

            menuRig.SetActive(false);

            ValidateObjectReference(routeContentBinding, "route", menuRoute, "RouteContentBinding route on Menu camera root");
            ValidateObjectReference(routeCameraBinding, "routeCameraRig", menuRig, "FrameworkRouteCameraBinding routeCameraRig on Menu camera root");
            ValidateObjectReference(routeCameraBinding, "director", director, "FrameworkRouteCameraBinding director on Menu camera root");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureGameplayScene()
        {
            Scene scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
            RouteAsset gameplayRoute = LoadRequiredAsset<RouteAsset>(GameplayRoutePath);
            ActivityAsset activityA = LoadRequiredAsset<ActivityAsset>(ActivityAPath);
            ActivityAsset activityB = LoadRequiredAsset<ActivityAsset>(ActivityBPath);
            ActivityAsset activityC = LoadRequiredAsset<ActivityAsset>(ActivityCPath);
            ActivityAsset activityD = LoadRequiredAsset<ActivityAsset>(ActivityDPath);

            if (gameplayRoute == null || activityA == null || activityB == null || activityC == null || activityD == null)
            {
                return;
            }

            GameObject mainCamera = EnsureMainCamera(scene, new Vector3(0f, 4f, -10f), Quaternion.Euler(20f, 0f, 0f));
            EnsureComponent<UnityEngine.Camera>(mainCamera);
            EnsureComponent<CinemachineBrain>(mainCamera);
            RemoveForbiddenCameraComponents(mainCamera, removeCamera: false, removeBrain: false);

            GameObject root = EnsureRoot(scene, "FirstGameCameraRoot");
            RouteContentBinding routeContentBinding = EnsureComponent<RouteContentBinding>(root);
            FrameworkCameraDirector director = EnsureComponent<FrameworkCameraDirector>(root);
            FrameworkCinemachineRigApplier rigApplier = EnsureComponent<FrameworkCinemachineRigApplier>(root);
            FrameworkRouteCameraBinding routeCameraBinding = EnsureComponent<FrameworkRouteCameraBinding>(root);

            if (!TryResolveCanonicalPlayer(
                    scene,
                    Selection.activeGameObject,
                    out FirstGameCameraResolvedPlayer resolvedPlayer,
                    out string playerFailureReason))
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Canonical FIRSTGAME player was not resolved. failureReason='{playerFailureReason}'.");
                return;
            }

            Debug.Log(
                "[FIRSTGAME_CAMERA_SETUP] Canonical FIRSTGAME player resolved. " +
                $"playerObject='{resolvedPlayer.PlayerObjectName}' " +
                $"identitySource='{resolvedPlayer.IdentitySource}' " +
                $"resolutionSource='{resolvedPlayer.ResolutionSource}' " +
                $"resolvedByName='{resolvedPlayer.ResolvedByName}' " +
                $"actorId='{resolvedPlayer.ActorId}' " +
                $"playerSlotId='{resolvedPlayer.PlayerSlotId}'.");

            GameObject anchorsObject = EnsureRoot(scene, "FirstGameCameraAnchors");
            FrameworkCameraAnchorHost anchors = EnsureComponent<FrameworkCameraAnchorHost>(anchorsObject);
            Transform target = resolvedPlayer.Transform;
            SetSerialized(anchors, "trackingTarget", target);
            SetSerialized(anchors, "lookAtTarget", target);

            GameObject routeRig = EnsureCinemachineRig(scene, "GameplayRoute_CameraRig", new Vector3(0f, 5f, -10f), Quaternion.Euler(25f, 0f, 0f));
            GameObject activityARig = EnsureCinemachineRig(scene, "ActivityA_CameraRig", new Vector3(0f, 3.5f, -7f), Quaternion.Euler(18f, 0f, 0f));

            SetSerialized(routeContentBinding, "route", gameplayRoute);
            SetSerialized(routeContentBinding, "localContentId", "firstgame.gameplay.route.camera");
            SetSerialized(routeContentBinding, "requiredness", 10);

            SetSerialized(director, "defaultCameraRig", routeRig);
            SetSerialized(director, "defaultAnchors", anchors);
            SetSerialized(director, "routePriority", 20);
            SetSerialized(director, "activityPriority", 100);
            SetSerialized(director, "setRigActiveState", true);
            SetSerialized(director, "rigApplier", rigApplier);
            SetSerialized(director, "logTransitions", true);

            FrameworkActivityCameraBinding activityABinding = EnsureActivityBinding(scene, "ActivityA_ContentRoot", activityA, activityARig, FrameworkCameraActivityPolicy.UseOwnOrRetainActivityUntilRouteExit, anchors, director);
            EnsureActivityBinding(scene, "ActivityB_ContentRoot", activityB, null, FrameworkCameraActivityPolicy.UseOwnOrRetainActivityUntilRouteExit, anchors, director);
            EnsureActivityBinding(scene, "ActivityC_RouteFallback_ContentRoot", activityC, null, FrameworkCameraActivityPolicy.UseRoute, anchors, director);
            EnsureActivityBinding(scene, "ActivityD_StopBgm_ContentRoot", activityD, null, FrameworkCameraActivityPolicy.UseRoute, anchors, director);

            SetSerialized(routeCameraBinding, "routeCameraRig", routeRig);
            SetSerialized(routeCameraBinding, "routeAnchors", anchors);
            SetSerialized(routeCameraBinding, "director", director);
            SetSerialized(routeCameraBinding, "startupActivityCameraBinding", activityABinding);

            routeRig.SetActive(false);
            activityARig.SetActive(false);

            ValidateObjectReference(routeContentBinding, "route", gameplayRoute, "RouteContentBinding route on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "routeCameraRig", routeRig, "FrameworkRouteCameraBinding routeCameraRig on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "routeAnchors", anchors, "FrameworkRouteCameraBinding routeAnchors on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "director", director, "FrameworkRouteCameraBinding director on Gameplay camera root");
            ValidateObjectReference(routeCameraBinding, "startupActivityCameraBinding", activityABinding, "FrameworkRouteCameraBinding startupActivityCameraBinding on Gameplay camera root");
            ValidateObjectReference(anchors, "trackingTarget", target, "FrameworkCameraAnchorHost trackingTarget on Gameplay camera anchors");
            ValidateObjectReference(anchors, "lookAtTarget", target, "FrameworkCameraAnchorHost lookAtTarget on Gameplay camera anchors");

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static FrameworkActivityCameraBinding EnsureActivityBinding(
            Scene scene,
            string rootName,
            ActivityAsset expectedActivity,
            GameObject rig,
            FrameworkCameraActivityPolicy policy,
            FrameworkCameraAnchorHost anchors,
            FrameworkCameraDirector director)
        {
            GameObject root = FindInScene(scene, rootName);
            if (root == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Activity root not found. root='{rootName}'.");
                return null;
            }

            ActivityLocalVisibilityAdapter adapter = EnsureComponent<ActivityLocalVisibilityAdapter>(root);
            SetSerialized(adapter, "activity", expectedActivity);

            FrameworkActivityCameraBinding binding = EnsureComponent<FrameworkActivityCameraBinding>(root);
            SetSerialized(binding, "assignedActivity", expectedActivity);
            SetSerialized(binding, "activityCameraRig", rig);
            SetSerialized(binding, "policy", (int)policy);
            SetSerialized(binding, "anchors", anchors);
            SetSerialized(binding, "director", director);

            ValidateObjectReference(adapter, "activity", expectedActivity, $"ActivityLocalVisibilityAdapter activity on '{rootName}'");
            ValidateObjectReference(binding, "assignedActivity", expectedActivity, $"FrameworkActivityCameraBinding assignedActivity on '{rootName}'");
            ValidateObjectReference(binding, "director", director, $"FrameworkActivityCameraBinding director on '{rootName}'");
            ValidateObjectReference(binding, "anchors", anchors, $"FrameworkActivityCameraBinding anchors on '{rootName}'");

            if (rig != null)
            {
                ValidateObjectReference(binding, "activityCameraRig", rig, $"FrameworkActivityCameraBinding activityCameraRig on '{rootName}'");
            }

            return binding;
        }

        private static GameObject EnsureMainCamera(Scene scene, Vector3 position, Quaternion rotation)
        {
            GameObject cameraObject = FindInScene(scene, "Main Camera");
            if (cameraObject == null)
            {
                cameraObject = new GameObject("Main Camera");
                SceneManager.MoveGameObjectToScene(cameraObject, scene);
            }

            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetPositionAndRotation(position, rotation);
            return cameraObject;
        }

        private static GameObject EnsureRoot(Scene scene, string name)
        {
            GameObject root = FindInScene(scene, name);
            if (root != null)
            {
                return root;
            }

            root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return root;
        }

        private static GameObject EnsureCinemachineRig(Scene scene, string name, Vector3 position, Quaternion rotation)
        {
            GameObject rig = FindInScene(scene, name);
            if (rig == null)
            {
                rig = new GameObject(name);
                SceneManager.MoveGameObjectToScene(rig, scene);
            }

            rig.transform.SetPositionAndRotation(position, rotation);
            EnsureComponent<CinemachineCamera>(rig);
            RemoveForbiddenCameraComponents(rig, removeCamera: true, removeBrain: true);
            return rig;
        }

        private static void RemoveForbiddenCameraComponents(GameObject owner, bool removeCamera, bool removeBrain)
        {
            if (removeCamera && owner.TryGetComponent(out UnityEngine.Camera camera))
            {
                Object.DestroyImmediate(camera);
            }

            if (removeBrain && owner.TryGetComponent(out CinemachineBrain brain))
            {
                Object.DestroyImmediate(brain);
            }

            if (owner.TryGetComponent(out AudioListener listener))
            {
                Object.DestroyImmediate(listener);
            }
        }

        private static T EnsureComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

        private static GameObject FindInScene(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform match = FindInChildren(root.transform, name);
                if (match != null)
                {
                    return match.gameObject;
                }
            }

            return null;
        }

        private static Transform FindInChildren(Transform root, string name)
        {
            if (root.name == name)
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform match = FindInChildren(root.GetChild(i), name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static bool TryResolveCanonicalPlayer(
            Scene scene,
            GameObject selectedObject,
            out FirstGameCameraResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "None";

            if (!scene.IsValid())
            {
                failureReason = "InvalidScene";
                return false;
            }

            if (TryResolveFromSelection(scene, selectedObject, out resolvedPlayer, out failureReason))
            {
                return true;
            }

            if (!string.Equals(failureReason, "None", System.StringComparison.Ordinal))
            {
                return false;
            }

            if (TryResolveByActorId(scene, out resolvedPlayer, out failureReason))
            {
                return true;
            }

            if (!string.Equals(failureReason, "NoMatchingPlayerActorDeclaration", System.StringComparison.Ordinal))
            {
                return false;
            }

            if (TryResolveBySlotId(scene, out resolvedPlayer, out failureReason))
            {
                return true;
            }

            if (!string.Equals(failureReason, "NoMatchingPlayerSlotDeclaration", System.StringComparison.Ordinal))
            {
                return false;
            }

            return TryResolveByCoherentPlayerInput(scene, out resolvedPlayer, out failureReason);
        }

        private static bool TryResolveFromSelection(
            Scene scene,
            GameObject selectedObject,
            out FirstGameCameraResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "None";

            if (selectedObject == null || selectedObject.scene != scene)
            {
                return false;
            }

            Component selectedInput = FindRelatedPlayerInputComponent(selectedObject);
            PlayerActorDeclaration selectedActor = FindRelatedComponent<PlayerActorDeclaration>(selectedObject);
            PlayerSlotDeclaration selectedSlot = FindRelatedComponent<PlayerSlotDeclaration>(selectedObject);

            if (selectedInput == null && selectedActor == null && selectedSlot == null)
            {
                return false;
            }

            GameObject target = selectedInput != null
                ? selectedInput.gameObject
                : selectedActor != null
                    ? selectedActor.gameObject
                    : selectedSlot.gameObject;

            if (!TryBuildCandidate(target, "Selection", out FirstGameCameraPlayerCandidate candidate, out failureReason))
            {
                return false;
            }

            return TryValidateCandidate(candidate, out resolvedPlayer, out failureReason);
        }

        private static bool TryResolveByActorId(
            Scene scene,
            out FirstGameCameraResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "NoMatchingPlayerActorDeclaration";
            FirstGameCameraPlayerCandidate match = default;
            int matches = 0;

            foreach (PlayerActorDeclaration actorDeclaration in FindSceneComponents<PlayerActorDeclaration>(scene))
            {
                if (actorDeclaration == null || !IsExpectedActorId(actorDeclaration.ActorId.ToString()))
                {
                    continue;
                }

                if (!TryBuildCandidate(actorDeclaration.gameObject, "PlayerActorDeclaration", out FirstGameCameraPlayerCandidate candidate, out string candidateFailure))
                {
                    failureReason = candidateFailure;
                    return false;
                }

                match = candidate;
                matches++;
            }

            if (matches == 0)
            {
                return false;
            }

            if (matches > 1)
            {
                failureReason = "MultipleMatchingPlayerActorDeclarations";
                return false;
            }

            return TryValidateCandidate(match, out resolvedPlayer, out failureReason);
        }

        private static bool TryResolveBySlotId(
            Scene scene,
            out FirstGameCameraResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "NoMatchingPlayerSlotDeclaration";
            FirstGameCameraPlayerCandidate match = default;
            int matches = 0;

            foreach (PlayerSlotDeclaration slotDeclaration in FindSceneComponents<PlayerSlotDeclaration>(scene))
            {
                if (slotDeclaration == null || !IsExpectedPlayerSlotId(slotDeclaration.PlayerSlotId.ToString()))
                {
                    continue;
                }

                if (!TryBuildCandidate(slotDeclaration.gameObject, "PlayerSlotDeclaration", out FirstGameCameraPlayerCandidate candidate, out string candidateFailure))
                {
                    failureReason = candidateFailure;
                    return false;
                }

                match = candidate;
                matches++;
            }

            if (matches == 0)
            {
                return false;
            }

            if (matches > 1)
            {
                failureReason = "MultipleMatchingPlayerSlotDeclarations";
                return false;
            }

            return TryValidateCandidate(match, out resolvedPlayer, out failureReason);
        }

        private static bool TryResolveByCoherentPlayerInput(
            Scene scene,
            out FirstGameCameraResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "NoCoherentPlayerInputCandidate";
            FirstGameCameraPlayerCandidate match = default;
            int matches = 0;

            foreach (Component playerInput in FindScenePlayerInputComponents(scene))
            {
                if (playerInput == null)
                {
                    continue;
                }

                if (!TryBuildCandidate(playerInput.gameObject, "PlayerInput", out FirstGameCameraPlayerCandidate candidate, out _))
                {
                    continue;
                }

                if (!candidate.HasExpectedIdentity)
                {
                    continue;
                }

                match = candidate;
                matches++;
            }

            if (matches == 0)
            {
                return false;
            }

            if (matches > 1)
            {
                failureReason = "MultipleCoherentPlayerInputCandidates";
                return false;
            }

            return TryValidateCandidate(match, out resolvedPlayer, out failureReason);
        }

        private static bool TryBuildCandidate(
            GameObject target,
            string resolutionSource,
            out FirstGameCameraPlayerCandidate candidate,
            out string failureReason)
        {
            candidate = default;
            failureReason = "None";

            if (target == null)
            {
                failureReason = "MissingPlayerCandidateGameObject";
                return false;
            }

            PlayerActorDeclaration actorDeclaration = target.GetComponent<PlayerActorDeclaration>();
            PlayerSlotDeclaration slotDeclaration = target.GetComponent<PlayerSlotDeclaration>();
            Component playerInput = FindPlayerInputOnGameObject(target);

            candidate = new FirstGameCameraPlayerCandidate(target, actorDeclaration, slotDeclaration, playerInput, resolutionSource);
            return true;
        }

        private static bool TryValidateCandidate(
            FirstGameCameraPlayerCandidate candidate,
            out FirstGameCameraResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "None";

            if (candidate.GameObject == null)
            {
                failureReason = "MissingPlayerCandidateGameObject";
                return false;
            }

            if (candidate.ActorDeclaration == null)
            {
                failureReason = "MissingPlayerActorDeclaration";
                return false;
            }

            if (candidate.SlotDeclaration == null)
            {
                failureReason = "MissingPlayerSlotDeclaration";
                return false;
            }

            if (candidate.PlayerInput == null)
            {
                failureReason = "MissingPlayerInput";
                return false;
            }

            if (!candidate.HasExpectedActorId && !candidate.HasExpectedPlayerSlotId)
            {
                failureReason = "UnexpectedPlayerIdentity";
                return false;
            }

            if (!candidate.HasExpectedActorId || !candidate.HasExpectedPlayerSlotId)
            {
                failureReason = "DivergentPlayerActorAndSlotDeclaration";
                return false;
            }

            if (!HasPlayerInputActions(candidate.PlayerInput))
            {
                failureReason = "MissingInputActions";
                return false;
            }

            if (!HasExpectedGameplayActionMap(candidate.PlayerInput))
            {
                failureReason = "MissingPlayerActionMap";
                return false;
            }

            resolvedPlayer = new FirstGameCameraResolvedPlayer(
                candidate.GameObject,
                candidate.ActorDeclaration,
                candidate.SlotDeclaration,
                candidate.PlayerInput,
                candidate.ResolutionSource,
                CanonicalIdentitySource,
                resolvedByName: false);
            return true;
        }

        private static bool HasPlayerInputActions(Component playerInput)
        {
            return TryGetPlayerInputActions(playerInput, out _);
        }

        private static bool HasExpectedGameplayActionMap(Component playerInput)
        {
            if (!TryGetPlayerInputActions(playerInput, out Object actions))
            {
                return false;
            }

            System.Reflection.MethodInfo findActionMap = actions.GetType().GetMethod(
                "FindActionMap",
                new[] { typeof(string), typeof(bool) });

            if (findActionMap == null)
            {
                return false;
            }

            object result = findActionMap.Invoke(actions, new object[] { ExpectedGameplayActionMap, false });
            return result != null;
        }

        private static bool TryGetPlayerInputActions(Component playerInput, out Object actions)
        {
            actions = null;
            if (!IsPlayerInputComponent(playerInput))
            {
                return false;
            }

            SerializedObject serializedObject = new SerializedObject(playerInput);
            SerializedProperty actionsProperty = serializedObject.FindProperty(PlayerInputActionsPropertyName);
            if (actionsProperty == null)
            {
                return false;
            }

            actions = actionsProperty.objectReferenceValue;
            return actions != null;
        }

        private static Component FindRelatedPlayerInputComponent(GameObject selectedObject)
        {
            if (selectedObject == null)
            {
                return null;
            }

            Component component = FindPlayerInputOnGameObject(selectedObject);
            if (component != null)
            {
                return component;
            }

            Transform current = selectedObject.transform.parent;
            while (current != null)
            {
                component = FindPlayerInputOnGameObject(current.gameObject);
                if (component != null)
                {
                    return component;
                }

                current = current.parent;
            }

            Component[] children = selectedObject.GetComponentsInChildren<Component>(true);
            for (int i = 0; i < children.Length; i++)
            {
                if (IsPlayerInputComponent(children[i]))
                {
                    return children[i];
                }
            }

            return null;
        }

        private static Component[] FindScenePlayerInputComponents(Scene scene)
        {
            System.Collections.Generic.List<Component> matches = new System.Collections.Generic.List<Component>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Component[] components = root.GetComponentsInChildren<Component>(true);
                for (int i = 0; i < components.Length; i++)
                {
                    if (IsPlayerInputComponent(components[i]))
                    {
                        matches.Add(components[i]);
                    }
                }
            }

            return matches.ToArray();
        }

        private static Component FindPlayerInputOnGameObject(GameObject target)
        {
            if (target == null)
            {
                return null;
            }

            Component[] components = target.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (IsPlayerInputComponent(components[i]))
                {
                    return components[i];
                }
            }

            return null;
        }

        private static bool IsPlayerInputComponent(Component component)
        {
            return component != null &&
                   string.Equals(component.GetType().FullName, PlayerInputTypeFullName, System.StringComparison.Ordinal);
        }

        private static T FindRelatedComponent<T>(GameObject selectedObject) where T : Component
        {
            T component = selectedObject.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            component = selectedObject.GetComponentInParent<T>();
            if (component != null)
            {
                return component;
            }

            return selectedObject.GetComponentInChildren<T>(true);
        }

        private static T[] FindSceneComponents<T>(Scene scene) where T : Component
        {
            System.Collections.Generic.List<T> components = new System.Collections.Generic.List<T>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                components.AddRange(root.GetComponentsInChildren<T>(true));
            }

            return components.ToArray();
        }

        private static bool IsExpectedActorId(string value)
        {
            return string.Equals(value, ExpectedActorIdRaw, System.StringComparison.Ordinal) ||
                   string.Equals(value, ExpectedActorIdDiagnostic, System.StringComparison.Ordinal);
        }

        private static bool IsExpectedPlayerSlotId(string value)
        {
            return string.Equals(value, ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal) ||
                   string.Equals(value, ExpectedPlayerSlotIdDiagnostic, System.StringComparison.Ordinal);
        }

        private static T LoadRequiredAsset<T>(string path) where T : Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Required asset not found. path='{path}'.");
            }

            return asset;
        }

        private static void SetSerialized(Object target, string propertyName, Object value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.objectReferenceValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetSerialized(Object target, string propertyName, string value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.stringValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetSerialized(Object target, string propertyName, int value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.intValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void SetSerialized(Object target, string propertyName, bool value)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            property.boolValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static SerializedProperty FindProperty(Object target, string propertyName)
        {
            if (target == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Cannot set property on null target. property='{propertyName}'.");
                return null;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Serialized property not found. target='{target.GetType().Name}' property='{propertyName}'.");
            }

            return property;
        }

        private static void ValidateObjectReference(Object target, string propertyName, Object expected, string context)
        {
            SerializedProperty property = FindProperty(target, propertyName);
            if (property == null)
            {
                return;
            }

            Object actual = property.objectReferenceValue;
            if (!ReferenceEquals(actual, expected))
            {
                ReportBlockingIssue($"[FIRSTGAME_CAMERA_SETUP] Object reference was not assigned. context='{context}' expected='{FormatObject(expected)}' actual='{FormatObject(actual)}'.");
            }
        }

        private readonly struct FirstGameCameraPlayerCandidate
        {
            internal FirstGameCameraPlayerCandidate(
                GameObject gameObject,
                PlayerActorDeclaration actorDeclaration,
                PlayerSlotDeclaration slotDeclaration,
                Component playerInput,
                string resolutionSource)
            {
                GameObject = gameObject;
                ActorDeclaration = actorDeclaration;
                SlotDeclaration = slotDeclaration;
                PlayerInput = playerInput;
                ResolutionSource = resolutionSource;
            }

            public GameObject GameObject { get; }

            public PlayerActorDeclaration ActorDeclaration { get; }

            public PlayerSlotDeclaration SlotDeclaration { get; }

            public Component PlayerInput { get; }

            public string ResolutionSource { get; }

            internal bool HasExpectedActorId => ActorDeclaration != null && IsExpectedActorId(ActorDeclaration.ActorId.ToString());

            internal bool HasExpectedPlayerSlotId => SlotDeclaration != null && IsExpectedPlayerSlotId(SlotDeclaration.PlayerSlotId.ToString());

            public bool HasExpectedIdentity => HasExpectedActorId && HasExpectedPlayerSlotId;
        }

        private readonly struct FirstGameCameraResolvedPlayer
        {
            public FirstGameCameraResolvedPlayer(
                GameObject gameObject,
                PlayerActorDeclaration actorDeclaration,
                PlayerSlotDeclaration slotDeclaration,
                Component playerInput,
                string resolutionSource,
                string identitySource,
                bool resolvedByName)
            {
                GameObject = gameObject;
                ActorDeclaration = actorDeclaration;
                SlotDeclaration = slotDeclaration;
                PlayerInput = playerInput;
                ResolutionSource = resolutionSource;
                IdentitySource = identitySource;
                ResolvedByName = resolvedByName;
            }

            public GameObject GameObject { get; }

            public Transform Transform => GameObject != null ? GameObject.transform : null;

            public PlayerActorDeclaration ActorDeclaration { get; }

            public PlayerSlotDeclaration SlotDeclaration { get; }

            public Component PlayerInput { get; }

            public string ResolutionSource { get; }

            public string IdentitySource { get; }

            public bool ResolvedByName { get; }

            public string PlayerObjectName => GameObject != null ? GameObject.name : "<none>";

            public string ActorId => ActorDeclaration != null ? ActorDeclaration.ActorId.ToString() : "<none>";

            public string PlayerSlotId => SlotDeclaration != null ? SlotDeclaration.PlayerSlotId.ToString() : "<none>";
        }

        private static void ReportBlockingIssue(string message)
        {
            hasBlockingIssue = true;
            Debug.LogError(message);
        }

        private static string FormatObject(Object value)
        {
            return value != null ? value.name : "<missing>";
        }
    }
}
