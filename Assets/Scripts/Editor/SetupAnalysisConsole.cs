using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SetupAnalysisConsole
{
    [MenuItem("Tools/Scam Response/Setup Analysis Console")]
    public static void Setup()
    {
        // Make sure we are in Lobby.
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != "Lobby")
        {
            Debug.LogError(
                "Setup Analysis Console must be run inside the Lobby scene."
            );
            return;
        }

        // --------------------------------------------------
        // 1. Find the existing Monitor
        // --------------------------------------------------

        GameObject monitor = GameObject.Find("Monitor");

        if (monitor == null)
        {
            Debug.LogError(
                "Could not find GameObject named 'Monitor' in Lobby."
            );
            return;
        }

        Undo.RegisterCompleteObjectUndo(
            monitor,
            "Setup Analysis Console"
        );

        // --------------------------------------------------
        // 2. Make sure monitor has a collider
        // --------------------------------------------------

        Collider collider = monitor.GetComponent<Collider>();

        if (collider == null)
        {
            MeshFilter meshFilter = monitor.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                MeshCollider meshCollider =
                    Undo.AddComponent<MeshCollider>(monitor);

                meshCollider.sharedMesh = meshFilter.sharedMesh;

                Debug.Log("Added MeshCollider to Monitor.");
            }
            else
            {
                Undo.AddComponent<BoxCollider>(monitor);

                Debug.Log(
                    "Monitor had no MeshFilter. Added BoxCollider instead."
                );
            }
        }

        // --------------------------------------------------
        // 3. Set Interactable layer
        // --------------------------------------------------

        int interactableLayer =
            LayerMask.NameToLayer("Interactable");

        if (interactableLayer == -1)
        {
            Debug.LogError(
                "Interactable layer does not exist. " +
                "Create it before running this setup."
            );
            return;
        }

        monitor.layer = interactableLayer;

        // --------------------------------------------------
        // 4. Create/find AnalysisSystem
        // --------------------------------------------------

        GameObject analysisSystem =
            GameObject.Find("AnalysisSystem");

        if (analysisSystem == null)
        {
            analysisSystem =
                new GameObject("AnalysisSystem");

            Undo.RegisterCreatedObjectUndo(
                analysisSystem,
                "Create AnalysisSystem"
            );
        }

        // --------------------------------------------------
        // 5. Add CaseFileManager
        // --------------------------------------------------

        CaseFileManager manager =
            analysisSystem.GetComponent<CaseFileManager>();

        if (manager == null)
        {
            manager =
                Undo.AddComponent<CaseFileManager>(
                    analysisSystem
                );
        }

        // --------------------------------------------------
        // 6. Add AnalysisConsoleInteraction
        // --------------------------------------------------

        AnalysisConsoleInteraction interaction =
            monitor.GetComponent<AnalysisConsoleInteraction>();

        if (interaction == null)
        {
            interaction =
                Undo.AddComponent<AnalysisConsoleInteraction>(
                    monitor
                );
        }

        // --------------------------------------------------
        // 7. Assign manager automatically
        // --------------------------------------------------

        SerializedObject interactionSO =
            new SerializedObject(interaction);

        SerializedProperty managerProperty =
            interactionSO.FindProperty("manager");

        if (managerProperty != null)
        {
            managerProperty.objectReferenceValue = manager;
            interactionSO.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogError(
                "Could not find 'manager' field on " +
                "AnalysisConsoleInteraction."
            );

            return;
        }

        // --------------------------------------------------
        // 8. Save scene
        // --------------------------------------------------

        EditorUtility.SetDirty(monitor);
        EditorUtility.SetDirty(analysisSystem);
        EditorUtility.SetDirty(interaction);
        EditorUtility.SetDirty(manager);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Selection.activeGameObject = analysisSystem;

        Debug.Log(
            "Analysis Console setup completed successfully."
        );
    }
}