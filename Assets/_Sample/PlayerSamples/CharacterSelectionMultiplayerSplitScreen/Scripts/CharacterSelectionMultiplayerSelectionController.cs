using Immersive.Framework.PlayerParticipation;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CharacterSelectionMultiplayerSelectionController : MonoBehaviour
{
    [Header("Session")]
    [SerializeField] private PlayerSessionObserver sessionObserver;

    [Header("Player Slots")]
    [SerializeField] private PlayerSlotProfile player1Slot;
    [SerializeField] private PlayerSlotProfile player2Slot;

    [Header("Views")]
    [SerializeField] private GameObject selectionMenuRoot;
    [SerializeField] private GameObject player1SelectionRoot;
    [SerializeField] private GameObject player2SelectionRoot;

    private bool _refreshPending = true;

    private void OnEnable()
    {
        if (sessionObserver != null)
        {
            sessionObserver.Changed += OnSessionChanged;
        }

        // Do not read Session observation from a synchronous change callback.
        // The current mutation may not have finished publishing all runtime evidence yet.
        SetActive(selectionMenuRoot, false);
        SetActive(player1SelectionRoot, false);
        SetActive(player2SelectionRoot, false);
        _refreshPending = true;
    }

    private void Update()
    {
        if (!_refreshPending)
        {
            return;
        }

        // Scoped access can be bound after scene composition. Keep retrying until
        // the first stable observation is available.
        if (RefreshView())
        {
            _refreshPending = false;
        }
    }

    private void OnDisable()
    {
        if (sessionObserver != null)
        {
            sessionObserver.Changed -= OnSessionChanged;
        }

        _refreshPending = false;
    }

    private void OnSessionChanged(PlayerSessionChange change)
    {
        _ = change;

        // Defer observation until Update so the Session mutation that raised the
        // event can finish publishing Host, Actor and gameplay evidence.
        _refreshPending = true;
    }

    private bool RefreshView()
    {
        if (sessionObserver == null ||
            !sessionObserver.TryGetObservation(
                out PlayerSessionScopedObservationSnapshot observation))
        {
            SetActive(selectionMenuRoot, false);
            SetActive(player1SelectionRoot, false);
            SetActive(player2SelectionRoot, false);
            return false;
        }

        bool player1NeedsSelection =
            TryGetSlot(observation, player1Slot, out var player1) &&
            player1.IsJoined &&
            !player1.HasSelectedActor;

        bool player2NeedsSelection =
            TryGetSlot(observation, player2Slot, out var player2) &&
            player2.IsJoined &&
            !player2.HasSelectedActor;

        SetActive(player1SelectionRoot, player1NeedsSelection);
        SetActive(player2SelectionRoot, player2NeedsSelection);

        SetActive(
            selectionMenuRoot,
            player1NeedsSelection || player2NeedsSelection);

        return true;
    }

    private static bool TryGetSlot(
        PlayerSessionScopedObservationSnapshot observation,
        PlayerSlotProfile slotProfile,
        out PlayerSessionScopedSlotObservation result)
    {
        result = default;

        if (observation == null ||
            slotProfile == null ||
            !slotProfile.TryGetPlayerSlotId(
                out var playerSlotId,
                out _))
        {
            return false;
        }

        for (int index = 0; index < observation.Slots.Count; index++)
        {
            PlayerSessionScopedSlotObservation candidate =
                observation.Slots[index];

            if (candidate.Slot.PlayerSlotId == playerSlotId)
            {
                result = candidate;
                return true;
            }
        }

        return false;
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null && target.activeSelf != active)
        {
            target.SetActive(active);
        }
    }
}
