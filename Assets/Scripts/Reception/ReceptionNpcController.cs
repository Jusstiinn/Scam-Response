using System.Collections;
using UnityEngine;

public class ReceptionNpcController : MonoBehaviour
{
    [SerializeField] private NpcNavAgent navigation;
    [SerializeField] private ReceptionNpcInteraction interaction;
    [SerializeField] private float unattendedWaitSeconds = 18f;
    [SerializeField] private Vector2 returnCooldownRange = new(5f, 10f);
    [SerializeField] private float followRefreshRate = 0.2f;
    [Header("Movement Speeds")]
    [SerializeField] private float normalSpeed = 2f;
    [SerializeField] private float calledSpeed = 3.5f;
    [SerializeField] private float followSpeed = 2.5f;
    public ScamCaseData CaseData { get; private set; }
    private Transform idlePoint, receptionPoint, exitPoint, followTarget;
    private Vector3 spawnPosition; private Quaternion spawnRotation;
    private Coroutine timeoutRoutine, followRoutine;

    public void Configure(ScamCaseData data, Transform idle, Transform reception, Transform exit, Transform follow)
    {
        CaseData = data; idlePoint = idle; receptionPoint = reception; exitPoint = exit; followTarget = follow;
        spawnPosition = transform.position; spawnRotation = transform.rotation; interaction.Configure(this); BeginBehaviour();
    }
        private void BeginBehaviour()
    {
        navigation.SetSpeed(normalSpeed);

        interaction.SetMode(ReceptionNpcInteractionMode.Disabled);

        if (CaseData.behaviourType ==
            NpcBehaviourType.AnxiousRush)
        {
            navigation.MoveTo(receptionPoint, EnableFollowing);
        }
        else
        {
            navigation.MoveTo(idlePoint, CaseData.behaviourType == 
            NpcBehaviourType.DoesNotRespond ? StartUnresponsive : StartNormal);
        }
    }
    private void StartNormal() { interaction.SetMode(ReceptionNpcInteractionMode.Disabled); StartTimeout(); }
    private void StartUnresponsive() { interaction.SetMode(ReceptionNpcInteractionMode.FindVictim); StartTimeout(); }
    private void EnableFollowing() { interaction.SetMode(ReceptionNpcInteractionMode.BeginFollowing); StartTimeout(); }
        public void OnNumberCalled()
    {
        StopTimeout();

        if (CaseData.behaviourType == NpcBehaviourType.DoesNotRespond)
        {
            interaction.SetMode(ReceptionNpcInteractionMode.FindVictim);
            return;
        }

        // Walk faster when their number is called.
        navigation.SetSpeed(calledSpeed);

        navigation.MoveTo(
            receptionPoint,
            EnableFollowing
        );
    }
        public void BeginFollowingPlayer()
    {
        StopTimeout();

        // Change to following speed.
        navigation.SetSpeed(followSpeed);

        GameManager.Instance.SetPhase(
            GamePhase.NpcFollowing
        );

        interaction.SetMode(
            ReceptionNpcInteractionMode.Disabled
        );

        if (followRoutine != null)
            StopCoroutine(followRoutine);

        followRoutine =
            StartCoroutine(FollowRoutine());
    }

    private IEnumerator FollowRoutine()
    {
        while (GameManager.Instance.CurrentPhase == GamePhase.NpcFollowing)
        { navigation.Follow(followTarget); yield return new WaitForSeconds(followRefreshRate); }
    }
    private void StartTimeout() { StopTimeout(); timeoutRoutine = StartCoroutine(TimeoutRoutine()); }
    private void StopTimeout() { if (timeoutRoutine != null) { StopCoroutine(timeoutRoutine); timeoutRoutine = null; } }
    private IEnumerator TimeoutRoutine() { yield return new WaitForSeconds(unattendedWaitSeconds); navigation.MoveTo(exitPoint, () => StartCoroutine(ReturnRoutine())); }
    private IEnumerator ReturnRoutine()
    {
        yield return new WaitForSeconds(Random.Range(returnCooldownRange.x, returnCooldownRange.y));
        navigation.Warp(spawnPosition); transform.rotation = spawnRotation; BeginBehaviour();
    }
}
