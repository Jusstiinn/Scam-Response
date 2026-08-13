using System.Collections;
using UnityEngine;

public class HingedDoor : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;

    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2.5f;
    [SerializeField] private bool reverseDirection = false;
    [SerializeField] private bool AffectGlowMaterial = false;

    [Header("Interaction Text")]
    [SerializeField] private string openPrompt = "Press E to open door";
    [SerializeField] private string closePrompt = "Press E to close door";

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool isMoving = false;

    // Text shown by PlayerInteraction
    public string InteractionPrompt
    {
        get
        {
            return isOpen ? closePrompt : openPrompt;
        }
    }

    // Stops interaction while door is moving
    public bool CanInteract => !isMoving;

    private void Start()
    {
        if (doorPivot == null)
        {
            Debug.LogError("Door Pivot is not assigned!", this);
            enabled = false;
            return;
        }

        // Save starting rotation as CLOSED position
        closedRotation = doorPivot.localRotation;

        // Calculate OPEN position
        float direction = reverseDirection ? -1f : 1f;

        openRotation =
            closedRotation *
            Quaternion.Euler(0f, openAngle * direction, 0f);
    }

    // Called by PlayerInteraction when player presses E
    public void Interact()
    {
        if (isMoving)
            return;
            
        if (AffectGlowMaterial)
        {
            GetComponent<ObjectiveHighlightTarget>()?
            .CompleteObjective();
        }

        StartCoroutine(RotateDoor());
    }

    private IEnumerator RotateDoor()
    {
        isMoving = true;

        Quaternion startRotation = doorPivot.localRotation;

        Quaternion targetRotation =
            isOpen ? closedRotation : openRotation;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * openSpeed;

            float smoothProgress =
                Mathf.SmoothStep(0f, 1f, progress);

            doorPivot.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    smoothProgress
                );

            yield return null;
        }

        // Make sure it reaches exact rotation
        doorPivot.localRotation = targetRotation;

        isOpen = !isOpen;
        isMoving = false;
    }
}