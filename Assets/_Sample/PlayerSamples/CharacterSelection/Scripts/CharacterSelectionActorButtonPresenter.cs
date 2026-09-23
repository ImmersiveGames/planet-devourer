using Immersive.Framework.Actors;
using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.UI;

public sealed class CharacterSelectionActorButtonPresenter : MonoBehaviour
{
    [Header("Actor Selection")]
    [SerializeField]
    private PlayerSessionSelectActorCommandTrigger selectActorCommand;

    [Header("Presentation")]
    [SerializeField]
    private Text label;

    [SerializeField]
    private Image icon;

    private void OnEnable()
    {
        RefreshPresentation();
    }

    [ContextMenu("Refresh Presentation")]
    public void RefreshPresentation()
    {
        ActorProfile profile =
            selectActorCommand != null
                ? selectActorCommand.ActorProfile
                : null;

        if (label != null)
        {
            label.text =
                profile != null
                    ? profile.DisplayName
                    : string.Empty;
        }

        if (icon != null)
        {
            Sprite sprite =
                profile != null
                    ? profile.Icon
                    : null;

            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }
    }
}