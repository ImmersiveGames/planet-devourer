using System.Collections;
using UnityEngine;

namespace FirstGame.Gameplay.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class FirstGameG1BLoopProof : MonoBehaviour
    {
        [SerializeField] private FirstGameMinimalLoopObjective objective;
        [SerializeField] private Transform player;
        [SerializeField] private Transform cameraTrackingTarget;
        [SerializeField] private bool runOnStart = true;

        private Vector3 _playerBaseline;
        private int _completionCount;
        private int _restoreCount;

        private void OnEnable()
        {
            if (objective != null)
            {
                objective.Completed += OnObjectiveCompleted;
                objective.Restored += OnObjectiveRestored;
            }
        }

        private void OnDisable()
        {
            if (objective != null)
            {
                objective.Completed -= OnObjectiveCompleted;
                objective.Restored -= OnObjectiveRestored;
            }
        }

        private IEnumerator Start()
        {
            if (!runOnStart)
            {
                yield break;
            }

            yield return null;

            int failed = 0;
            failed += Check(objective != null, "objective-resolved");
            failed += Check(player != null, "player-resolved");
            failed += Check(cameraTrackingTarget != null, "camera-target-resolved");

            if (player != null)
            {
                _playerBaseline = player.position;
            }

            Debug.Log(
                "[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] " +
                $"status='{(failed == 0 ? "Ready" : "Failed")}' " +
                $"failed='{failed}' " +
                "controls='Move to the green goal. R resets the loop. T restarts the current Activity. Escape pauses/resumes.'");
        }

        private void OnObjectiveCompleted(FirstGameMinimalLoopObjective source)
        {
            _completionCount++;
            Debug.Log(
                "[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] " +
                "status='LoopCompleted' " +
                $"completionCount='{_completionCount}' " +
                $"objectiveId='{source.ObjectiveId}' " +
                $"cameraTargetValid='{(cameraTrackingTarget != null)}'.");
        }

        private void OnObjectiveRestored(FirstGameMinimalLoopObjective source)
        {
            _restoreCount++;
            bool playerAtBaseline = player != null && Vector3.Distance(player.position, _playerBaseline) <= 0.25f;

            Debug.Log(
                "[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] " +
                "status='LoopRestored' " +
                $"restoreCount='{_restoreCount}' " +
                $"completionCount='{_completionCount}' " +
                $"playerAtBaseline='{playerAtBaseline}' " +
                $"cameraTargetValid='{(cameraTrackingTarget != null)}' " +
                $"repeatable='{(_completionCount > 0)}'.");
        }

        private static int Check(bool condition, string caseName)
        {
            Debug.Log(
                "[G1B_FIRSTGAME_MINIMAL_PLAYABLE_LOOP] " +
                $"case='{caseName}' result='{(condition ? "Passed" : "Failed")}'.");
            return condition ? 0 : 1;
        }
    }
}
