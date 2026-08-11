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

    private ReceptionNpcController currentNpc;

    private void Start()
    {
        /*
         * If we returned from the InterviewRoom and are now ready
         * for analysis, don't spawn another reception NPC.
         */
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentPhase == GamePhase.ReadyForAnalysis)
        {
            if (queueNumberText != null)
                queueNumberText.text = "---";

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

        currentNpc =
            npcObject.GetComponent<
                ReceptionNpcController>();

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
}