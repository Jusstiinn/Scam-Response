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
        Debug.Log("BEGIN BEHAVIOUR CALLED FOR: " + gameObject.name);

        navigation.SetSpeed(normalSpeed);

        interaction.SetMode(
            ReceptionNpcInteractionMode.Disabled
        );

        switch (CaseData.behaviourType)
        {
            // ==========================================
            // CASE 1 - NORMAL
            // ==========================================
            case NpcBehaviourType.NormalResponder:

                navigation.MoveTo(
                    idlePoint,
                    StartNormal
                );

                break;


            // ==========================================
            // CASE 2 - ANXIOUS RUSH
            // ==========================================
            case NpcBehaviourType.AnxiousRush:

                navigation.SetSpeed(calledSpeed);

                navigation.MoveTo(
                    receptionPoint,
                    StartAnxiousDialogue
                );

                break;


            // ==========================================
            // CASE 3 - DOES NOT RESPOND
            // ==========================================
            case NpcBehaviourType.DoesNotRespond:

                navigation.MoveTo(
                    idlePoint,
                    StartUnresponsive
                );

                break;
        }
    }

    private void StartAnxiousDialogue()
    {
        StopTimeout();

        if (ReceptionDialogueUI.Instance == null)
        {
            Debug.LogError(
                "ReceptionNpcController: ReceptionDialogueUI is missing.",
                this
            );

            return;
        }

        ReceptionDialogueUI.Instance.ShowDialogue(
            CaseData.victimName,
            CaseData.receptionDialogue,
            BeginFollowingPlayer
        );
    }
    private void StartNormal() { interaction.SetMode(ReceptionNpcInteractionMode.Disabled); StartTimeout(); }
    private void StartUnresponsive()
    {
        // Wait here until the number is called.
        interaction.SetMode(
            ReceptionNpcInteractionMode.Disabled
        );

        // Do NOT start the unattended timer.
        // This NPC must remain here until called.
        StopTimeout();
    }
    
    private void EnableFollowing() { interaction.SetMode(ReceptionNpcInteractionMode.BeginFollowing); StartTimeout(); }
    public void OnNumberCalled()
    {
        StopTimeout();

        if (CaseData == null)
            return;

        switch (CaseData.behaviourType)
        {
            // ==========================================
            // NORMAL
            // ==========================================
            case NpcBehaviourType.NormalResponder:

                navigation.SetSpeed(calledSpeed);

                navigation.MoveTo(
                    receptionPoint,
                    EnableFollowing
                );

                break;


            // ==========================================
            // ANXIOUS
            // ==========================================
            case NpcBehaviourType.AnxiousRush:

                // Ignore the call button.
                // This NPC already rushed in automatically.
                break;


            // ==========================================
            // DOES NOT RESPOND
            // ==========================================
            case NpcBehaviourType.DoesNotRespond:

                // NPC stays exactly where they are.
                // Player can now go find and speak to them.
                interaction.SetMode(
                    ReceptionNpcInteractionMode.FindVictim
                );

                break;
        }
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
