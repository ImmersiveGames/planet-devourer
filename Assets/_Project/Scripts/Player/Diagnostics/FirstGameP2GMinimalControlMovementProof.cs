using System.Collections;
using System.Collections.Generic;
using Immersive.Framework.UnityInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.Player
{
    /// <summary>
    /// P2G consumer proof. Observes the real FIRSTGAME player, game-owned mover and
    /// framework Gate adapter. It never injects input or executes movement.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("FIRSTGAME/Diagnostics/P2G Minimal Control Movement Proof")]
    public sealed class FirstGameP2GMinimalControlMovementProof : MonoBehaviour
    {
        private const string Prefix = "[P2G_FIRSTGAME_MINIMAL_CONTROL_MOVEMENT]";
        private const int EntryTransitionTimeoutFrames = 900;

        [Header("Real FIRSTGAME Player")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private UnityPlayerInputGateAdapter gateAdapter;
        [SerializeField] private MonoBehaviour movementConsumer;
        [SerializeField] private Transform movementTarget;

        [Header("Expected Input")]
        [SerializeField] private string gameplayActionMapName = "Player";
        [SerializeField] private string moveActionName = "Move";

        [Header("Interactive Proof")]
        [SerializeField] private bool runOnStart = true;
        [SerializeField, Min(0.001f)] private float minimumDisplacement = 0.05f;
        [SerializeField, Min(60)] private int movementObservationFrames = 1200;
        [SerializeField] private bool throwOnFailure;

        private IEnumerator Start()
        {
            if (!runOnStart)
            {
                yield break;
            }

            yield return null;
            yield return null;
            yield return RunProof();
        }

        [ContextMenu("Run P2G Minimal Control Movement Proof")]
        public void RunFromContextMenu()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError(
                    $"{Prefix} status='Failed' case='RequiresPlayMode' " +
                    "message='Enter the real FIRSTGAME flow in Play Mode before running this proof.'",
                    this);
                return;
            }

            StartCoroutine(RunProof());
        }

        private IEnumerator RunProof()
        {
            var cases = new List<CaseResult>();

            InputActionMap gameplayMap = ResolveGameplayMap();
            InputAction moveAction = gameplayMap?.FindAction(
                moveActionName,
                false);

            cases.Add(Case(
                "RealPlayerComponentsPresent",
                playerInput != null
                && gateAdapter != null
                && movementConsumer != null
                && movementTarget != null,
                $"playerInput='{playerInput != null}' " +
                $"gateAdapter='{gateAdapter != null}' " +
                $"movementConsumer='{movementConsumer != null}' " +
                $"movementTarget='{movementTarget != null}'"));

            cases.Add(Case(
                "GameOwnedMovementConsumer",
                movementConsumer != null
                && !movementConsumer.GetType().FullName.StartsWith(
                    "Immersive.Framework.",
                    System.StringComparison.Ordinal),
                $"type='{(movementConsumer != null ? movementConsumer.GetType().FullName : "<none>")}'"));

            cases.Add(Case(
                "GameplayMapAndMoveActionResolved",
                gameplayMap != null
                && moveAction != null,
                $"map='{gameplayActionMapName}' mapResolved='{gameplayMap != null}' " +
                $"action='{moveActionName}' actionResolved='{moveAction != null}'"));

            if (playerInput == null
                || gateAdapter == null
                || movementConsumer == null
                || movementTarget == null
                || gameplayMap == null
                || moveAction == null)
            {
                Complete(cases);
                yield break;
            }

            bool entryTransitionReleased = false;
            for (int frame = 0; frame < EntryTransitionTimeoutFrames; frame++)
            {
                gateAdapter.ApplyCurrentGate();

                entryTransitionReleased =
                    !gateAdapter.IsBlockedByAdapter
                    && gameplayMap.enabled;

                if (entryTransitionReleased)
                {
                    break;
                }

                yield return null;
            }

            cases.Add(Case(
                "EntryTransitionGateReleased",
                entryTransitionReleased,
                $"released='{entryTransitionReleased}' " +
                $"blockedByAdapter='{gateAdapter.IsBlockedByAdapter}' " +
                $"adapterStatus='{gateAdapter.LastStatus}' " +
                $"mapEnabled='{gameplayMap.enabled}'"));

            cases.Add(Case(
                "MovementConsumerEnabled",
                movementConsumer.enabled
                && movementConsumer.gameObject.activeInHierarchy,
                $"enabled='{movementConsumer.enabled}' " +
                $"activeInHierarchy='{movementConsumer.gameObject.activeInHierarchy}'"));

            cases.Add(Case(
                "PlayerInputReady",
                playerInput.enabled
                && playerInput.gameObject.activeInHierarchy
                && gameplayMap.enabled,
                $"playerInputEnabled='{playerInput.enabled}' " +
                $"activeInHierarchy='{playerInput.gameObject.activeInHierarchy}' " +
                $"mapEnabled='{gameplayMap.enabled}'"));

            if (!entryTransitionReleased)
            {
                Complete(cases);
                yield break;
            }

            Vector3 startPosition = movementTarget.position;
            bool moveActionObserved = false;
            bool displacementObserved = false;
            float greatestDisplacement = 0f;

            Debug.Log(
                $"{Prefix} status='AwaitingInput' " +
                $"message='Use the configured Move controls now. The proof observes input and real player displacement for up to {movementObservationFrames} frames.'",
                this);

            int safeFrames = Mathf.Max(60, movementObservationFrames);
            for (int frame = 0; frame < safeFrames; frame++)
            {
                if (moveAction.IsPressed()
                    || moveAction.ReadValue<Vector2>().sqrMagnitude > 0.0001f)
                {
                    moveActionObserved = true;
                }

                float displacement =
                    Vector3.Distance(
                        startPosition,
                        movementTarget.position);

                if (displacement > greatestDisplacement)
                {
                    greatestDisplacement = displacement;
                }

                if (greatestDisplacement >= minimumDisplacement)
                {
                    displacementObserved = true;
                }

                if (moveActionObserved && displacementObserved)
                {
                    break;
                }

                yield return null;
            }

            cases.Add(Case(
                "MoveActionObserved",
                moveActionObserved,
                $"action='{moveAction.name}' observed='{moveActionObserved}'"));

            cases.Add(Case(
                "RealPlayerDisplacementObserved",
                displacementObserved,
                $"start='{startPosition}' current='{movementTarget.position}' " +
                $"greatestDisplacement='{greatestDisplacement:F4}' " +
                $"minimum='{minimumDisplacement:F4}'"));

            cases.Add(Case(
                "InputAndMovementCorrelated",
                moveActionObserved
                && displacementObserved,
                $"moveActionObserved='{moveActionObserved}' " +
                $"displacementObserved='{displacementObserved}'"));

            cases.Add(Case(
                "FrameworkGateRemainedAllowed",
                !gateAdapter.IsBlockedByAdapter
                && gameplayMap.enabled,
                $"blockedByAdapter='{gateAdapter.IsBlockedByAdapter}' " +
                $"adapterStatus='{gateAdapter.LastStatus}' " +
                $"mapEnabled='{gameplayMap.enabled}'"));

            int playerInputCount =
                CountPlayerInputsInScene();
            cases.Add(Case(
                "ExactlyOnePlayerInputInGameplayScene",
                playerInputCount == 1,
                $"count='{playerInputCount}'"));

            bool succeeded = Complete(cases);
            if (!succeeded && throwOnFailure)
            {
                throw new System.InvalidOperationException(
                    "P2G FIRSTGAME minimal control and movement proof failed.");
            }
        }

        private InputActionMap ResolveGameplayMap()
        {
            if (playerInput == null || playerInput.actions == null)
            {
                return null;
            }

            return playerInput.actions.FindActionMap(
                gameplayActionMapName,
                false);
        }

        private int CountPlayerInputsInScene()
        {
            PlayerInput[] inputs =
                Object.FindObjectsByType<PlayerInput>(
                    FindObjectsInactive.Include);

            int count = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                PlayerInput input = inputs[i];
                if (input != null
                    && input.gameObject.scene == gameObject.scene)
                {
                    count++;
                }
            }

            return count;
        }

        private static CaseResult Case(
            string name,
            bool passed,
            string detail)
        {
            return new CaseResult(
                name,
                passed,
                detail);
        }

        private bool Complete(
            IReadOnlyList<CaseResult> cases)
        {
            int passed = 0;
            int failed = 0;

            for (int i = 0; i < cases.Count; i++)
            {
                CaseResult result = cases[i];
                if (result.Passed)
                {
                    passed++;
                    Debug.Log(
                        $"{Prefix} case='{result.Name}' status='Passed' detail='{result.Detail}'",
                        this);
                }
                else
                {
                    failed++;
                    Debug.LogError(
                        $"{Prefix} case='{result.Name}' status='Failed' detail='{result.Detail}'",
                        this);
                }
            }

            string status =
                failed == 0
                    ? "Succeeded"
                    : "Failed";

            string summary =
                $"{Prefix} status='{status}' passed='{passed}' failed='{failed}' cases='{cases.Count}'.";

            if (failed == 0)
            {
                Debug.Log(summary, this);
                return true;
            }

            Debug.LogError(summary, this);
            return false;
        }

        private readonly struct CaseResult
        {
            public CaseResult(
                string name,
                bool passed,
                string detail)
            {
                Name = name ?? string.Empty;
                Passed = passed;
                Detail = detail ?? string.Empty;
            }

            public string Name { get; }

            public bool Passed { get; }

            public string Detail { get; }
        }
    }
}
