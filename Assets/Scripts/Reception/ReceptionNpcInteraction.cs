using UnityEngine;

public enum ReceptionNpcInteractionMode { Disabled, FindVictim, BeginFollowing }
public class ReceptionNpcInteraction : MonoBehaviour, IInteractable
{
    private ReceptionNpcController controller;
    private ReceptionNpcInteractionMode mode;
    public string InteractionPrompt => mode == ReceptionNpcInteractionMode.FindVictim ? "Press E to approach the victim" : "Press E to ask the victim to follow you";
    public bool CanInteract => mode != ReceptionNpcInteractionMode.Disabled;
    public void Configure(ReceptionNpcController value) => controller = value;
    public void SetMode(ReceptionNpcInteractionMode value) => mode = value;
    public void Interact() { if (CanInteract) controller.BeginFollowingPlayer(); }
}
