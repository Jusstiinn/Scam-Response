using UnityEngine;

public class NormalResponderBehaviour : MonoBehaviour
{
    private ReceptionNpcController controller;

    public void Initialize(
        ReceptionNpcController npcController)
    {
        controller = npcController;
    }

    public void Begin()
    {
        if (controller == null)
            return;

        controller.MoveToIdlePoint();
    }

    public void OnNumberCalled()
    {
        if (controller == null)
            return;

        controller.MoveToReception();
    }

    public void OnPlayerInteract()
    {
        if (controller == null)
            return;

        if (!controller.HasReachedReception)
            return;

        controller.BeginFollowingPlayer();
    }
}