using UnityEngine;

public class PlayerConversationController : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Behaviour[] gameplayBehaviours;
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera conversationCamera;
    private Vector3 savedPosition; private Quaternion savedRotation;
    public void EnterConversation(Transform point)
    {
        savedPosition = playerRoot.position; savedRotation = playerRoot.rotation;
        foreach (var b in gameplayBehaviours) if (b != null) b.enabled = false;
        if (characterController != null) characterController.enabled = false;
        playerRoot.SetPositionAndRotation(point.position, point.rotation);
        firstPersonCamera.enabled = false; conversationCamera.enabled = true;
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }
    public void ExitConversation()
    {
        conversationCamera.enabled = false; firstPersonCamera.enabled = true;
        playerRoot.SetPositionAndRotation(savedPosition, savedRotation);
        if (characterController != null) characterController.enabled = true;
        foreach (var b in gameplayBehaviours) if (b != null) b.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }
}
