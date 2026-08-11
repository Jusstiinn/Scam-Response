using UnityEditor;
using UnityEngine;

public class SetupNpcBehaviours
{
    [MenuItem("Tools/Scam Response/Setup NPC Behaviours")]
    public static void Setup()
    {
        // ============================================
        // CASE DATA
        // ============================================

        ScamCaseData case01 =
            AssetDatabase.LoadAssetAtPath<ScamCaseData>(
                "Assets/Data/Cases/Sign-in Alert Mimic.asset"
            );

        ScamCaseData case02 =
            AssetDatabase.LoadAssetAtPath<ScamCaseData>(
                "Assets/Data/Cases/Failed Delivery Scam.asset"
            );

        ScamCaseData case03 =
            AssetDatabase.LoadAssetAtPath<ScamCaseData>(
                "Assets/Data/Cases/Fake Bank Letter.asset"
            );

        if (case01 == null ||
            case02 == null ||
            case03 == null)
        {
            Debug.LogError(
                "One or more ScamCaseData assets could not be found."
            );

            return;
        }

        // C01 = Normal Responder
        case01.behaviourType =
            NpcBehaviourType.NormalResponder;

        // C02 = Does Not Respond
        case02.behaviourType =
            NpcBehaviourType.DoesNotRespond;

        // C03 = Anxious Rush
        case03.behaviourType =
            NpcBehaviourType.AnxiousRush;

        EditorUtility.SetDirty(case01);
        EditorUtility.SetDirty(case02);
        EditorUtility.SetDirty(case03);

        // ============================================
        // NPC PREFABS
        // ============================================

        string[] prefabPaths =
        {
            "Assets/Prefabs/NPCs/AlanNPC.prefab",
            "Assets/Prefabs/NPCs/EmilyNPC.prefab",
            "Assets/Prefabs/NPCs/DanielNPC.prefab"
        };

        foreach (string path in prefabPaths)
        {
            GameObject root =
                PrefabUtility.LoadPrefabContents(path);

            if (root == null)
            {
                Debug.LogWarning(
                    "NPC prefab could not be opened: " +
                    path
                );

                continue;
            }

            ReceptionNpcController controller =
                root.GetComponent<ReceptionNpcController>();

            if (controller == null)
            {
                Debug.LogError(
                    root.name +
                    " has no ReceptionNpcController."
                );

                PrefabUtility.UnloadPrefabContents(root);
                continue;
            }

            SerializedObject controllerSO =
                new SerializedObject(controller);

            SerializedProperty waitProperty =
                controllerSO.FindProperty(
                    "unattendedWaitSeconds"
                );

            if (waitProperty != null)
                waitProperty.floatValue = 18f;

            SerializedProperty cooldownProperty =
                controllerSO.FindProperty(
                    "returnCooldownRange"
                );

            if (cooldownProperty != null)
            {
                cooldownProperty.vector2Value =
                    new Vector2(5f, 10f);
            }

            SerializedProperty refreshProperty =
                controllerSO.FindProperty(
                    "followRefreshRate"
                );

            if (refreshProperty != null)
                refreshProperty.floatValue = 0.2f;

            controllerSO.ApplyModifiedProperties();

            PrefabUtility.SaveAsPrefabAsset(
                root,
                path
            );

            PrefabUtility.UnloadPrefabContents(
                root
            );
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "NPC Behaviour setup completed successfully."
        );
    }
}