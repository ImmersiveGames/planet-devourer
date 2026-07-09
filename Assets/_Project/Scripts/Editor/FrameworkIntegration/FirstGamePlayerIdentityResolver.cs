using Immersive.Framework.Actors;
using Immersive.Framework.PlayerSlots;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FirstGame.FrameworkIntegration.Editor
{
    /// <summary>
    /// FIRSTGAME editor-only resolver for the real player identity.
    /// It resolves the player from canonical framework declarations and PlayerInput, never from GameObject.name.
    /// </summary>
    public static class FirstGamePlayerIdentityResolver
    {
        public const string ExpectedActorIdRaw = "firstgame.player";
        public const string ExpectedActorIdDiagnostic = "Actor:firstgame.player";
        public const string ExpectedPlayerSlotIdRaw = "player.1";
        public const string ExpectedPlayerSlotIdDiagnostic = "PlayerSlot:player.1";
        public const string ExpectedGameplayActionMap = "Player";
        public const string CanonicalIdentitySource = "PlayerActorDeclaration+PlayerSlotDeclaration";

        public static bool TryResolveCanonicalPlayer(
            Scene scene,
            GameObject selectedObject,
            out FirstGameResolvedPlayer resolvedPlayer,
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

            if (TryResolveByCoherentPlayerInput(scene, out resolvedPlayer, out failureReason))
            {
                return true;
            }

            return false;
        }

        public static bool HasExpectedGameplayActionMap(PlayerInput playerInput)
        {
            return playerInput != null &&
                   playerInput.actions != null &&
                   playerInput.actions.FindActionMap(ExpectedGameplayActionMap, false) != null;
        }

        private static bool TryResolveFromSelection(
            Scene scene,
            GameObject selectedObject,
            out FirstGameResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "None";

            if (selectedObject == null || selectedObject.scene != scene)
            {
                return false;
            }

            PlayerInput selectedInput = FindRelatedComponent<PlayerInput>(selectedObject);
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

            if (!TryBuildCandidate(target, "Selection", out FirstGamePlayerCandidate candidate, out failureReason))
            {
                return false;
            }

            if (!TryValidateCandidate(candidate, out resolvedPlayer, out failureReason))
            {
                return false;
            }

            return true;
        }

        private static bool TryResolveByActorId(
            Scene scene,
            out FirstGameResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "NoMatchingPlayerActorDeclaration";
            FirstGamePlayerCandidate match = default;
            int matches = 0;

            foreach (PlayerActorDeclaration actorDeclaration in FindSceneComponents<PlayerActorDeclaration>(scene))
            {
                if (actorDeclaration == null || !IsExpectedActorId(actorDeclaration.ActorId.ToString()))
                {
                    continue;
                }

                if (!TryBuildCandidate(actorDeclaration.gameObject, "PlayerActorDeclaration", out FirstGamePlayerCandidate candidate, out string candidateFailure))
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
            out FirstGameResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "NoMatchingPlayerSlotDeclaration";
            FirstGamePlayerCandidate match = default;
            int matches = 0;

            foreach (PlayerSlotDeclaration slotDeclaration in FindSceneComponents<PlayerSlotDeclaration>(scene))
            {
                if (slotDeclaration == null || !IsExpectedPlayerSlotId(slotDeclaration.PlayerSlotId.ToString()))
                {
                    continue;
                }

                if (!TryBuildCandidate(slotDeclaration.gameObject, "PlayerSlotDeclaration", out FirstGamePlayerCandidate candidate, out string candidateFailure))
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
            out FirstGameResolvedPlayer resolvedPlayer,
            out string failureReason)
        {
            resolvedPlayer = default;
            failureReason = "NoCoherentPlayerInputCandidate";
            FirstGamePlayerCandidate match = default;
            int matches = 0;

            foreach (PlayerInput playerInput in FindSceneComponents<PlayerInput>(scene))
            {
                if (playerInput == null)
                {
                    continue;
                }

                if (!TryBuildCandidate(playerInput.gameObject, "PlayerInput", out FirstGamePlayerCandidate candidate, out _))
                {
                    continue;
                }

                if (!candidate.HasCanonicalDeclarations || !candidate.HasExpectedIdentity)
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
            out FirstGamePlayerCandidate candidate,
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
            PlayerInput playerInput = target.GetComponent<PlayerInput>();

            candidate = new FirstGamePlayerCandidate(target, actorDeclaration, slotDeclaration, playerInput, resolutionSource);
            return true;
        }

        private static bool TryValidateCandidate(
            FirstGamePlayerCandidate candidate,
            out FirstGameResolvedPlayer resolvedPlayer,
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

            if (candidate.PlayerInput.actions == null)
            {
                failureReason = "MissingInputActions";
                return false;
            }

            if (!HasExpectedGameplayActionMap(candidate.PlayerInput))
            {
                failureReason = "MissingPlayerActionMap";
                return false;
            }

            resolvedPlayer = new FirstGameResolvedPlayer(
                candidate.GameObject,
                candidate.ActorDeclaration,
                candidate.SlotDeclaration,
                candidate.PlayerInput,
                candidate.ResolutionSource,
                CanonicalIdentitySource,
                resolvedByName: false);
            return true;
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

        private readonly struct FirstGamePlayerCandidate
        {
            internal FirstGamePlayerCandidate(
                GameObject gameObject,
                PlayerActorDeclaration actorDeclaration,
                PlayerSlotDeclaration slotDeclaration,
                PlayerInput playerInput,
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

            public PlayerInput PlayerInput { get; }

            public string ResolutionSource { get; }

            internal bool HasCanonicalDeclarations => ActorDeclaration != null && SlotDeclaration != null;

            internal bool HasExpectedActorId => ActorDeclaration != null && IsExpectedActorId(ActorDeclaration.ActorId.ToString());

            internal bool HasExpectedPlayerSlotId => SlotDeclaration != null && IsExpectedPlayerSlotId(SlotDeclaration.PlayerSlotId.ToString());

            public bool HasExpectedIdentity => HasExpectedActorId && HasExpectedPlayerSlotId;
        }
    }

    public readonly struct FirstGameResolvedPlayer
    {
        public FirstGameResolvedPlayer(
            GameObject gameObject,
            PlayerActorDeclaration actorDeclaration,
            PlayerSlotDeclaration slotDeclaration,
            PlayerInput playerInput,
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

        public PlayerInput PlayerInput { get; }

        public string ResolutionSource { get; }

        public string IdentitySource { get; }

        public bool ResolvedByName { get; }

        public string PlayerObjectName => GameObject != null ? GameObject.name : "<none>";

        public string ActorId => ActorDeclaration != null ? ActorDeclaration.ActorId.ToString() : "<none>";

        public string PlayerSlotId => SlotDeclaration != null ? SlotDeclaration.PlayerSlotId.ToString() : "<none>";

        public bool HasExpectedIdentity =>
            ActorDeclaration != null &&
            SlotDeclaration != null &&
            (string.Equals(ActorId, FirstGamePlayerIdentityResolver.ExpectedActorIdRaw, System.StringComparison.Ordinal) ||
             string.Equals(ActorId, FirstGamePlayerIdentityResolver.ExpectedActorIdDiagnostic, System.StringComparison.Ordinal)) &&
            (string.Equals(PlayerSlotId, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdRaw, System.StringComparison.Ordinal) ||
             string.Equals(PlayerSlotId, FirstGamePlayerIdentityResolver.ExpectedPlayerSlotIdDiagnostic, System.StringComparison.Ordinal));
    }
}
