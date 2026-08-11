using UnityEngine;

public class ReceptionCallButton : MonoBehaviour, IInteractable
{
    [SerializeField] private ReceptionManager receptionManager;
    [SerializeField] private PressButtonAnimation physicalButton;

    public string InteractionPrompt =>
        "Press E to call the next case number";

    public bool CanInteract =>
        GameManager.Instance != null &&
        GameManager.Instance.CurrentPhase == GamePhase.Reception;

    public void Interact()
    {
        if (!CanInteract)
            return;

        // Play physical red button press animation.
        physicalButton?.Press();

        // Call the current queue number.
        receptionManager?.CallCurrentNumber();
    }
}