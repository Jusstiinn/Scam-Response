using System.Collections;
using UnityEngine;

public class PressButtonAnimation : MonoBehaviour
{
    [Header("Button Reference")]
    [Tooltip("Drag the part of the red button that should physically move.")]
    [SerializeField] private Transform buttonTop;

    [Header("Press Settings")]
    [SerializeField] private float pressDistance = 0.03f;
    [SerializeField] private float pressSpeed = 8f;
    [SerializeField] private float holdDuration = 0.08f;

    private Vector3 startLocalPosition;
    private bool isAnimating;

    private void Awake()
    {
        if (buttonTop != null)
        {
            startLocalPosition = buttonTop.localPosition;
        }
        else
        {
            Debug.LogWarning(
                "PressButtonAnimation: Button Top has not been assigned.",
                this
            );
        }
    }

    public void Press()
    {
        if (buttonTop == null)
            return;

        // Prevent multiple press animations from overlapping.
        if (isAnimating)
            return;

        StartCoroutine(PressRoutine());
    }

    private IEnumerator PressRoutine()
    {
        isAnimating = true;

        Vector3 pressedPosition =
            startLocalPosition +
            Vector3.down * pressDistance;

        // ----------------------------------------
        // MOVE BUTTON DOWN
        // ----------------------------------------

        while (Vector3.Distance(
                   buttonTop.localPosition,
                   pressedPosition) > 0.001f)
        {
            buttonTop.localPosition =
                Vector3.MoveTowards(
                    buttonTop.localPosition,
                    pressedPosition,
                    pressSpeed * Time.deltaTime
                );

            yield return null;
        }

        buttonTop.localPosition = pressedPosition;

        // ----------------------------------------
        // HOLD BUTTON DOWN BRIEFLY
        // ----------------------------------------

        yield return new WaitForSeconds(
            holdDuration
        );

        // ----------------------------------------
        // MOVE BUTTON BACK UP
        // ----------------------------------------

        while (Vector3.Distance(
                   buttonTop.localPosition,
                   startLocalPosition) > 0.001f)
        {
            buttonTop.localPosition =
                Vector3.MoveTowards(
                    buttonTop.localPosition,
                    startLocalPosition,
                    pressSpeed * Time.deltaTime
                );

            yield return null;
        }

        buttonTop.localPosition =
            startLocalPosition;

        isAnimating = false;
    }

    private void OnDisable()
    {
        // Make sure the button doesn't remain
        // physically depressed if disabled mid-animation.
        if (buttonTop != null)
        {
            buttonTop.localPosition =
                startLocalPosition;
        }

        isAnimating = false;
    }
}