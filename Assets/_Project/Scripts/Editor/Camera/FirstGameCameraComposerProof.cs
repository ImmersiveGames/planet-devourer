using System;
using System.Collections.Generic;
using System.Linq;
using Immersive.Framework.Camera;
using Immersive.Framework.CameraAuthoring;
using System.Reflection;
using Immersive.Framework.PlayerAuthoring;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.Editor.Camera
{
    /// <summary>
    /// FIRSTGAME-only proof helper for the official CameraComposer Product Surface.
    /// This is editor-only consumer setup/proof code. It does not define framework contracts,
    /// does not create runtime authority and does not use Camera.main.
    /// </summary>
    public static class FirstGameCameraComposerProof
    {
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";
        private const string ExpectedActorId = "player.actor";
        private const string ExpectedPlayerSlotId = "player.1";
        private const string ProofRigName = "FIRSTGAME_CameraComposerRig";
        private const int ProofPriority = 100;

        [MenuItem("FIRSTGAME/Immersive Framework/Camera Composer Proof/Configure Gameplay CameraComposer Proof")]
        public static void ConfigureGameplayCameraComposerProof()
        {
            Scene scene = EnsureGameplaySceneOpen();
            if (!scene.IsValid())
            {
                Debug.LogError("[FIRSTGAME][CameraComposerProof] status='Failed' reason='gameplay-scene-not-open'");
                return;
            }

            if (!TryResolvePlayerComposer(scene, out PlayerComposer playerComposer, out string playerIssue))
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' reason='{playerIssue}'");
                return;
            }

            if (!TryResolveGameplayCamera(scene, out UnityEngine.Camera unityCamera, out string cameraIssue))
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' reason='{cameraIssue}'");
                return;
            }

            CameraComposer cameraComposer = ResolveOrCreateCameraComposer(scene, playerComposer);
            ConfigureCameraComposer(cameraComposer, playerComposer, unityCamera);

            CameraComposerProofUtilityResult validate = InvokeCameraComposerValidate(cameraComposer);
            if (!validate.Succeeded)
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' step='validate' reason='{validate.BlockingIssue}'");
                return;
            }

            CameraComposerProofUtilityResult firstApply = InvokeCameraComposerApplyOrRebuild(cameraComposer);
            if (!firstApply.Succeeded)
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' step='first-apply' reason='{firstApply.BlockingIssue}'");
                return;
            }

            CameraComposerProofUtilityResult secondApply = InvokeCameraComposerApplyOrRebuild(cameraComposer);
            if (!secondApply.Succeeded)
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' step='second-apply' reason='{secondApply.BlockingIssue}'");
                return;
            }

            if (secondApply.CreatedCount != 0)
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' step='idempotency' reason='second-apply-created-new-objects' created='{secondApply.CreatedCount}'");
                return;
            }

            if (secondApply.BlockedCount != 0)
            {
                Debug.LogError($"[FIRSTGAME][CameraComposerProof] status='Failed' step='idempotency' reason='second-apply-blocked' blocked='{secondApply.BlockedCount}'");
                return;
            }

            if (cameraComposer.LastResolvedFollowTarget != playerComposer.CameraTarget)
            {
                Debug.LogError("[FIRSTGAME][CameraComposerProof] status='Failed' step='target-resolution' reason='follow-target-not-player-composer-camera-target'");
                return;
            }

            if (cameraComposer.LastResolvedLookAtTarget != playerComposer.LookAtTarget)
            {
                Debug.LogError("[FIRSTGAME][CameraComposerProof] status='Failed' step='target-resolution' reason='look-at-target-not-player-composer-look-at-target'");
                return;
            }

            if (cameraComposer.CinemachineCamera == null)
            {
                Debug.LogError("[FIRSTGAME][CameraComposerProof] status='Failed' step='cinemachine' reason='cinemachine-camera-not-materialized'");
                return;
            }

            if (cameraComposer.CinemachineCamera.Follow != playerComposer.CameraTarget)
            {
                Debug.LogError("[FIRSTGAME][CameraComposerProof] status='Failed' step='cinemachine' reason='cinemachine-follow-target-mismatch'");
                return;
            }

            if (cameraComposer.CinemachineCamera.LookAt != playerComposer.LookAtTarget)
            {
                Debug.LogError("[FIRSTGAME][CameraComposerProof] status='Failed' step='cinemachine' reason='cinemachine-look-at-target-mismatch'");
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeObject = cameraComposer.gameObject;

            Debug.Log($"[FIRSTGAME][CameraComposerProof] status='Succeeded' camera='{cameraComposer.name}' playerActorId='{playerComposer.ActorId}' playerSlotId='{playerComposer.PlayerSlotId}' followTarget='{playerComposer.CameraTarget.name}' lookAtTarget='{playerComposer.LookAtTarget.name}' createdFirst='{firstApply.CreatedCount}' createdSecond='{secondApply.CreatedCount}' blockedSecond='{secondApply.BlockedCount}' resolvedByName='False'");
        }

        private static Scene EnsureGameplaySceneOpen()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid() && activeScene.path == GameplayScenePath)
            {
                return activeScene;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return default;
            }

            return EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
        }

        private static bool TryResolvePlayerComposer(Scene scene, out PlayerComposer playerComposer, out string issue)
        {
            playerComposer = null;
            issue = string.Empty;

            List<PlayerComposer> matches = FindSceneComponents<PlayerComposer>(scene)
                .Where(candidate => string.Equals(candidate.ActorId, ExpectedActorId, StringComparison.Ordinal)
                    && string.Equals(candidate.PlayerSlotId, ExpectedPlayerSlotId, StringComparison.Ordinal))
                .ToList();

            if (matches.Count == 0)
            {
                issue = "player-composer-not-found-by-typed-ids";
                return false;
            }

            if (matches.Count > 1)
            {
                issue = $"multiple-player-composers-for-typed-ids:{matches.Count}";
                return false;
            }

            playerComposer = matches[0];
            return true;
        }

        private static bool TryResolveGameplayCamera(Scene scene, out UnityEngine.Camera unityCamera, out string issue)
        {
            unityCamera = null;
            issue = string.Empty;

            List<CinemachineBrain> brains = FindSceneComponents<CinemachineBrain>(scene).ToList();
            if (brains.Count == 0)
            {
                issue = "cinemachine-brain-not-found";
                return false;
            }

            if (brains.Count > 1)
            {
                issue = $"multiple-cinemachine-brains:{brains.Count}";
                return false;
            }

            unityCamera = brains[0].GetComponent<UnityEngine.Camera>();
            if (unityCamera == null)
            {
                issue = "cinemachine-brain-has-no-unity-camera";
                return false;
            }

            return true;
        }

        private static CameraComposer ResolveOrCreateCameraComposer(Scene scene, PlayerComposer playerComposer)
        {
            List<CameraComposer> existingComposers = FindSceneComponents<CameraComposer>(scene)
                .Where(candidate => candidate.PlayerComposer == playerComposer)
                .ToList();

            if (existingComposers.Count == 1)
            {
                return existingComposers[0];
            }

            if (existingComposers.Count > 1)
            {
                throw new InvalidOperationException($"Multiple CameraComposers already reference the FIRSTGAME PlayerComposer: {existingComposers.Count}");
            }

            var rig = new GameObject(ProofRigName);
            SceneManager.MoveGameObjectToScene(rig, scene);
            rig.transform.localPosition = new Vector3(0f, 4f, -10f);
            rig.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
            rig.transform.localScale = Vector3.one;
            Undo.RegisterCreatedObjectUndo(rig, "Create FIRSTGAME CameraComposer proof rig");
            return Undo.AddComponent<CameraComposer>(rig);
        }

        private static void ConfigureCameraComposer(
            CameraComposer cameraComposer,
            PlayerComposer playerComposer,
            UnityEngine.Camera unityCamera)
        {
            var serialized = new SerializedObject(cameraComposer);
            serialized.Update();

            SetSerialized(serialized, "recipe", (UnityEngine.Object)null);
            SetSerialized(serialized, "mode", (int)CameraMode.SinglePlayerFollowCamera);
            SetSerialized(serialized, "ownershipScope", (int)CameraOwnershipScope.SinglePlayer);
            SetSerialized(serialized, "targetSourceKind", (int)CameraTargetSourceKind.PlayerComposer);
            SetSerialized(serialized, "playerComposer", playerComposer);
            SetSerialized(serialized, "explicitFollowTarget", (UnityEngine.Object)null);
            SetSerialized(serialized, "explicitLookAtTarget", (UnityEngine.Object)null);
            SetSerialized(serialized, "followRequirement", (int)CameraTargetRequirement.Required);
            SetSerialized(serialized, "lookAtRequirement", (int)CameraTargetRequirement.Optional);
            SetSerialized(serialized, "priority", ProofPriority);
            SetSerialized(serialized, "unityCamera", unityCamera);
            SetSerialized(serialized, "createUnityCameraIfMissing", false);
            SetSerialized(serialized, "createCinemachineCameraIfMissing", true);
            SetSerialized(serialized, "unityCameraObjectName", "FIRSTGAME Unity Camera");
            SetSerialized(serialized, "cinemachineCameraObjectName", "FIRSTGAME Cinemachine Camera");
            SetSerialized(serialized, "logApplyRebuildDiagnostics", true);

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(cameraComposer);
        }


        private static CameraComposerProofUtilityResult InvokeCameraComposerValidate(CameraComposer composer)
        {
            return InvokeCameraComposerEditorUtility(
                "Validate",
                new[] { typeof(CameraComposer), typeof(bool) },
                composer,
                true);
        }

        private static CameraComposerProofUtilityResult InvokeCameraComposerApplyOrRebuild(CameraComposer composer)
        {
            return InvokeCameraComposerEditorUtility(
                "ApplyOrRebuild",
                new[] { typeof(CameraComposer), typeof(bool), typeof(bool) },
                composer,
                true,
                true);
        }

        private static CameraComposerProofUtilityResult InvokeCameraComposerEditorUtility(
            string methodName,
            Type[] parameterTypes,
            params object[] arguments)
        {
            Type utilityType = FindTypeByFullName("Immersive.Framework.Editor.CameraAuthoring.CameraComposerApplyRebuildUtility");
            if (utilityType == null)
            {
                return CameraComposerProofUtilityResult.Failed("camera-composer-editor-utility-not-found");
            }

            MethodInfo method = utilityType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, parameterTypes, null);
            if (method == null)
            {
                return CameraComposerProofUtilityResult.Failed($"camera-composer-editor-utility-method-not-found:{methodName}");
            }

            try
            {
                object result = method.Invoke(null, arguments);
                return CameraComposerProofUtilityResult.FromResultObject(result);
            }
            catch (TargetInvocationException exception)
            {
                string issue = exception.InnerException != null ? exception.InnerException.Message : exception.Message;
                return CameraComposerProofUtilityResult.Failed($"camera-composer-editor-utility-threw:{issue}");
            }
        }

        private static Type FindTypeByFullName(string typeFullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeFullName, false);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static IEnumerable<T> FindSceneComponents<T>(Scene scene)
            where T : Component
        {
            if (!scene.IsValid())
            {
                yield break;
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (T component in root.GetComponentsInChildren<T>(true))
                {
                    if (component != null)
                    {
                        yield return component;
                    }
                }
            }
        }

        private static void SetSerialized(SerializedObject serialized, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
        }

        private static void SetSerialized(SerializedObject serialized, string propertyName, int value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.intValue = value;
            }
        }

        private static void SetSerialized(SerializedObject serialized, string propertyName, bool value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.boolValue = value;
            }
        }

        private static void SetSerialized(SerializedObject serialized, string propertyName, string value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value ?? string.Empty;
            }
        }

        private readonly struct CameraComposerProofUtilityResult
        {
            private CameraComposerProofUtilityResult(bool succeeded, string blockingIssue, int createdCount, int blockedCount)
            {
                Succeeded = succeeded;
                BlockingIssue = string.IsNullOrWhiteSpace(blockingIssue) ? string.Empty : blockingIssue.Trim();
                CreatedCount = createdCount;
                BlockedCount = blockedCount;
            }

            public bool Succeeded { get; }

            public string BlockingIssue { get; }

            public int CreatedCount { get; }

            public int BlockedCount { get; }

            public static CameraComposerProofUtilityResult Failed(string issue)
            {
                return new CameraComposerProofUtilityResult(false, issue, 0, 1);
            }

            public static CameraComposerProofUtilityResult FromResultObject(object result)
            {
                if (result == null)
                {
                    return Failed("camera-composer-editor-utility-returned-null");
                }

                Type resultType = result.GetType();
                bool succeeded = ReadBool(resultType, result, "Succeeded");
                string blockingIssue = ReadString(resultType, result, "BlockingIssue");
                int createdCount = ReadInt(resultType, result, "CreatedCount");
                int blockedCount = ReadInt(resultType, result, "BlockedCount");
                return new CameraComposerProofUtilityResult(succeeded, blockingIssue, createdCount, blockedCount);
            }

            private static bool ReadBool(Type type, object instance, string propertyName)
            {
                PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                return property != null && property.PropertyType == typeof(bool) && (bool)property.GetValue(instance);
            }

            private static int ReadInt(Type type, object instance, string propertyName)
            {
                PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                return property != null && property.PropertyType == typeof(int) ? (int)property.GetValue(instance) : 0;
            }

            private static string ReadString(Type type, object instance, string propertyName)
            {
                PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                return property != null && property.PropertyType == typeof(string) ? (string)property.GetValue(instance) : string.Empty;
            }
        }

    }
}
