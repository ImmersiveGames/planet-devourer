using Immersive.Framework.PlayerBinding;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.FrameworkProof
{
    /// <summary>
    /// FIRSTGAME-local preflight probe for the accepted Immersive Framework PlayerView/PlayerControl binding chains.
    /// This component proves consumer-side usability only. It does not bind lifecycle, read InputActions, execute gameplay or enable movement.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("FIRSTGAME/Framework Proof/Player Binding Usability Probe")]
    public sealed class FirstGamePlayerBindingUsabilityProbe : MonoBehaviour
    {
        [SerializeField] private PlayerControlBindingTargetBehaviour playerControlBindingTarget;
        [SerializeField] private UnityPlayerInputBridgeTargetBehaviour unityPlayerInputBridgeTarget;
        [SerializeField] private UnityPlayerInputActivationTargetBehaviour unityPlayerInputActivationTarget;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string expectedGameplayActionMap = "Player";
        [SerializeField] private bool runOnStart;

        public bool HasPlayerControlBindingTarget => playerControlBindingTarget != null;

        public bool HasUnityPlayerInputBridgeTarget => unityPlayerInputBridgeTarget != null;

        public bool HasUnityPlayerInputActivationTarget => unityPlayerInputActivationTarget != null;

        public bool HasPlayerInput => playerInput != null;

        public bool HasInputActions => playerInput != null && playerInput.actions != null;

        public bool HasExpectedGameplayActionMap =>
            HasInputActions &&
            !string.IsNullOrWhiteSpace(expectedGameplayActionMap) &&
            playerInput.actions.FindActionMap(expectedGameplayActionMap, false) != null;

        public bool EnablesMovement => false;

        public bool SpawnsActor => false;

        public bool ExecutesGameplayCommand => false;

        private void Reset()
        {
            AutoAssignFromCurrentGameObject();
        }

        private void OnValidate()
        {
            AutoAssignFromCurrentGameObject();
        }

        private void Start()
        {
            AutoAssignFromCurrentGameObject();

            if (runOnStart)
            {
                RunPreflight();
            }
        }

        public void AutoAssignFromCurrentGameObject()
        {
            if (playerControlBindingTarget == null)
            {
                playerControlBindingTarget = GetComponent<PlayerControlBindingTargetBehaviour>();
            }

            if (unityPlayerInputBridgeTarget == null)
            {
                unityPlayerInputBridgeTarget = GetComponent<UnityPlayerInputBridgeTargetBehaviour>();
            }

            if (unityPlayerInputActivationTarget == null)
            {
                unityPlayerInputActivationTarget = GetComponent<UnityPlayerInputActivationTargetBehaviour>();
            }

            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
        }

        public void ConfigureExternalPlayerInput(PlayerInput input, string actionMapName)
        {
            playerInput = input;

            if (!string.IsNullOrWhiteSpace(actionMapName))
            {
                expectedGameplayActionMap = actionMapName;
            }

            AutoAssignFromCurrentGameObject();
        }

        [ContextMenu("Run FIRSTGAME Player Binding Preflight")]
        public void RunPreflight()
        {
            bool frameworkTargets = HasPlayerControlBindingTarget && HasUnityPlayerInputBridgeTarget && HasUnityPlayerInputActivationTarget;
            bool unityInput = HasPlayerInput;
            bool inputActions = HasInputActions;
            bool expectedActionMap = HasExpectedGameplayActionMap;
            bool safeBoundary = !EnablesMovement && !SpawnsActor && !ExecutesGameplayCommand;
            bool succeeded = frameworkTargets && unityInput && inputActions && expectedActionMap && safeBoundary;

            Debug.Log(
                $"[F53A_FIRSTGAME_PLAYER_BINDING_PREFLIGHT] status='{(succeeded ? "Succeeded" : "Failed")}' " +
                $"frameworkTargets='{frameworkTargets}' " +
                $"playerControlBindingTarget='{HasPlayerControlBindingTarget}' " +
                $"unityPlayerInputBridgeTarget='{HasUnityPlayerInputBridgeTarget}' " +
                $"unityPlayerInputActivationTarget='{HasUnityPlayerInputActivationTarget}' " +
                $"playerInput='{unityInput}' " +
                $"playerInputName='{(playerInput != null ? playerInput.name : "<none>")}' " +
                $"inputActions='{inputActions}' " +
                $"expectedGameplayActionMap='{expectedActionMap}' " +
                $"expectedGameplayActionMapName='{(string.IsNullOrWhiteSpace(expectedGameplayActionMap) ? "<none>" : expectedGameplayActionMap)}' " +
                $"movement='{EnablesMovement}' " +
                $"actorSpawning='{SpawnsActor}' " +
                $"gameplayCommandExecution='{ExecutesGameplayCommand}' " +
                $"failureReason='{BuildFailureReason(frameworkTargets, unityInput, inputActions, expectedActionMap, safeBoundary)}'.",
                this);
        }

        private static string BuildFailureReason(
            bool frameworkTargets,
            bool unityInput,
            bool inputActions,
            bool expectedActionMap,
            bool safeBoundary)
        {
            if (!frameworkTargets)
            {
                return "MissingFrameworkTargets";
            }

            if (!unityInput)
            {
                return "MissingPlayerInput";
            }

            if (!inputActions)
            {
                return "MissingInputActions";
            }

            if (!expectedActionMap)
            {
                return "MissingExpectedGameplayActionMap";
            }

            if (!safeBoundary)
            {
                return "BoundaryViolation";
            }

            return "None";
        }
    }
}
