using UnityEngine;

public class ReceptionCallButton : MonoBehaviour, IInteractable
{
    [SerializeField] private ReceptionManager receptionManager;
    [SerializeField] private PressButtonAnimation physicalButton;
    private bool glowed = false;

    public string InteractionPrompt =>
        "Press E to call the next number";

    public bool CanInteract =>
        GameManager.Instance != null &&
        GameManager.Instance.CurrentPhase == GamePhase.Reception;

    public void Interact()
    {
        if (!CanInteract)
            return;

        // Play physical red button press animation.
        physicalButton?.Press();

        if (!glowed)
        {
            GetComponent<ObjectiveHighlightTarget>()?
            .CompleteObjective();
            glowed = true;
        }

        // Call the current queue number.
        receptionManager?.CallCurrentNumber();
    }
}