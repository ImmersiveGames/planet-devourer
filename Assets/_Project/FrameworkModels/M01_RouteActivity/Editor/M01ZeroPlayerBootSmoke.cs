using System;
using System.Collections.Generic;
using System.Reflection;
using Immersive.Framework.Authoring;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkModels.M01
{
    /// <summary>
    /// Read-only M01 integration smoke. It proves that an application with zero
    /// configured Player Slots boots Game Flow without composing Player runtime
    /// modules or creating fallback Player state.
    /// </summary>
    public static class M01ZeroPlayerBootSmoke
    {
        private const string MenuPath =
            "Tools/Immersive Framework/FIRSTGAME/M01/Run Zero Player Boot Smoke";
        private const string RuntimeAssemblyName = "Immersive.Framework.Runtime";
        private const string RuntimeHostTypeName =
            "Immersive.Framework.ApplicationLifecycle.FrameworkRuntimeHost";
        private const string ParticipationModuleTypeName =
            "Immersive.Framework.PlayerParticipation.PlayerParticipationRuntimeHostModule";
        private const string PreparationModuleTypeName =
            "Immersive.Framework.PlayerParticipation.PlayerActorPreparationRuntimeHostModule";
        private const string GameplayModuleTypeName =
            "Immersive.Framework.PlayerParticipation.PlayerGameplayRuntimeHostModule";
        private const string SceneAdmissionModuleTypeName =
            "Immersive.Framework.PlayerParticipation.SceneLocalPlayerAdmissionRuntimeHostModule";
        private const string ApplicationPath =
            "Assets/_Project/FrameworkModels/M01_RouteActivity/Application/GA_M01_RouteActivity.asset";
        private const string StartupSceneName = "M01_Menu";

        [MenuItem(MenuPath)]
        public static void Run()
        {
            var completed = new List<string>();

            try
            {
                AssertTrue(
                    EditorApplication.isPlaying,
                    "M01 Zero Player Boot Smoke requires Play Mode.");

                Type hostType = ResolveRuntimeType(RuntimeHostTypeName);
                Component runtimeHost = ResolveUniqueLoadedRuntimeHost(hostType);
                AssertNotNull(
                    runtimeHost,
                    "FrameworkRuntimeHost was not resolved. Boot did not complete.");
                completed.Add("runtime-host-resolved");

                GameApplicationAsset expectedApplication =
                    AssetDatabase.LoadAssetAtPath<GameApplicationAsset>(
                        ApplicationPath);
                AssertNotNull(
                    expectedApplication,
                    $"M01 Game Application is missing at '{ApplicationPath}'.");
                AssertEqual(
                    0,
                    expectedApplication.LocalPlayerSlotCount,
                    "M01 must remain explicitly configured with zero Local Player Slots.");
                completed.Add("application-has-zero-slots");

                GameApplicationAsset activeApplication =
                    ResolveGameApplication(hostType, runtimeHost);
                AssertSame(
                    expectedApplication,
                    activeApplication,
                    "Runtime Host did not boot the M01 Game Application.");
                completed.Add("m01-application-active");

                AssertModuleCount(
                    runtimeHost,
                    ParticipationModuleTypeName,
                    0);
                AssertModuleCount(
                    runtimeHost,
                    PreparationModuleTypeName,
                    0);
                AssertModuleCount(
                    runtimeHost,
                    GameplayModuleTypeName,
                    0);
                AssertModuleCount(
                    runtimeHost,
                    SceneAdmissionModuleTypeName,
                    0);
                completed.Add("player-runtime-not-composed");

                Scene menuScene =
                    SceneManager.GetSceneByName(StartupSceneName);
                AssertTrue(
                    menuScene.IsValid() &&
                    menuScene.isLoaded,
                    $"Startup Route scene '{StartupSceneName}' is not loaded after boot.");
                completed.Add("startup-route-loaded");

                Debug.Log(
                    "[M01_ZERO_PLAYER_BOOT_SMOKE] " +
                    "status='Passed' cases='5' " +
                    "configuredSlots='0' " +
                    "playerRuntime='NotConfigured' " +
                    "sceneAdmission='NotConfigured' " +
                    $"startupScene='{StartupSceneName}' " +
                    $"completed='{string.Join(",", completed)}'.");
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[M01_ZERO_PLAYER_BOOT_SMOKE] " +
                    "status='Failed' " +
                    $"exception='{exception.GetType().Name}' " +
                    $"message='{Escape(exception.Message)}' " +
                    $"completed='{string.Join(",", completed)}'.");
                throw;
            }
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return EditorApplication.isPlaying;
        }

        private static Type ResolveRuntimeType(string fullName)
        {
            Type type =
                Type.GetType($"{fullName}, {RuntimeAssemblyName}");
            if (type == null)
            {
                throw new InvalidOperationException(
                    $"Runtime type '{fullName}' was not found.");
            }

            return type;
        }

        private static Component ResolveUniqueLoadedRuntimeHost(
            Type hostType)
        {
            UnityEngine.Object[] candidates =
                Resources.FindObjectsOfTypeAll(hostType);
            var loaded = new List<Component>();
            var seen = new HashSet<Component>();

            for (int index = 0;
                 index < candidates.Length;
                 index++)
            {
                Component candidate =
                    candidates[index] as Component;
                if (candidate == null ||
                    !candidate.gameObject.scene.IsValid() ||
                    !candidate.gameObject.scene.isLoaded ||
                    !seen.Add(candidate))
                {
                    continue;
                }

                loaded.Add(candidate);
            }

            if (loaded.Count == 0)
            {
                throw new InvalidOperationException(
                    "FrameworkRuntimeHost was not found in any loaded runtime scene.");
            }

            if (loaded.Count != 1)
            {
                var details =
                    new List<string>(loaded.Count);
                for (int index = 0;
                     index < loaded.Count;
                     index++)
                {
                    Component candidate = loaded[index];
                    details.Add(
                        $"object='{candidate.name}' scene='{candidate.gameObject.scene.name}'");
                }

                throw new InvalidOperationException(
                    $"FrameworkRuntimeHost resolution is ambiguous. candidates='{loaded.Count}' details='{string.Join(";", details)}'.");
            }

            return loaded[0];
        }

        private static GameApplicationAsset ResolveGameApplication(
            Type hostType,
            Component runtimeHost)
        {
            FieldInfo field = hostType.GetField(
                "_gameApplication",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException(
                    hostType.FullName,
                    "_gameApplication");
            }

            return field.GetValue(runtimeHost) as GameApplicationAsset;
        }

        private static void AssertModuleCount(
            Component runtimeHost,
            string moduleTypeName,
            int expectedCount)
        {
            Type moduleType =
                ResolveRuntimeType(moduleTypeName);
            Component[] modules =
                runtimeHost.GetComponents(moduleType);
            AssertEqual(
                expectedCount,
                modules.Length,
                $"Unexpected module count for '{moduleTypeName}'.");
        }

        private static void AssertTrue(
            bool condition,
            string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void AssertNotNull(
            object value,
            string message)
        {
            AssertTrue(value != null, message);
        }

        private static void AssertSame(
            object expected,
            object actual,
            string message)
        {
            if (!ReferenceEquals(expected, actual))
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void AssertEqual<T>(
            T expected,
            T actual,
            string message)
        {
            if (!EqualityComparer<T>.Default.Equals(
                    expected,
                    actual))
            {
                throw new InvalidOperationException(
                    $"{message} expected='{expected}' actual='{actual}'.");
            }
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }
    }
}
