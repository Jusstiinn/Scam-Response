using UnityEngine;

public class DoesNotRespondBehaviour : MonoBehaviour
{
    private ReceptionNpcController controller;

    public void Initialize(
        ReceptionNpcController npcController)
    {
        controller = npcController;
    }

    public void Begin()
    {

        // NPC remains at the idle location.
    }

    public void OnNumberCalled()
    {
        if (controller == null)
            return;


        // Do NOT move to reception.
        // Player now has to find this NPC.
        controller.EnableFindNpcInteraction();
    }

    public void OnPlayerInteract()
    {
        // ReceptionNpcInteraction handles the dialogue.
    }
}