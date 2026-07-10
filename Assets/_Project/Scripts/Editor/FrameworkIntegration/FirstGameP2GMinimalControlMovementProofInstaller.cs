using System;
using System.Linq;
using Immersive.Framework.PlayerAuthoring;
using Immersive.Framework.UnityInput;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkIntegration.Editor
{
    /// <summary>
    /// Installs the P2G consumer proof on the existing real FIRSTGAME player.
    /// It creates no PlayerInput, mover, PlayerComposer, Gate adapter or gameplay authority.
    /// </summary>
    public static class FirstGameP2GMinimalControlMovementProofInstaller
    {
        private const string MenuPath =
            "FIRSTGAME/Immersive Framework/Player/P2G Install Minimal Control Movement Proof";

        private const string GameplayScenePath =
            "Assets/_Project/Scenes/Gameplay/FG_Gameplay.unity";

        private const string FixtureTypeName =
            "FirstGame.Player.FirstGameP2GMinimalControlMovementProof";

        private const string ExpectedMoverTypeName =
            "FirstGamePlayerMover";

        [MenuItem(MenuPath)]
        public static void Install()
        {
            try
            {
                SceneAsset sceneAsset =
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(
                        GameplayScenePath);

                if (sceneAsset == null)
                {
                    throw new InvalidOperationException(
                        $"FIRSTGAME gameplay scene is missing at '{GameplayScenePath}'.");
                }

                Scene scene =
                    EditorSceneManager.OpenScene(
                        GameplayScenePath,
                        OpenSceneMode.Single);

                PlayerComposer composer =
                    FindSingleInScene<PlayerComposer>(
                        scene,
                        nameof(PlayerComposer));

                PlayerInput playerInput =
                    composer.GetComponent<PlayerInput>();

                if (playerInput == null)
                {
                    throw new InvalidOperationException(
                        "The real PlayerComposer object has no PlayerInput.");
                }

                UnityPlayerInputGateAdapter gateAdapter =
                    composer.GetComponent<UnityPlayerInputGateAdapter>();

                if (gateAdapter == null)
                {
                    throw new InvalidOperationException(
                        "The real PlayerComposer object has no UnityPlayerInputGateAdapter.");
                }

                MonoBehaviour mover =
                    composer.GetComponents<MonoBehaviour>()
                        .SingleOrDefault(component =>
                            component != null
                            && string.Equals(
                                component.GetType().Name,
                                ExpectedMoverTypeName,
                                StringComparison.Ordinal));

                if (mover == null)
                {
                    throw new InvalidOperationException(
                        $"The real PlayerComposer object does not contain exactly one '{ExpectedMoverTypeName}' game-owned movement consumer.");
                }

                Type fixtureType =
                    ResolveFixtureType();

                Component fixture =
                    composer.GetComponent(
                        fixtureType);

                if (fixture == null)
                {
                    fixture =
                        Undo.AddComponent(
                            composer.gameObject,
                            fixtureType);
                }

                ConfigureFixture(
                    fixture,
                    playerInput,
                    gateAdapter,
                    mover,
                    composer.transform);

                EditorUtility.SetDirty(
                    composer.gameObject);
                EditorUtility.SetDirty(
                    fixture);
                EditorSceneManager.MarkSceneDirty(
                    scene);

                if (!EditorSceneManager.SaveScene(
                    scene,
                    GameplayScenePath))
                {
                    throw new InvalidOperationException(
                        $"Could not save P2G proof into '{GameplayScenePath}'.");
                }

                ValidateSavedScene();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log(
                    "[P2G_FIRSTGAME_MINIMAL_CONTROL_MOVEMENT_SETUP] " +
                    "status='Succeeded' " +
                    $"scene='{GameplayScenePath}' " +
                    $"playerActorId='{composer.ActorId}' " +
                    $"playerSlotId='{composer.PlayerSlotId}' " +
                    $"movementConsumer='{mover.GetType().FullName}' " +
                    "message='Enter FIRSTGAME through its normal Menu -> Gameplay flow, then use Move controls when AwaitingInput is logged.'");
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[P2G_FIRSTGAME_MINIMAL_CONTROL_MOVEMENT_SETUP] " +
                    "status='Failed' " +
                    $"exception='{exception.GetType().Name}' " +
                    $"message='{Escape(exception.Message)}'.");
                throw;
            }
        }

        private static Type ResolveFixtureType()
        {
            Type fixtureType =
                TypeCache.GetTypesDerivedFrom<MonoBehaviour>()
                    .SingleOrDefault(type =>
                        string.Equals(
                            type.FullName,
                            FixtureTypeName,
                            StringComparison.Ordinal));

            if (fixtureType == null)
            {
                throw new InvalidOperationException(
                    $"Runtime fixture type '{FixtureTypeName}' was not found after compilation.");
            }

            return fixtureType;
        }

        private static T FindSingleInScene<T>(
            Scene scene,
            string diagnosticName)
            where T : Component
        {
            T[] matches =
                UnityEngine.Object.FindObjectsByType<T>(
                    FindObjectsInactive.Include)
                .Where(component =>
                    component != null
                    && component.gameObject.scene == scene)
                .ToArray();

            if (matches.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Expected exactly one {diagnosticName} in '{scene.name}', found '{matches.Length}'.");
            }

            return matches[0];
        }

        private static void ConfigureFixture(
            Component fixture,
            PlayerInput playerInput,
            UnityPlayerInputGateAdapter gateAdapter,
            MonoBehaviour mover,
            Transform movementTarget)
        {
            var serialized =
                new SerializedObject(fixture);
            serialized.Update();

            SetObject(
                serialized,
                "playerInput",
                playerInput);
            SetObject(
                serialized,
                "gateAdapter",
                gateAdapter);
            SetObject(
                serialized,
                "movementConsumer",
                mover);
            SetObject(
                serialized,
                "movementTarget",
                movementTarget);
            SetString(
                serialized,
                "gameplayActionMapName",
                "Player");
            SetString(
                serialized,
                "moveActionName",
                "Move");
            SetBool(
                serialized,
                "runOnStart",
                true);
            SetFloat(
                serialized,
                "minimumDisplacement",
                0.05f);
            SetInt(
                serialized,
                "movementObservationFrames",
                1200);
            SetBool(
                serialized,
                "throwOnFailure",
                false);

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ValidateSavedScene()
        {
            Scene scene =
                EditorSceneManager.OpenScene(
                    GameplayScenePath,
                    OpenSceneMode.Single);

            PlayerComposer composer =
                FindSingleInScene<PlayerComposer>(
                    scene,
                    nameof(PlayerComposer));

            Type fixtureType =
                ResolveFixtureType();

            Component fixture =
                composer.GetComponent(
                    fixtureType);

            if (fixture == null)
            {
                throw new InvalidOperationException(
                    "Saved FIRSTGAME PlayerComposer has no P2G proof fixture.");
            }

            var serialized =
                new SerializedObject(fixture);
            serialized.Update();

            AssertReference(
                serialized,
                "playerInput",
                composer.GetComponent<PlayerInput>());
            AssertReference(
                serialized,
                "gateAdapter",
                composer.GetComponent<UnityPlayerInputGateAdapter>());

            SerializedProperty mover =
                serialized.FindProperty(
                    "movementConsumer");

            if (mover == null
                || mover.objectReferenceValue == null
                || !string.Equals(
                    mover.objectReferenceValue.GetType().Name,
                    ExpectedMoverTypeName,
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Saved P2G movementConsumer reference is missing or does not point to FirstGamePlayerMover.");
            }

            AssertReference(
                serialized,
                "movementTarget",
                composer.transform);
        }

        private static void AssertReference(
            SerializedObject serialized,
            string propertyName,
            UnityEngine.Object expected)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null
                || property.propertyType
                    != SerializedPropertyType.ObjectReference
                || property.objectReferenceValue != expected)
            {
                throw new InvalidOperationException(
                    $"Saved P2G reference '{propertyName}' is missing or incorrect.");
            }
        }

        private static void SetObject(
            SerializedObject serialized,
            string propertyName,
            UnityEngine.Object value)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null
                || property.propertyType
                    != SerializedPropertyType.ObjectReference)
            {
                throw new InvalidOperationException(
                    $"P2G object reference '{propertyName}' was not found.");
            }

            property.objectReferenceValue =
                value;
        }

        private static void SetString(
            SerializedObject serialized,
            string propertyName,
            string value)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null
                || property.propertyType
                    != SerializedPropertyType.String)
            {
                throw new InvalidOperationException(
                    $"P2G string field '{propertyName}' was not found.");
            }

            property.stringValue =
                value ?? string.Empty;
        }

        private static void SetBool(
            SerializedObject serialized,
            string propertyName,
            bool value)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null
                || property.propertyType
                    != SerializedPropertyType.Boolean)
            {
                throw new InvalidOperationException(
                    $"P2G bool field '{propertyName}' was not found.");
            }

            property.boolValue =
                value;
        }

        private static void SetFloat(
            SerializedObject serialized,
            string propertyName,
            float value)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null
                || property.propertyType
                    != SerializedPropertyType.Float)
            {
                throw new InvalidOperationException(
                    $"P2G float field '{propertyName}' was not found.");
            }

            property.floatValue =
                value;
        }

        private static void SetInt(
            SerializedObject serialized,
            string propertyName,
            int value)
        {
            SerializedProperty property =
                serialized.FindProperty(
                    propertyName);

            if (property == null
                || property.propertyType
                    != SerializedPropertyType.Integer)
            {
                throw new InvalidOperationException(
                    $"P2G int field '{propertyName}' was not found.");
            }

            property.intValue =
                value;
        }

        private static string Escape(
            string value)
        {
            return (value ?? string.Empty)
                .Replace("'", "\\'");
        }
    }
}
