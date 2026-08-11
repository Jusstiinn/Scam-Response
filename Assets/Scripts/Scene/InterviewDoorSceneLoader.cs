using UnityEngine;

public class InterviewDoorSceneLoader : MonoBehaviour, IInteractable
{
    [SerializeField] private string interviewSceneName = "InterviewRoom";
    public string InteractionPrompt => "Press E to enter the interview room";
    public bool CanInteract => GameManager.Instance != null && GameManager.Instance.CurrentCase != null && GameManager.Instance.CurrentPhase == GamePhase.NpcFollowing;
    public void Interact() { if (CanInteract) { GameManager.Instance.SetPhase(GamePhase.Interview); SceneTransitionManager.Instance.LoadScene(interviewSceneName); } }
}
