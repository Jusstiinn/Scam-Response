using System.Collections;
using UnityEngine;

public class DoubleSlidingDoor : MonoBehaviour
{
    [Header("Door Panels")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float openDistance = 1.2f;
    [SerializeField, Min(0.01f)] private float moveDuration = 0.75f;
    [SerializeField] private Vector3 movementAxis = Vector3.right;

    private Vector3 leftClosedPosition;
    private Vector3 rightClosedPosition;
    private Coroutine moveRoutine;

    public bool IsOpen { get; private set; }
    public bool IsMoving => moveRoutine != null;

    private void Awake()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError(
                "Assign Left Door and Right Door in DoubleSlidingDoor.",
                this
            );

            enabled = false;
            return;
        }

        movementAxis = movementAxis.normalized;

        // Whatever positions the panels have before Play mode
        // are treated as the closed positions.
        leftClosedPosition = leftDoor.localPosition;
        rightClosedPosition = rightDoor.localPosition;

        // Force the door to begin closed.
        leftDoor.localPosition = leftClosedPosition;
        rightDoor.localPosition = rightClosedPosition;
        IsOpen = false;
    }

    public void OpenDoor()
    {
        SetDoorOpen(true);
    }

    public void CloseDoor()
    {
        SetDoorOpen(false);
    }

    public void SetDoorOpen(bool shouldOpen)
    {
        if (!enabled)
            return;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveDoors(shouldOpen));
    }

    private IEnumerator MoveDoors(bool shouldOpen)
    {
        Vector3 leftStart = leftDoor.localPosition;
        Vector3 rightStart = rightDoor.localPosition;

        Vector3 leftTarget = shouldOpen
            ? leftClosedPosition - movementAxis * openDistance
            : leftClosedPosition;

        Vector3 rightTarget = shouldOpen
            ? rightClosedPosition + movementAxis * openDistance
            : rightClosedPosition;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / moveDuration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            leftDoor.localPosition = Vector3.Lerp(
                leftStart,
                leftTarget,
                smoothProgress
            );

            rightDoor.localPosition = Vector3.Lerp(
                rightStart,
                rightTarget,
                smoothProgress
            );

            yield return null;
        }

        leftDoor.localPosition = leftTarget;
        rightDoor.localPosition = rightTarget;

        IsOpen = shouldOpen;
        moveRoutine = null;
    }
}