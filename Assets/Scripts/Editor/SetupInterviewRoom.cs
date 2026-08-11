using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;

public class SetupInterviewRoom
{
    [MenuItem("Tools/Scam Response/Setup Interview Room")]
    public static void Setup()
    {
        // ==================================================
        // 0. CHECK SCENE
        // ==================================================

        if (UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().name != "InterviewRoom")
        {
            Debug.LogError(
                "Open the InterviewRoom scene before running this setup."
            );
            return;
        }

        // ==================================================
        // 1. FIND PLAYER
        // ==================================================

        GameObject player =
            GameObject.Find("PlayerCapsule");

        if (player == null)
        {
            Debug.LogError(
                "PlayerCapsule was not found."
            );
            return;
        }

        CharacterController characterController =
            player.GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError(
                "PlayerCapsule has no CharacterController."
            );
            return;
        }

        // ==================================================
        // 2. FIND FIRST PERSON CAMERA
        // ==================================================

        Camera firstPersonCamera =
            player.GetComponentInChildren<Camera>(true);

        if (firstPersonCamera == null)
        {
            Debug.LogError(
                "No Camera was found inside PlayerCapsule."
            );
            return;
        }

        // ==================================================
        // 3. FIND CHAIRS
        // ==================================================

        GameObject playerChair =
            GameObject.Find("(Msh)OfficeChair(Player)");

        GameObject npcChair =
            GameObject.Find("(Msh)OfficeChair(NPC)");

        if (playerChair == null ||
            npcChair == null)
        {
            Debug.LogError(
                "Could not find one or both interview chairs."
            );
            return;
        }

        // ==================================================
        // 4. CREATE INTERVIEW POINTS PARENT
        // ==================================================

        GameObject points =
            GameObject.Find("InterviewPoints");

        if (points == null)
        {
            points =
                new GameObject("InterviewPoints");
        }

        // ==================================================
        // 5. CREATE NPC SPAWN POINT
        // ==================================================

        Transform npcSpawnPoint =
            CreatePoint(
                points.transform,
                "NpcSpawnPoint"
            );

        if (npcSpawnPoint.localPosition == Vector3.zero)
        {
            npcSpawnPoint.position =
                npcChair.transform.position +
                npcChair.transform.forward * 2.5f;

            npcSpawnPoint.rotation =
                npcChair.transform.rotation;
        }

        // ==================================================
        // 6. CREATE NPC CHAIR POINT
        // ==================================================

        Transform npcChairPoint =
            CreatePoint(
                points.transform,
                "NpcChairPoint"
            );

        npcChairPoint.position =
            npcChair.transform.position;

        npcChairPoint.rotation =
            npcChair.transform.rotation;

        // ==================================================
        // 7. CREATE PLAYER CHAIR POINT
        // ==================================================

        Transform playerChairPoint =
            CreatePoint(
                points.transform,
                "PlayerChairPoint"
            );

        playerChairPoint.position =
            playerChair.transform.position;

        playerChairPoint.rotation =
            playerChair.transform.rotation;

        // ==================================================
        // 8. CREATE INTERVIEW SYSTEMS
        // ==================================================

        GameObject interviewSystems =
            GameObject.Find("InterviewSystems");

        if (interviewSystems == null)
        {
            interviewSystems =
                new GameObject("InterviewSystems");
        }

        InterviewSceneController sceneController =
            interviewSystems.GetComponent<
                InterviewSceneController>();

        if (sceneController == null)
        {
            sceneController =
                interviewSystems.AddComponent<
                    InterviewSceneController>();
        }

        DialogueManager dialogueManager =
            interviewSystems.GetComponent<
                DialogueManager>();

        if (dialogueManager == null)
        {
            dialogueManager =
                interviewSystems.AddComponent<
                    DialogueManager>();
        }

        // ==================================================
        // 9. PLAYER CONVERSATION CONTROLLER
        // ==================================================

        PlayerConversationController conversationController =
            player.GetComponent<
                PlayerConversationController>();

        if (conversationController == null)
        {
            conversationController =
                player.AddComponent<
                    PlayerConversationController>();
        }

        // ==================================================
        // 10. CREATE CONVERSATION CAMERA
        // ==================================================

        GameObject conversationCameraObject =
            GameObject.Find("ConversationCamera");

        Camera conversationCamera;

        if (conversationCameraObject == null)
        {
            conversationCameraObject =
                new GameObject(
                    "ConversationCamera",
                    typeof(Camera)
                );

            conversationCameraObject.transform
                .SetParent(
                    interviewSystems.transform,
                    false
                );

            conversationCamera =
                conversationCameraObject
                    .GetComponent<Camera>();

            conversationCamera.CopyFrom(
                firstPersonCamera
            );

            conversationCamera.enabled = false;

            conversationCameraObject.transform.position =
                playerChair.transform.position +
                Vector3.up * 1.5f;

            Vector3 target =
                npcChair.transform.position +
                Vector3.up * 1.2f;

            conversationCameraObject.transform.LookAt(
                target
            );
        }
        else
        {
            conversationCamera =
                conversationCameraObject
                    .GetComponent<Camera>();

            if (conversationCamera == null)
            {
                conversationCamera =
                    conversationCameraObject
                        .AddComponent<Camera>();
            }

            conversationCamera.enabled = false;
        }

        // ==================================================
        // 11. FIND GAMEPLAY BEHAVIOURS GENERICALLY
        // ==================================================

        Behaviour movementBehaviour = null;
        Behaviour playerInteractionBehaviour = null;

        foreach (
            Behaviour behaviour
            in player.GetComponents<Behaviour>())
        {
            if (behaviour == null)
                continue;

            string typeName =
                behaviour.GetType().Name;

            if (typeName ==
                "FirstPersonController")
            {
                movementBehaviour =
                    behaviour;
            }

            if (typeName ==
                "PlayerInteraction")
            {
                playerInteractionBehaviour =
                    behaviour;
            }
        }

        // ==================================================
        // 12. WIRE PLAYER CONVERSATION CONTROLLER
        // ==================================================

        SerializedObject playerConversationSO =
            new SerializedObject(
                conversationController
            );

        playerConversationSO.FindProperty(
            "playerRoot"
        ).objectReferenceValue =
            player.transform;

        playerConversationSO.FindProperty(
            "characterController"
        ).objectReferenceValue =
            characterController;

        playerConversationSO.FindProperty(
            "firstPersonCamera"
        ).objectReferenceValue =
            firstPersonCamera;

        playerConversationSO.FindProperty(
            "conversationCamera"
        ).objectReferenceValue =
            conversationCamera;

        SerializedProperty gameplayBehaviours =
            playerConversationSO.FindProperty(
                "gameplayBehaviours"
            );

        int behaviourCount = 0;

        if (movementBehaviour != null)
            behaviourCount++;

        if (playerInteractionBehaviour != null)
            behaviourCount++;

        gameplayBehaviours.arraySize =
            behaviourCount;

        int behaviourIndex = 0;

        if (movementBehaviour != null)
        {
            gameplayBehaviours
                .GetArrayElementAtIndex(
                    behaviourIndex++
                )
                .objectReferenceValue =
                movementBehaviour;
        }

        if (playerInteractionBehaviour != null)
        {
            gameplayBehaviours
                .GetArrayElementAtIndex(
                    behaviourIndex++
                )
                .objectReferenceValue =
                playerInteractionBehaviour;
        }

        playerConversationSO
            .ApplyModifiedProperties();

        // ==================================================
        // 13. PLAYER CHAIR INTERACTION
        // ==================================================

        int interactableLayer =
            LayerMask.NameToLayer(
                "Interactable"
            );

        if (interactableLayer == -1)
        {
            Debug.LogError(
                "Interactable layer does not exist."
            );
            return;
        }

        playerChair.layer =
            interactableLayer;

        Collider chairCollider =
            playerChair.GetComponentInChildren<
                Collider>();

        if (chairCollider == null)
        {
            BoxCollider box =
                playerChair.AddComponent<
                    BoxCollider>();

            box.isTrigger = false;

            Debug.LogWarning(
                "Player chair had no Collider. " +
                "A BoxCollider was added."
            );
        }

        InterviewChairInteraction chairInteraction =
            playerChair.GetComponent<
                InterviewChairInteraction>();

        if (chairInteraction == null)
        {
            chairInteraction =
                playerChair.AddComponent<
                    InterviewChairInteraction>();
        }

        // ==================================================
        // 14. WIRE CHAIR INTERACTION
        // ==================================================

        SerializedObject chairSO =
            new SerializedObject(
                chairInteraction
            );

        chairSO.FindProperty(
            "controller"
        ).objectReferenceValue =
            sceneController;

        chairSO.ApplyModifiedProperties();

        // ==================================================
        // 15. WIRE INTERVIEW SCENE CONTROLLER
        // ==================================================

        SerializedObject sceneControllerSO =
            new SerializedObject(
                sceneController
            );

        sceneControllerSO.FindProperty(
            "npcSpawnPoint"
        ).objectReferenceValue =
            npcSpawnPoint;

        sceneControllerSO.FindProperty(
            "npcChairPoint"
        ).objectReferenceValue =
            npcChairPoint;

        sceneControllerSO.FindProperty(
            "playerConversationController"
        ).objectReferenceValue =
            conversationController;

        sceneControllerSO.FindProperty(
            "playerChairPoint"
        ).objectReferenceValue =
            playerChairPoint;

        sceneControllerSO.FindProperty(
            "playerChairInteraction"
        ).objectReferenceValue =
            chairInteraction;

        sceneControllerSO.FindProperty(
            "dialogueManager"
        ).objectReferenceValue =
            dialogueManager;

        sceneControllerSO.FindProperty(
            "lobbySceneName"
        ).stringValue =
            "Lobby";

        sceneControllerSO
            .ApplyModifiedProperties();

        // ==================================================
        // 16. CREATE INTERVIEW NAVMESH SURFACE
        // ==================================================

        GameObject navMeshObject =
            GameObject.Find(
                "InterviewNavMesh"
            );

        if (navMeshObject == null)
        {
            navMeshObject =
                new GameObject(
                    "InterviewNavMesh"
                );
        }

        NavMeshSurface navMeshSurface =
            navMeshObject.GetComponent<
                NavMeshSurface>();

        if (navMeshSurface == null)
        {
            navMeshSurface =
                navMeshObject.AddComponent<
                    NavMeshSurface>();
        }

        // ==================================================
        // 17. SAVE
        // ==================================================

        EditorUtility.SetDirty(
            interviewSystems
        );

        EditorUtility.SetDirty(
            player
        );

        EditorUtility.SetDirty(
            playerChair
        );

        EditorUtility.SetDirty(
            points
        );

        EditorUtility.SetDirty(
            navMeshObject
        );

        EditorSceneManager.MarkSceneDirty(
            player.scene
        );

        EditorSceneManager.SaveScene(
            player.scene
        );

        Selection.activeGameObject =
            interviewSystems;

        Debug.Log(
            "InterviewRoom setup completed successfully. " +
            "Adjust the interview points, ConversationCamera " +
            "and bake the InterviewNavMesh before testing."
        );
    }

    private static Transform CreatePoint(
        Transform parent,
        string pointName)
    {
        Transform existing =
            parent.Find(pointName);

        if (existing != null)
            return existing;

        GameObject point =
            new GameObject(pointName);

        point.transform.SetParent(
            parent,
            false
        );

        return point.transform;
    }
}