using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SetupInterviewDoor
{
    [MenuItem("Tools/Scam Response/Setup Interview Door")]
    public static void Setup()
    {
        // --------------------------------------------------
        // 1. FIND INVESTIGATION DOOR
        // --------------------------------------------------

        GameObject door = GameObject.Find("Investigation_door");

        if (door == null)
        {
            Debug.LogError(
                "Could not find Investigation_door in the current scene."
            );
            return;
        }

        // --------------------------------------------------
        // 2. SET INTERACTABLE LAYER
        // --------------------------------------------------

        int interactableLayer =
            LayerMask.NameToLayer("Interactable");

        if (interactableLayer == -1)
        {
            Debug.LogError(
                "Interactable layer does not exist."
            );
            return;
        }

        SetLayerRecursively(
            door,
            interactableLayer
        );

        // --------------------------------------------------
        // 3. ENSURE COLLIDER EXISTS
        // --------------------------------------------------

        Collider collider =
            door.GetComponentInChildren<Collider>();

        if (collider == null)
        {
            BoxCollider box =
                door.AddComponent<BoxCollider>();

            box.isTrigger = false;

            Debug.LogWarning(
                "Investigation_door had no Collider. " +
                "A BoxCollider was added to the root."
            );
        }

        // --------------------------------------------------
        // 4. ADD / FIND SCENE LOADER
        // --------------------------------------------------

        InterviewDoorSceneLoader loader =
            door.GetComponent<InterviewDoorSceneLoader>();

        if (loader == null)
        {
            loader =
                door.AddComponent<InterviewDoorSceneLoader>();
        }

        // --------------------------------------------------
        // 5. SET SCENE NAME
        // --------------------------------------------------

        SerializedObject loaderSO =
            new SerializedObject(loader);

        SerializedProperty sceneName =
            loaderSO.FindProperty("interviewSceneName");

        if (sceneName != null)
        {
            sceneName.stringValue =
                "InterviewRoom";
        }

        loaderSO.ApplyModifiedProperties();

        // --------------------------------------------------
        // 6. SAVE
        // --------------------------------------------------

        EditorUtility.SetDirty(door);

        EditorSceneManager.MarkSceneDirty(
            door.scene
        );

        EditorSceneManager.SaveScene(
            door.scene
        );

        Selection.activeGameObject =
            door;

        Debug.Log(
            "Interview door setup completed successfully."
        );
    }

    private static void SetLayerRecursively(
        GameObject obj,
        int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(
                child.gameObject,
                layer
            );
        }
    }
}