using UnityEngine;

public class InterviewDoorSceneLoader : MonoBehaviour, IInteractable
{
    [SerializeField] private string interviewSceneName = "InterviewRoom";
    [SerializeField] private AudioSource doorOpenSound;
    [SerializeField] private AudioClip doorOpenClip;
    private bool glowed = false;
    public string InteractionPrompt => "Press E to enter the interview room";
    public bool CanInteract => GameManager.Instance != null && GameManager.Instance.CurrentCase != null && GameManager.Instance.CurrentPhase == GamePhase.NpcFollowing;
    public void Interact() 
    { 
        if (CanInteract) 
        { 
            doorOpenSound.PlayOneShot(doorOpenClip);
            if (!glowed)
            {
                Debug.Log("Completed");
                GetComponent<ObjectiveHighlightTarget>()?
                .CompleteObjective();
                glowed = true;
            }
            GameManager.Instance.SetPhase(GamePhase.Interview); 
            SceneTransitionManager.Instance.LoadScene(interviewSceneName); 
        } 
    }
}
