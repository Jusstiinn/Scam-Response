using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorController doorController;
    [SerializeField] private string prompt = "Press E to open/close door";
    [SerializeField] private bool locked;

    public string InteractionPrompt => locked ? "Door is locked" : prompt;
    public bool CanInteract => !locked && doorController != null && !doorController.IsMoving;

    private void Reset()
    {
        doorController = GetComponent<DoorController>();
    }

    public void Interact()
    {
        if (CanInteract)
            doorController.Toggle();
    }

    public void SetLocked(bool value)
    {
        locked = value;
    }
}
