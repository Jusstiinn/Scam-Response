using UnityEngine;

public class SceneDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string prompt = "Press E to enter";
    [SerializeField] private bool requireCurrentCase = true;

    public string InteractionPrompt => prompt;

    public bool CanInteract =>
        SceneTransitionManager.Instance != null &&
        (!requireCurrentCase ||
         (GameSession.Instance != null && GameSession.Instance.CurrentCase != null));

    public void Interact()
    {
        if (CanInteract && !string.IsNullOrWhiteSpace(targetSceneName))
            SceneTransitionManager.Instance.LoadScene(targetSceneName);
    }
}
