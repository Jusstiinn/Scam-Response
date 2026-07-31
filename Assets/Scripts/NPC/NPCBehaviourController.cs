using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCBehaviourController : MonoBehaviour
{
    [SerializeField] private NPCMovement movement;
    [SerializeField] private NPCWaitingTimer waitingTimer;
    [SerializeField] private NPCInteraction npcInteraction;

    private CaseData caseData;
    private Transform waitingPoint;
    private Transform interviewPoint;
    private Transform exitPoint;
    private Vector3 originalSpawnPosition;
    private Quaternion originalSpawnRotation;

    private void Reset()
    {
        movement = GetComponent<NPCMovement>();
        waitingTimer = GetComponent<NPCWaitingTimer>();
        npcInteraction = GetComponent<NPCInteraction>();
    }

    public void Configure(
        CaseData newCaseData,
        Transform newWaitingPoint,
        Transform newInterviewPoint,
        Transform newExitPoint)
    {
        caseData = newCaseData;
        waitingPoint = newWaitingPoint;
        interviewPoint = newInterviewPoint;
        exitPoint = newExitPoint;

        originalSpawnPosition = transform.position;
        originalSpawnRotation = transform.rotation;

        if (npcInteraction != null)
            npcInteraction.Configure(caseData, this);

        StartBehaviour();
    }

    private void StartBehaviour()
    {
        if (caseData == null)
            return;

        switch (caseData.npcBehaviour)
        {
            case NPCBehaviourType.NormalWaiter:
                MoveToWaitingArea(true);
                break;

            case NPCBehaviourType.Unresponsive:
                MoveToWaitingArea(false);
                break;

            case NPCBehaviourType.AnxiousRush:
                if (interviewPoint != null)
                    movement.MoveTo(interviewPoint, EnableInteraction);
                else
                    EnableInteraction();
                break;
        }
    }

    private void MoveToWaitingArea(bool respondsNormally)
    {
        if (waitingPoint != null)
        {
            movement.MoveTo(waitingPoint, () =>
            {
                if (npcInteraction != null)
                {
                    npcInteraction.SetAvailable(respondsNormally);
                    npcInteraction.SetRequiresFinding(!respondsNormally);
                }

                waitingTimer?.Begin(LeaveTemporarily);
            });
        }
        else
        {
            EnableInteraction();
        }
    }

    public void CallToInterview()
    {
        waitingTimer?.Stop();

        if (interviewPoint != null)
            movement.MoveTo(interviewPoint, EnableInteraction);
        else
            EnableInteraction();
    }

    public void FoundByPlayer()
    {
        if (npcInteraction != null)
            npcInteraction.SetRequiresFinding(false);

        CallToInterview();
    }

    private void EnableInteraction()
    {
        npcInteraction?.SetAvailable(true);
    }

    private void LeaveTemporarily()
    {
        npcInteraction?.SetAvailable(false);

        if (exitPoint != null)
        {
            movement.MoveTo(exitPoint, () =>
            {
                StartCoroutine(ReturnRoutine());
            });
        }
    }

    private IEnumerator ReturnRoutine()
    {
        float cooldown = waitingTimer != null
            ? waitingTimer.GetRandomReturnCooldown()
            : 5f;

        yield return new WaitForSeconds(cooldown);

        movement.Warp(originalSpawnPosition);
        transform.rotation = originalSpawnRotation;
        StartBehaviour();
    }
}
