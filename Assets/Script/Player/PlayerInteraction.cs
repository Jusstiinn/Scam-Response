using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0.1f)] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private InteractionPromptUI promptUI;

    private IInteractable currentInteractable;

    private void Reset()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        FindInteractable();

        if (currentInteractable != null &&
            currentInteractable.CanInteract &&
            Input.GetKeyDown(interactionKey))
        {
            currentInteractable.Interact();
        }
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        if (playerCamera == null)
        {
            promptUI?.Hide();
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayers))
        {
            currentInteractable = hit.collider.GetComponentInParent<IInteractable>();

            if (currentInteractable != null && currentInteractable.CanInteract)
            {
                promptUI?.Show(currentInteractable.InteractionPrompt);
                return;
            }
        }

        promptUI?.Hide();
    }
}
