using UnityEngine;

public enum ReceptionNpcInteractionMode
{
    Disabled,
    FindVictim,
    BeginFollowing
}

public class ReceptionNpcInteraction : MonoBehaviour, IInteractable
{
    private ReceptionNpcController controller;
    private ReceptionNpcInteractionMode mode;

    public string InteractionPrompt
    {
        get
        {
            if (mode == ReceptionNpcInteractionMode.FindVictim)
                return "Press E to get the victim's attention";

            if (mode == ReceptionNpcInteractionMode.BeginFollowing)
                return "Press E to speak with the victim";

            return "";
        }
    }

    public bool CanInteract =>
        mode != ReceptionNpcInteractionMode.Disabled;

    public void Configure(ReceptionNpcController value)
    {
        controller = value;
    }

    public void SetMode(ReceptionNpcInteractionMode value)
    {
        mode = value;

        Debug.Log(
            gameObject.name +
            " interaction mode changed to: " +
            mode
        );
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        if (controller == null)
        {
            Debug.LogError(
                "ReceptionNpcInteraction: Controller is null.",
                this
            );

            return;
        }

        // NPC has already reached reception.
        // Start the VN dialogue instead of following immediately.
        if (mode == ReceptionNpcInteractionMode.BeginFollowing)
        {
            StartReceptionDialogue();
            return;
        }

        // Keep your current FindVictim behaviour for now.
        if (mode == ReceptionNpcInteractionMode.FindVictim)
        {
            StartReceptionDialogue();
            return;
        }
    }

    private void StartReceptionDialogue()
    {
        ScamCaseData caseData = controller.CaseData;

        if (caseData == null)
        {
            Debug.LogError(
                "ReceptionNpcInteraction: CaseData is null.",
                this
            );
            return;
        }

        if (ReceptionDialogueUI.Instance == null)
        {
            Debug.LogError(
                "ReceptionNpcInteraction: ReceptionDialogueUI is missing from the Lobby scene.",
                this
            );
            return;
        }

        SetMode(ReceptionNpcInteractionMode.Disabled);

        // GET THIS NPC'S MUMBLE AUDIO
        NpcMumbleAudio mumbleAudio =
            controller.GetComponentInChildren<NpcMumbleAudio>(true);

        // GIVE IT TO THE RECEPTION DIALOGUE UI
        ReceptionDialogueUI.Instance.SetNpcMumbleAudio(
            mumbleAudio
        );

        // OPEN THE DIALOGUE
        ReceptionDialogueUI.Instance.ShowDialogue(
            caseData.victimName,
            caseData.receptionDialogue,
            controller.BeginFollowingPlayer
        );
    }
}