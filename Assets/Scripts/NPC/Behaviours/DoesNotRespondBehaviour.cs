using UnityEngine;

public class DoesNotRespondBehaviour : MonoBehaviour
{
    private ReceptionNpcController controller;

    private bool numberCalled;
    private bool dialogueStarted;

    public void Initialize(
        ReceptionNpcController npcController)
    {
        controller = npcController;
    }

    public void Begin()
    {
        numberCalled = false;
        dialogueStarted = false;

        // NPC remains at the idle location.
    }

    public void OnNumberCalled()
    {
        if (controller == null)
            return;

        numberCalled = true;

        // Do NOT move to reception.
        // Player now has to find this NPC.
        controller.EnableFindNpcInteraction();
    }

    public void OnPlayerInteract()
    {
        // ReceptionNpcInteraction handles the dialogue.
    }
}