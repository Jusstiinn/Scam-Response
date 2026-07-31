using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt = "Press E to interview";
    [SerializeField] private bool available;
    [SerializeField] private bool requiresFinding;

    private CaseData caseData;
    private NPCBehaviourController behaviourController;

    public string InteractionPrompt =>
        requiresFinding ? "Press E to approach victim" : prompt;

    public bool CanInteract => available || requiresFinding;

    public void Configure(CaseData newCaseData, NPCBehaviourController controller)
    {
        caseData = newCaseData;
        behaviourController = controller;
    }

    public void SetAvailable(bool value)
    {
        available = value;
    }

    public void SetRequiresFinding(bool value)
    {
        requiresFinding = value;
    }

    public void Interact()
    {
        if (requiresFinding)
        {
            requiresFinding = false;
            behaviourController?.FoundByPlayer();
            return;
        }

        if (!available || caseData == null || DialogueManager.Instance == null)
            return;

        DialogueManager.Instance.StartInterview(caseData);
    }
}
