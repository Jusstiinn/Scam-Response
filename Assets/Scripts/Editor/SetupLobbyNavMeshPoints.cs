using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class SetupLobbyNavMeshPoints
{
    [MenuItem("Tools/Scam Response/Setup Lobby NavMesh Points")]
    public static void Setup()
    {
        // --------------------------------------------------
        // 1. FIND PLAYER
        // --------------------------------------------------

        GameObject player = GameObject.Find("PlayerCapsule");

        if (player == null)
        {
            Debug.LogError(
                "Could not find PlayerCapsule in the current scene."
            );
            return;
        }

        // --------------------------------------------------
        // 2. CREATE ROUTE POINT PARENT
        // --------------------------------------------------

        GameObject routeParent = GameObject.Find("NPC_Route_Points");

        if (routeParent == null)
        {
            routeParent = new GameObject("NPC_Route_Points");
        }

        // --------------------------------------------------
        // 3. CREATE NPC POINTS
        // --------------------------------------------------

        CreatePoint(
            routeParent.transform,
            "NpcSpawnPoint",
            new Vector3(0f, 0f, 0f)
        );

        CreatePoint(
            routeParent.transform,
            "IdlePoint1",
            new Vector3(1.5f, 0f, 0f)
        );

        CreatePoint(
            routeParent.transform,
            "IdlePoint2",
            new Vector3(-1.5f, 0f, 0f)
        );

        CreatePoint(
            routeParent.transform,
            "IdlePoint3",
            new Vector3(0f, 0f, 1.5f)
        );

        CreatePoint(
            routeParent.transform,
            "ReceptionPoint",
            new Vector3(0f, 0f, 3f)
        );

        CreatePoint(
            routeParent.transform,
            "ExitPoint",
            new Vector3(0f, 0f, -3f)
        );

        // --------------------------------------------------
        // 4. CREATE PLAYER FOLLOW TARGET
        // --------------------------------------------------

        Transform existingFollow =
            player.transform.Find("PlayerFollowTarget");

        if (existingFollow == null)
        {
            GameObject followTarget =
                new GameObject("PlayerFollowTarget");

            followTarget.transform.SetParent(
                player.transform,
                false
            );

            followTarget.transform.localPosition =
                new Vector3(0f, 0f, -1.7f);

            followTarget.transform.localRotation =
                Quaternion.identity;
        }

        // --------------------------------------------------
        // 5. CREATE NAVMESH OBJECT
        // --------------------------------------------------

        GameObject navMeshObject =
            GameObject.Find("LobbyNavMesh");

        if (navMeshObject == null)
        {
            navMeshObject =
                new GameObject("LobbyNavMesh");
        }

        NavMeshSurface surface =
            navMeshObject.GetComponent<NavMeshSurface>();

        if (surface == null)
        {
            surface =
                navMeshObject.AddComponent<NavMeshSurface>();
        }

        // Recommended defaults
        surface.collectObjects =
            CollectObjects.All;

        surface.useGeometry =
            NavMeshCollectGeometry.RenderMeshes;

        // --------------------------------------------------
        // 6. SAVE
        // --------------------------------------------------

        EditorUtility.SetDirty(routeParent);
        EditorUtility.SetDirty(navMeshObject);
        EditorUtility.SetDirty(player);

        EditorSceneManager.MarkSceneDirty(
            player.scene
        );

        EditorSceneManager.SaveScene(
            player.scene
        );

        Selection.activeGameObject =
            routeParent;

        Debug.Log(
            "Lobby NavMesh point setup completed. " +
            "Now move the points into sensible positions and bake the NavMesh."
        );
    }

    private static GameObject CreatePoint(
        Transform parent,
        string name,
        Vector3 localPosition)
    {
        Transform existing =
            parent.Find(name);

        if (existing != null)
            return existing.gameObject;

        GameObject point =
            new GameObject(name);

        point.transform.SetParent(
            parent,
            false
        );

        point.transform.localPosition =
            localPosition;

        return point;
    }
}