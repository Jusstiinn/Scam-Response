using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform waitingPoint;
    [SerializeField] private Transform interviewPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private bool spawnOnStart = true;

    private GameObject spawnedNpc;

    private void Start()
    {
        if (spawnOnStart)
            SpawnCurrentCaseNpc();
    }

    public void SpawnCurrentCaseNpc()
    {
        CaseData currentCase = CaseManager.Instance != null
            ? CaseManager.Instance.EnsureCurrentCase()
            : GameSession.Instance?.CurrentCase;

        if (currentCase == null || currentCase.npcPrefab == null)
        {
            Debug.LogWarning("No current case or NPC prefab has been assigned.");
            return;
        }

        if (spawnedNpc != null)
            Destroy(spawnedNpc);

        Transform point = spawnPoint != null ? spawnPoint : transform;
        spawnedNpc = Instantiate(currentCase.npcPrefab, point.position, point.rotation);

        NPCBehaviourController behaviour = spawnedNpc.GetComponent<NPCBehaviourController>();
        if (behaviour != null)
        {
            behaviour.Configure(
                currentCase,
                waitingPoint,
                interviewPoint,
                exitPoint
            );
        }
    }
}
