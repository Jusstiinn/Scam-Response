using UnityEngine;

public class AnxiousRushBehaviour : MonoBehaviour
{
    private ReceptionNpcController controller;
    private bool dialogueStarted;

    public void Initialize(
        ReceptionNpcController npcController)
    {
        controller = npcController;
    }

    public void Begin()
    {
        if (controller == null)
            return;

        dialogueStarted = false;

        // Rush directly to reception.
        controller.MoveToReception(
            OnReachedReception
        );
    }

    public void OnNumberCalled()
    {
        // Ignored.
        // This NPC enters without waiting
        // for the queue button.
    }

    private void OnReachedReception()
    {
        if (controller == null ||
            dialogueStarted)
        {
            return;
        }

        dialogueStarted = true;

        // Immediately open reception dialogue.
        controller.StartReceptionDialogue(
            OnDialogueFinished
        );
    }

    private void OnDialogueFinished()
    {
        if (controller == null)
            return;

        controller.BeginFollowingPlayer();
    }
}