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
        var data = GameManager.Instance.CurrentCase;
        var prefab = data.interviewNpcPrefab != null ? data.interviewNpcPrefab : data.lobbyNpcPrefab;
        var go = Instantiate(prefab, npcSpawnPoint.position, npcSpawnPoint.rotation);
        var nav = go.GetComponent<NpcNavAgent>(); var anim = go.GetComponent<NpcAnimatorBridge>();
        nav.MoveTo(npcChairPoint, () => { anim?.PlaySit(); playerChairInteraction.SetAvailable(true); });
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
