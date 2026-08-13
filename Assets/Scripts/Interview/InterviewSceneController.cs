using UnityEngine;

public class InterviewSceneController : MonoBehaviour
{
    [SerializeField] private Transform npcChairPoint;
    [SerializeField] private PlayerConversationController playerConversationController;
    [SerializeField] private Transform playerChairPoint;
    [SerializeField] private InterviewChairInteraction playerChairInteraction;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private string lobbySceneName = "Lobby";

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "InterviewSceneController: GameManager.Instance is NULL.",
                this
            );

            return;
        }

        ScamCaseData data =
            GameManager.Instance.CurrentCase;

        if (data == null)
        {
            Debug.LogError(
                "InterviewSceneController: CurrentCase is NULL.",
                this
            );

            return;
        }

        GameObject prefab =
            data.interviewNpcPrefab != null
                ? data.interviewNpcPrefab
                : data.lobbyNpcPrefab;

        if (prefab == null)
        {
            Debug.LogError(
                "InterviewSceneController: No NPC prefab assigned for " +
                data.caseTitle,
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

        // Spawn NPC directly at the chair.
        GameObject go = Instantiate(
            prefab,
            npcChairPoint.position,
            npcChairPoint.rotation
        );

        NpcMumbleAudio mumbleAudio = go.GetComponentInChildren<NpcMumbleAudio>(true);

        if (dialogueUI != null)
        {
            dialogueUI.SetNpcMumbleAudio(mumbleAudio);
        }

        // Find animator bridge anywhere inside the prefab.
        NpcAnimatorBridge anim =
            go.GetComponentInChildren<NpcAnimatorBridge>(true);

        if (anim != null)
        {
            anim.PlaySit();
        }
        else
        {
            Debug.LogWarning(
                "InterviewSceneController: Spawned NPC has no NpcAnimatorBridge.",
                go
            );
        }

        // Player can now interact with their chair.
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
    public void BeginInterview()
    {
        playerConversationController.EnterConversation(playerChairPoint);
        dialogueManager.StartInterview(GameManager.Instance.CurrentCase, EndInterview);
    }
    private void EndInterview()
    {
        GameManager.Instance.SetPhase(
            GamePhase.ReadyForAnalysis
        );

        SceneTransitionManager.Instance.LoadScene(
            lobbySceneName
        );
    }
}
