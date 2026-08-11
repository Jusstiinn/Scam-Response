using UnityEngine;

public class AnalysisConsoleInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private CaseFileManager manager;
    public string InteractionPrompt => "Press E to analyse the case";
    public bool CanInteract => GameManager.Instance != null && GameManager.Instance.CurrentPhase == GamePhase.ReadyForAnalysis;
    public void Interact() { if (CanInteract) { GameManager.Instance.SetPhase(GamePhase.Analysing); manager.OpenCaseFile(); } }
}
