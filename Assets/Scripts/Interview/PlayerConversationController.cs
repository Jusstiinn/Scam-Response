using UnityEngine;

public class PlayerConversationController : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Behaviour[] gameplayBehaviours;
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera conversationCamera;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private GameObject interactionCanvas;
    private Vector3 savedPosition; private Quaternion savedRotation;
    public void EnterConversation(Transform point)
    {
        savedPosition = playerRoot.position;
        savedRotation = playerRoot.rotation;

        foreach (var b in gameplayBehaviours)
        {
            if (b != null)
                b.enabled = false;
        }

        // Disable the interaction raycast
        if (playerInteraction != null)
            playerInteraction.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        playerRoot.SetPositionAndRotation(
            point.position,
            point.rotation
        );

        firstPersonCamera.enabled = false;
        conversationCamera.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (interactionCanvas != null)
        interactionCanvas.SetActive(false);
    }
    public void ExitConversation()
    {
        conversationCamera.enabled = false;
        firstPersonCamera.enabled = true;

        playerRoot.SetPositionAndRotation(
            savedPosition,
            savedRotation
        );

        if (characterController != null)
            characterController.enabled = true;

        foreach (var b in gameplayBehaviours)
        {
            if (b != null)
                b.enabled = true;
        }

        if (playerInteraction != null)
            playerInteraction.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (interactionCanvas != null)
        interactionCanvas.SetActive(true);
    }
}
