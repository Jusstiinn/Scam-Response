using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HingedDoor : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 3f;

    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2.5f;

    [Tooltip("Tick this if the door opens in the wrong direction.")]
    [SerializeField] private bool reverseDirection;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen;
    private bool isMoving;

    private void Start()
    {
        if (doorPivot == null)
        {
            Debug.LogError("Door Pivot is not assigned.", this);
            enabled = false;
            return;
        }

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        closedRotation = doorPivot.localRotation;

        float angle = reverseDirection
            ? -openAngle
            : openAngle;

        openRotation =
            closedRotation * Quaternion.Euler(0f, angle, 0f);
    }

    private void Update()
    {
        if (player == null || isMoving)
            return;

        float distance = Vector3.Distance(
            player.position,
            doorPivot.position
        );

        bool pressedE =
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame;

        if (distance <= interactionDistance && pressedE)
        {
            StartCoroutine(RotateDoor());
        }
    }

    private IEnumerator RotateDoor()
    {
        isMoving = true;

        Quaternion startRotation = doorPivot.localRotation;

        Quaternion targetRotation =
            isOpen ? closedRotation : openRotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;

            doorPivot.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                t
            );

            yield return null;
        }

        doorPivot.localRotation = targetRotation;

        isOpen = !isOpen;
        isMoving = false;
    }
}