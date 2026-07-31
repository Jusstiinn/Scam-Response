using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public enum DoorMotion
    {
        Slide,
        Rotate
    }

    [SerializeField] private DoorMotion motion = DoorMotion.Slide;
    [SerializeField] private Transform movingPart;
    [SerializeField] private Vector3 openLocalPositionOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private Vector3 openLocalEulerOffset = new Vector3(0f, 90f, 0f);
    [SerializeField, Min(0.01f)] private float duration = 0.75f;
    [SerializeField] private bool startsOpen;

    public bool IsOpen { get; private set; }
    public bool IsMoving { get; private set; }

    private Vector3 closedPosition;
    private Quaternion closedRotation;
    private Coroutine movementRoutine;

    private void Awake()
    {
        if (movingPart == null)
            movingPart = transform;

        closedPosition = movingPart.localPosition;
        closedRotation = movingPart.localRotation;

        if (startsOpen)
        {
            ApplyPose(1f);
            IsOpen = true;
        }
    }

    public void Toggle()
    {
        SetOpen(!IsOpen);
    }

    public void Open()
    {
        SetOpen(true);
    }

    public void Close()
    {
        SetOpen(false);
    }

    public void SetOpen(bool shouldOpen)
    {
        if (movementRoutine != null)
            StopCoroutine(movementRoutine);

        movementRoutine = StartCoroutine(MoveDoor(shouldOpen));
    }

    private IEnumerator MoveDoor(bool shouldOpen)
    {
        IsMoving = true;

        Vector3 startPosition = movingPart.localPosition;
        Quaternion startRotation = movingPart.localRotation;

        Vector3 targetPosition = shouldOpen
            ? closedPosition + openLocalPositionOffset
            : closedPosition;

        Quaternion targetRotation = shouldOpen
            ? closedRotation * Quaternion.Euler(openLocalEulerOffset)
            : closedRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            movingPart.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            movingPart.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        movingPart.localPosition = targetPosition;
        movingPart.localRotation = targetRotation;
        IsOpen = shouldOpen;
        IsMoving = false;
        movementRoutine = null;
    }

    private void ApplyPose(float openAmount)
    {
        movingPart.localPosition = closedPosition + openLocalPositionOffset * openAmount;
        movingPart.localRotation = closedRotation *
                                   Quaternion.Euler(openLocalEulerOffset * openAmount);
    }
}
