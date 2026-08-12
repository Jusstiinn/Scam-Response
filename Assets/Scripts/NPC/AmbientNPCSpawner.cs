using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientNpcSpawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    [Tooltip("NPC prefabs that can be used as ambient NPCs.")]
    [SerializeField] private GameObject[] npcPrefabs;


    // ==================================================
    // LEFT SIDE
    // ==================================================

    [Header("LEFT SIDE - Spawn Points")]
    [Tooltip("Spawn points located on the LEFT NavMesh.")]
    [SerializeField] private Transform[] leftSpawnPoints;

    [Header("LEFT SIDE - Waypoints")]
    [Tooltip("Pavement waypoints located on the LEFT NavMesh.")]
    [SerializeField] private Transform[] leftWaypoints;

    [Header("LEFT SIDE - Despawn Points")]
    [Tooltip("NPCs can leave through any of these LEFT exit points.")]
    [SerializeField] private Transform[] leftDespawnPoints;


    // ==================================================
    // RIGHT SIDE
    // ==================================================

    [Header("RIGHT SIDE - Spawn Points")]
    [Tooltip("Spawn points located on the RIGHT NavMesh.")]
    [SerializeField] private Transform[] rightSpawnPoints;

    [Header("RIGHT SIDE - Waypoints")]
    [Tooltip("Pavement waypoints located on the RIGHT NavMesh.")]
    [SerializeField] private Transform[] rightWaypoints;

    [Header("RIGHT SIDE - Despawn Points")]
    [Tooltip("NPCs can leave through any of these RIGHT exit points.")]
    [SerializeField] private Transform[] rightDespawnPoints;


    // ==================================================
    // WAYPOINT SETTINGS
    // ==================================================

    [Header("Waypoint Settings")]
    [Tooltip("Minimum number of waypoints each NPC visits.")]
    [SerializeField, Min(1)]
    private int minWaypoints = 2;

    [Tooltip("Maximum number of waypoints each NPC visits.")]
    [SerializeField, Min(1)]
    private int maxWaypoints = 4;


    // ==================================================
    // SPAWN SETTINGS
    // ==================================================

    [Header("Spawn Settings")]
    [SerializeField, Min(1)]
    private int npcCount = 4;

    [Tooltip("Automatically spawn NPCs when the scene starts.")]
    [SerializeField]
    private bool spawnOnStart = true;

    [Tooltip("Time to wait before spawning a replacement NPC.")]
    [SerializeField, Min(0f)]
    private float respawnDelay = 1f;


    // ==================================================
    // INTERNAL
    // ==================================================

    private readonly List<GameObject> spawnedNpcs =
        new List<GameObject>();

    private readonly HashSet<int> activePrefabIndexes =
        new HashSet<int>();


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnNpcs();
        }
    }


    // ==================================================
    // SPAWN NPCS
    // ==================================================

    public void SpawnNpcs()
    {
        ClearSpawnedNpcs();

        if (!ValidateSetup())
            return;

        for (int i = 0; i < npcCount; i++)
        {
            SpawnSingleNpc();
        }
    }


    private void SpawnSingleNpc()
    {
        if (npcPrefabs == null ||
            npcPrefabs.Length == 0)
        {
            return;
        }


        // --------------------------------------------------
        // PICK NPC PREFAB
        // --------------------------------------------------

        List<int> availablePrefabIndexes =
            new List<int>();

        for (int i = 0; i < npcPrefabs.Length; i++)
        {
            if (npcPrefabs[i] != null &&
                !activePrefabIndexes.Contains(i))
            {
                availablePrefabIndexes.Add(i);
            }
        }


        // If all NPCs are already being used,
        // allow a duplicate.
        if (availablePrefabIndexes.Count == 0)
        {
            for (int i = 0; i < npcPrefabs.Length; i++)
            {
                if (npcPrefabs[i] != null)
                {
                    availablePrefabIndexes.Add(i);
                }
            }
        }


        if (availablePrefabIndexes.Count == 0)
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No valid NPC prefabs available.",
                this
            );

            return;
        }


        int randomPrefabListIndex =
            Random.Range(
                0,
                availablePrefabIndexes.Count
            );

        int prefabIndex =
            availablePrefabIndexes[randomPrefabListIndex];

        GameObject prefab =
            npcPrefabs[prefabIndex];


        // --------------------------------------------------
        // CHOOSE LEFT OR RIGHT SIDE
        // --------------------------------------------------

        bool useLeftSide =
            Random.Range(0, 2) == 0;


        Transform[] selectedSpawnPoints =
            useLeftSide
                ? leftSpawnPoints
                : rightSpawnPoints;

        Transform[] selectedWaypoints =
            useLeftSide
                ? leftWaypoints
                : rightWaypoints;

        Transform[] selectedDespawnPoints =
            useLeftSide
                ? leftDespawnPoints
                : rightDespawnPoints;


        // --------------------------------------------------
        // CHECK SELECTED SIDE
        // --------------------------------------------------

        if (selectedSpawnPoints == null ||
            selectedSpawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: The selected side has no spawn points.",
                this
            );

            return;
        }

        if (selectedWaypoints == null ||
            selectedWaypoints.Length == 0)
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: The selected side has no waypoints.",
                this
            );

            return;
        }

        if (selectedDespawnPoints == null ||
            selectedDespawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: The selected side has no despawn points.",
                this
            );

            return;
        }


        // --------------------------------------------------
        // PICK RANDOM SPAWN POINT
        // --------------------------------------------------

        Transform spawnPoint =
            GetRandomValidTransform(
                selectedSpawnPoints
            );

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No valid spawn point found on the selected side.",
                this
            );

            return;
        }


        // --------------------------------------------------
        // SPAWN NPC
        // --------------------------------------------------

        GameObject npc =
            Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        npc.name =
            prefab.name +
            "_Ambient_" +
            (spawnedNpcs.Count + 1);


        spawnedNpcs.Add(npc);
        activePrefabIndexes.Add(prefabIndex);


        // --------------------------------------------------
        // GET / ADD AMBIENT NPC EXIT
        // --------------------------------------------------

        AmbientNpcExit exit =
            npc.GetComponent<AmbientNpcExit>();

        if (exit == null)
        {
            exit =
                npc.AddComponent<AmbientNpcExit>();
        }


        // --------------------------------------------------
        // GIVE NPC ITS SIDE-SPECIFIC ROUTE
        // --------------------------------------------------

        exit.Initialize(
            selectedDespawnPoints,
            this,
            prefabIndex,
            selectedWaypoints,
            minWaypoints,
            maxWaypoints
        );
    }


    // ==================================================
    // RANDOM VALID TRANSFORM
    // ==================================================

    private Transform GetRandomValidTransform(
        Transform[] transforms
    )
    {
        List<Transform> validTransforms =
            new List<Transform>();

        foreach (Transform point in transforms)
        {
            if (point != null)
            {
                validTransforms.Add(point);
            }
        }

        if (validTransforms.Count == 0)
            return null;

        return validTransforms[
            Random.Range(
                0,
                validTransforms.Count
            )
        ];
    }


    // ==================================================
    // NPC DESPAWNED
    // ==================================================

    public void OnNpcDespawned(
        GameObject npc,
        int prefabIndex
    )
    {
        if (npc != null)
        {
            spawnedNpcs.Remove(npc);
        }

        activePrefabIndexes.Remove(prefabIndex);

        StartCoroutine(
            RespawnAfterDelay()
        );
    }


    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(
            respawnDelay
        );

        if (spawnedNpcs.Count < npcCount)
        {
            SpawnSingleNpc();
        }
    }


    // ==================================================
    // CLEAR
    // ==================================================

    public void ClearSpawnedNpcs()
    {
        for (
            int i = spawnedNpcs.Count - 1;
            i >= 0;
            i--
        )
        {
            if (spawnedNpcs[i] != null)
            {
                Destroy(
                    spawnedNpcs[i]
                );
            }
        }

        spawnedNpcs.Clear();
        activePrefabIndexes.Clear();
    }


    // ==================================================
    // VALIDATION
    // ==================================================

    private bool ValidateSetup()
    {
        if (
            npcPrefabs == null ||
            npcPrefabs.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No NPC prefabs have been assigned.",
                this
            );

            return false;
        }


        if (
            leftSpawnPoints == null ||
            leftSpawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No LEFT spawn points have been assigned.",
                this
            );

            return false;
        }


        if (
            rightSpawnPoints == null ||
            rightSpawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No RIGHT spawn points have been assigned.",
                this
            );

            return false;
        }


        if (
            leftWaypoints == null ||
            leftWaypoints.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No LEFT waypoints have been assigned.",
                this
            );

            return false;
        }


        if (
            rightWaypoints == null ||
            rightWaypoints.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No RIGHT waypoints have been assigned.",
                this
            );

            return false;
        }


        if (
            leftDespawnPoints == null ||
            leftDespawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No LEFT despawn points have been assigned.",
                this
            );

            return false;
        }


        if (
            rightDespawnPoints == null ||
            rightDespawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "AmbientNpcSpawner: No RIGHT despawn points have been assigned.",
                this
            );

            return false;
        }


        if (maxWaypoints < minWaypoints)
        {
            maxWaypoints = minWaypoints;
        }


        return true;
    }
}


// ======================================================
// AMBIENT NPC EXIT / WAYPOINT CONTROLLER
// ======================================================

public class AmbientNpcExit : MonoBehaviour
{
    private Transform[] despawnPoints;

    private AmbientNpcSpawner spawner;

    private int prefabIndex;

    private NpcNavAgent navAgent;

    private Transform[] waypoints;

    private int minWaypoints;

    private int maxWaypoints;

    private List<Transform> route =
        new List<Transform>();

    private int currentWaypointIndex;

    private bool initialized;

    private bool despawning;


    // ==================================================
    // INITIALIZE
    // ==================================================

    public void Initialize(
        Transform[] availableDespawnPoints,
        AmbientNpcSpawner npcSpawner,
        int assignedPrefabIndex,
        Transform[] availableWaypoints,
        int minimumWaypoints,
        int maximumWaypoints
    )
    {
        despawnPoints =
            availableDespawnPoints;

        spawner =
            npcSpawner;

        prefabIndex =
            assignedPrefabIndex;

        waypoints =
            availableWaypoints;

        minWaypoints =
            minimumWaypoints;

        maxWaypoints =
            maximumWaypoints;

        navAgent = GetComponent<NpcNavAgent>();

        if (navAgent == null)
        {
            navAgent = GetComponentInChildren<NpcNavAgent>();
        }


        if (navAgent == null)
        {
            Debug.LogWarning(
                "AmbientNpcExit: NPC does not have an NpcNavAgent component.",
                this
            );

            return;
        }


        initialized = true;


        // Create random route using ONLY
        // waypoints from the NPC's side.
        CreateRandomRoute();


        // Start walking.
        MoveToNextPoint();
    }


    // ==================================================
    // CREATE RANDOM ROUTE
    // ==================================================

    private void CreateRandomRoute()
    {
        route.Clear();


        if (
            waypoints == null ||
            waypoints.Length == 0
        )
        {
            return;
        }


        List<Transform> availableWaypoints =
            new List<Transform>();


        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                availableWaypoints.Add(
                    waypoint
                );
            }
        }


        if (availableWaypoints.Count == 0)
        {
            return;
        }


        // Pick how many waypoints this NPC
        // will visit.
        int amountToVisit =
            Random.Range(
                minWaypoints,
                maxWaypoints + 1
            );


        amountToVisit =
            Mathf.Min(
                amountToVisit,
                availableWaypoints.Count
            );


        // Pick unique random waypoints.
        for (
            int i = 0;
            i < amountToVisit;
            i++
        )
        {
            int randomIndex =
                Random.Range(
                    0,
                    availableWaypoints.Count
                );


            route.Add(
                availableWaypoints[randomIndex]
            );


            availableWaypoints.RemoveAt(
                randomIndex
            );
        }


        currentWaypointIndex = 0;
    }


    // ==================================================
    // MOVE TO NEXT WAYPOINT
    // ==================================================

    private void MoveToNextPoint()
    {
        if (despawning)
            return;


        if (navAgent == null)
            return;


        // Still have waypoints to visit.
        if (
            currentWaypointIndex <
            route.Count
        )
        {
            Transform nextWaypoint =
                route[currentWaypointIndex];


            currentWaypointIndex++;


            if (nextWaypoint != null)
            {
                navAgent.MoveTo(
                    nextWaypoint,
                    MoveToNextPoint
                );

                return;
            }


            // Skip invalid waypoint.
            MoveToNextPoint();

            return;
        }


        // All waypoints completed.
        // Now choose an exit on the SAME side.
        MoveToRandomDespawnPoint();
    }


    // ==================================================
    // MOVE TO RANDOM DESPAWN POINT
    // ==================================================

    private void MoveToRandomDespawnPoint()
    {
        if (
            navAgent == null ||
            despawnPoints == null ||
            despawnPoints.Length == 0
        )
        {
            Despawn();

            return;
        }


        Transform destination =
            GetRandomValidDespawnPoint();


        if (destination == null)
        {
            Despawn();

            return;
        }


        navAgent.MoveTo(
            destination,
            OnReachedDespawnPoint
        );
    }


    // ==================================================
    // GET RANDOM DESPAWN
    // ==================================================

    private Transform GetRandomValidDespawnPoint()
    {
        List<Transform> validPoints =
            new List<Transform>();


        foreach (Transform point in despawnPoints)
        {
            if (point != null)
            {
                validPoints.Add(point);
            }
        }


        if (validPoints.Count == 0)
            return null;


        return validPoints[
            Random.Range(
                0,
                validPoints.Count
            )
        ];
    }


    // ==================================================
    // ARRIVED AT DESPAWN
    // ==================================================

    private void OnReachedDespawnPoint()
    {
        Despawn();
    }


    // ==================================================
    // BACKUP DISTANCE CHECK
    // ==================================================

    private void Update()
    {
        if (
            !initialized ||
            despawning ||
            navAgent == null
        )
        {
            return;
        }


        // Only perform the backup check
        // once all waypoints are complete.
        if (
            currentWaypointIndex <
            route.Count
        )
        {
            return;
        }


        if (
            despawnPoints == null ||
            despawnPoints.Length == 0
        )
        {
            return;
        }


        // Check all possible exit points.
        foreach (Transform point in despawnPoints)
        {
            if (point == null)
                continue;


            float distance =
                Vector3.Distance(
                    transform.position,
                    point.position
                );


            if (distance <= 1f)
            {
                Despawn();

                return;
            }
        }
    }


    // ==================================================
    // DESPAWN
    // ==================================================

    private void Despawn()
    {
        if (despawning)
            return;


        despawning = true;


        if (spawner != null)
        {
            spawner.OnNpcDespawned(
                gameObject,
                prefabIndex
            );
        }


        Destroy(gameObject);
    }
}