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
    public bool HasReachedReception { get; private set; }
    private Transform idlePoint, receptionPoint, exitPoint, followTarget;
    private Vector3 spawnPosition; private Quaternion spawnRotation;
    private Coroutine timeoutRoutine, followRoutine;
    private NormalResponderBehaviour normalBehaviour;
    private AnxiousRushBehaviour anxiousBehaviour;
    private DoesNotRespondBehaviour noResponseBehaviour;

        public void Configure(
        ScamCaseData data,
        Transform idle,
        Transform reception,
        Transform exit,
        Transform follow)
    {
        CaseData = data;

        idlePoint = idle;
        receptionPoint = reception;
        exitPoint = exit;
        followTarget = follow;

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        interaction.Configure(this);

        navigation.SetSpeed(normalSpeed);

        interaction.SetMode(
            ReceptionNpcInteractionMode.Disabled
        );

        SetupBehaviour();
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

    private void Awake()
    {
    normalBehaviour =
        GetComponentInChildren<NormalResponderBehaviour>();

    anxiousBehaviour =
        GetComponentInChildren<AnxiousRushBehaviour>();

    noResponseBehaviour =
        GetComponentInChildren<DoesNotRespondBehaviour>();
    }

    private void SetupBehaviour()
    {
        if (normalBehaviour != null)
            normalBehaviour.enabled = false;

        if (anxiousBehaviour != null)
            anxiousBehaviour.enabled = false;

        if (noResponseBehaviour != null)
            noResponseBehaviour.enabled = false;

        switch (CaseData.behaviourType)
        {
            case NpcBehaviourType.NormalResponder:

                if (normalBehaviour != null)
                {
                    normalBehaviour.enabled = true;
                    normalBehaviour.Initialize(this);
                    normalBehaviour.Begin();
                }

                break;


            case NpcBehaviourType.AnxiousRush:

                if (anxiousBehaviour != null)
                {
                    anxiousBehaviour.enabled = true;
                    anxiousBehaviour.Initialize(this);
                    anxiousBehaviour.Begin();
                }

                break;


            case NpcBehaviourType.DoesNotRespond:

                if (noResponseBehaviour != null)
                {
                    noResponseBehaviour.enabled = true;
                    noResponseBehaviour.Initialize(this);
                    noResponseBehaviour.Begin();
                }

                break;
        }
    }

        public void MoveToReception()
    {
        MoveToReception(null);
    }

    public void MoveToIdlePoint(System.Action onReached = null)
    {
        if (navigation == null ||
            idlePoint == null)
        {
            return;
        }

        navigation.SetSpeed(normalSpeed);

        navigation.MoveTo(
            idlePoint,
            () =>
            {
                onReached?.Invoke();
            }
        );
    }

    public void EnableFindNpcInteraction()
    {
        if (interaction == null)
            return;

        interaction.SetMode(
            ReceptionNpcInteractionMode.FindVictim
        );
    }

    public void MoveToReception(
        System.Action onReached)
    {
        if (navigation == null ||
            receptionPoint == null)
        {
            return;
        }

        HasReachedReception = false;

        navigation.MoveTo(
            receptionPoint,
            () =>
            {
                HasReachedReception = true;
                onReached?.Invoke();
            }
        );
    }

    public void StartReceptionDialogue(System.Action onFinished = null)
    {
        if (CaseData == null)
        {
            Debug.LogError(
                "ReceptionNpcController: CaseData is null.",
                this
            );

            return;
        }

        if (ReceptionDialogueUI.Instance == null)
        {
            Debug.LogError(
                "ReceptionNpcController: ReceptionDialogueUI is missing.",
                this
            );

            return;
        }

        interaction.SetMode(
            ReceptionNpcInteractionMode.Disabled
        );

        ReceptionDialogueUI.Instance.ShowDialogue(
            CaseData.victimName,
            CaseData.receptionDialogue,
            onFinished
        );
    }

    private void StartNormal() { interaction.SetMode(ReceptionNpcInteractionMode.Disabled); StartTimeout(); }
    private void StartUnresponsive() { interaction.SetMode(ReceptionNpcInteractionMode.FindVictim); StartTimeout(); }
    private void EnableFollowing() { interaction.SetMode(ReceptionNpcInteractionMode.BeginFollowing); StartTimeout(); }
    public void OnNumberCalled()
    {
        if (CaseData == null)
            return;

        switch (CaseData.behaviourType)
        {
            case NpcBehaviourType.NormalResponder:
                normalBehaviour?.OnNumberCalled();
                break;

            case NpcBehaviourType.AnxiousRush:
                anxiousBehaviour?.OnNumberCalled();
                break;

            case NpcBehaviourType.DoesNotRespond:
                noResponseBehaviour?.OnNumberCalled();
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
