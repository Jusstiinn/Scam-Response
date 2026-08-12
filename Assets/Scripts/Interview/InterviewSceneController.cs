using UnityEngine;

public class InterviewSceneController : MonoBehaviour
{
    [SerializeField] private Transform npcSpawnPoint;
    [SerializeField] private Transform npcChairPoint;
    [SerializeField] private PlayerConversationController playerConversationController;
    [SerializeField] private Transform playerChairPoint;
    [SerializeField] private InterviewChairInteraction playerChairInteraction;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private string lobbySceneName = "Lobby";

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "InterviewSceneController: GameManager.Instance is NULL. " +
                "Start the game from the Lobby scene.", this);
            return;
        }

        ScamCaseData data = GameManager.Instance.CurrentCase;

        if (data == null)
        {
            Debug.LogError(
                "InterviewSceneController: CurrentCase is NULL. " +
                "A case must be started in the Lobby before entering the Interview scene.",
                this);

            return;
        }

        GameObject prefab =
            data.interviewNpcPrefab != null
                ? data.interviewNpcPrefab
                : data.lobbyNpcPrefab;

        if (prefab == null)
        {
            Debug.LogError(
                "InterviewSceneController: Both interviewNpcPrefab and lobbyNpcPrefab are missing for case: " +
                data.caseTitle,
                this
            );

            return;
        }

        if (npcSpawnPoint == null)
        {
            Debug.LogError(
                "InterviewSceneController: Npc Spawn Point is not assigned.",
                this
            );

            return;
        }

        if (npcChairPoint == null)
        {
            Debug.LogError(
                "InterviewSceneController: Npc Chair Point is not assigned.",
                this
            );

            return;
        }

        GameObject go = Instantiate(
            prefab,
            npcSpawnPoint.position,
            npcSpawnPoint.rotation
        );

        NpcNavAgent nav =
            go.GetComponent<NpcNavAgent>();

        NpcAnimatorBridge anim =
            go.GetComponent<NpcAnimatorBridge>();

        if (nav == null)
        {
            Debug.LogError(
                prefab.name +
                " does not contain NpcNavAgent.",
                go
            );

            Destroy(go);
            return;
        }

        nav.MoveTo(
            npcChairPoint,
            () =>
            {
                anim?.PlaySit();

                if (playerChairInteraction != null)
                {
                    playerChairInteraction.SetAvailable(true);
                }
                else
                {
                    Debug.LogError(
                        "InterviewSceneController: Player Chair Interaction is not assigned.",
                        this
                    );
                }
            }
        );
    }
    public void BeginInterview()
    {
        playerConversationController.EnterConversation(playerChairPoint);
        dialogueManager.StartInterview(GameManager.Instance.CurrentCase, EndInterview);
    }
    private void EndInterview()
    {
        playerConversationController.ExitConversation();
        GameManager.Instance.SetPhase(GamePhase.ReadyForAnalysis);
        SceneTransitionManager.Instance.LoadScene(lobbySceneName);
    }
}
