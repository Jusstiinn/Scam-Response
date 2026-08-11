using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;
    [SerializeField] private InteractionPromptUI promptUI;
    private IInteractable current;

    private void Update()
    {
        current = null;
        if (playerCamera != null && Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionDistance, interactionLayers))
            current = hit.collider.GetComponentInParent<IInteractable>();

        if (current != null && current.CanInteract) promptUI?.Show(current.InteractionPrompt); else promptUI?.Hide();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && current != null && current.CanInteract)
            current.Interact();
    }
}
