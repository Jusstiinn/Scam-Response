using UnityEngine;

public class InterviewChairInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private InterviewSceneController controller;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chairSittingSound;
    private bool available;
    public string InteractionPrompt => "Press E to sit and begin the interview";
    public bool CanInteract => available;
    public void SetAvailable(bool value) => available = value;
    public void Interact() 
    { 
        if (available) 
        { 
            GetComponent<ObjectiveHighlightTarget>()?
            .CompleteObjective();
            audioSource.PlayOneShot(chairSittingSound);
            available = false; controller.BeginInterview(); 
        } 
    }
}
