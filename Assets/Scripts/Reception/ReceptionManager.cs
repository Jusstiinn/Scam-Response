using TMPro;
using UnityEngine;

public class ReceptionManager : MonoBehaviour
{
    [Header("NPC Route")]
    [SerializeField] private Transform npcSpawnPoint;
    [SerializeField] private Transform[] idlePoints;
    [SerializeField] private Transform receptionPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform playerFollowTarget;

    [Header("Queue Display")]
    [SerializeField] private TMP_Text queueNumberText;

    [Header("Return From Interview")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private Transform interviewReturnPoint;

    private ReceptionNpcController currentNpc;

    private void Start()
    {
        /*
        * Returning from the InterviewRoom.
        * Move the player beside the interview door
        * before the scene fades back in.
        */
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentPhase == GamePhase.ReadyForAnalysis)
        {
            MovePlayerToInterviewReturnPoint();

            // Keep displaying the current case's queue number
            if (queueNumberText != null &&
                GameManager.Instance.CurrentCase != null)
            {
                queueNumberText.text =
                    GameManager.Instance.CurrentCase.queueNumber;
            }

            return;
        }

        PrepareNextCase();
    }

    public void PrepareNextCase()
    {
        if (GameManager.Instance == null ||
            CaseManager.Instance == null)
        {
            Debug.LogError(
                "GameManager or CaseManager is missing."
            );

            return;
        }

        // All cases finished.
        if (CaseManager.Instance.AllCasesCompleted())
        {
            GameManager.Instance.SetPhase(
                GamePhase.Complete
            );

            if (queueNumberText != null)
                queueNumberText.text = "---";

            return;
        }

        ScamCaseData caseData =
            GameManager.Instance.CurrentCase;

        /*
         * If there isn't already an active case,
         * get the next incomplete case sequentially.
         */
        if (caseData == null)
        {
            caseData =
                GetNextIncompleteCaseSequential();

            if (caseData == null)
            {
                Debug.LogWarning(
                    "No incomplete case could be found."
                );

                return;
            }

            GameManager.Instance.StartCase(
                caseData
            );
        }

        SpawnNpc(caseData);
    }

    private ScamCaseData
        GetNextIncompleteCaseSequential()
    {
        foreach (
            ScamCaseData caseData
            in CaseManager.Instance.Cases)
        {
            if (caseData == null)
                continue;

            if (!GameManager.Instance
                .IsCaseCompleted(caseData))
            {
                return caseData;
            }
        }

        return null;
    }

    private void SpawnNpc(
        ScamCaseData caseData)
    {
        if (caseData == null)
            return;

        if (caseData.lobbyNpcPrefab == null)
        {
            Debug.LogError(
                "Lobby NPC Prefab is missing for: " +
                caseData.caseTitle
            );

            return;
        }

        if (npcSpawnPoint == null)
        {
            Debug.LogError(
                "NpcSpawnPoint is not assigned."
            );

            return;
        }

        /*
         * Prevent accidental duplicate NPC.
         */
        if (currentNpc != null)
        {
            Destroy(
                currentNpc.gameObject
            );
        }

        GameObject npcObject =
            Instantiate(
                caseData.lobbyNpcPrefab,
                npcSpawnPoint.position,
                npcSpawnPoint.rotation
            );

        currentNpc = npcObject.GetComponentInChildren<ReceptionNpcController>();

        if (currentNpc == null)
        {
            Debug.LogError(
                caseData.lobbyNpcPrefab.name +
                " does not contain " +
                "ReceptionNpcController."
            );

            Destroy(npcObject);
            return;
        }

        Transform idlePoint =
            GetIdlePoint();

        currentNpc.Configure(
            caseData,
            idlePoint,
            receptionPoint,
            exitPoint,
            playerFollowTarget
        );

        /*
         * Nothing is called yet.
         */
        if (queueNumberText != null)
        {
            queueNumberText.text = "---";
        }

        Debug.Log(
            "Prepared case " +
            caseData.caseId +
            " - " +
            caseData.caseTitle
        );
    }

    private Transform GetIdlePoint()
    {
        if (idlePoints == null ||
            idlePoints.Length == 0)
        {
            return receptionPoint;
        }

        /*
         * Idle location can still be random.
         * Only CASE ORDER is sequential.
         */
        return idlePoints[
            Random.Range(
                0,
                idlePoints.Length
            )
        ];
    }

    public void CallCurrentNumber()
    {
        if (currentNpc == null)
        {
            Debug.LogWarning(
                "There is currently no NPC to call."
            );

            return;
        }

        if (currentNpc.CaseData == null)
            return;

        if (queueNumberText != null)
        {
            queueNumberText.text =
                currentNpc.CaseData
                    .queueNumber;
        }

        currentNpc.OnNumberCalled();

        Debug.Log(
            "Called queue number: " +
            currentNpc.CaseData
                .queueNumber
        );
    }

    private void MovePlayerToInterviewReturnPoint()
    {
        if (playerRoot == null)
        {
            Debug.LogError(
                "ReceptionManager: Player Root is not assigned.",
                this
            );
            return;
        }

        if (interviewReturnPoint == null)
        {
            Debug.LogError(
                "ReceptionManager: Interview Return Point is not assigned.",
                this
            );
            return;
        }

        // Temporarily disable CharacterController for teleport
        if (playerCharacterController != null)
            playerCharacterController.enabled = false;

        // Teleport player outside interview door
        playerRoot.SetPositionAndRotation(
            interviewReturnPoint.position,
            interviewReturnPoint.rotation
        );

        // Re-enable CharacterController
        if (playerCharacterController != null)
            playerCharacterController.enabled = true;

        // Restore normal FPS mouse behaviour
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "Player returned outside interview room."
        );
    }
}