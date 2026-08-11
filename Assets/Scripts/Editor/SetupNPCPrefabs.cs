using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class SetupNPCPrefabs
{
    [MenuItem("Tools/Scam Response/Setup NPC Prefabs")]
    public static void Setup()
    {
        string[] npcPrefabPaths =
        {
            "Assets/Prefabs/NPCs/AlanNPC.prefab",
            "Assets/Prefabs/NPCs/EmilyNPC.prefab",
            "Assets/Prefabs/NPCs/DanielNPC.prefab"
        };

        int completed = 0;

        foreach (string path in npcPrefabPaths)
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                Debug.LogWarning(
                    "NPC prefab not found at: " + path
                );

                continue;
            }

            GameObject root =
                PrefabUtility.LoadPrefabContents(path);

            // ------------------------------
            // TAG
            // ------------------------------

            root.tag = "NPC";

            // ------------------------------
            // ANIMATOR
            // ------------------------------

            Animator animator =
                root.GetComponentInChildren<Animator>();

            if (animator == null)
            {
                animator =
                    root.AddComponent<Animator>();

                Debug.LogWarning(
                    root.name +
                    " had no Animator. An empty Animator was added. " +
                    "You will need to assign its Animator Controller manually."
                );
            }

            // ------------------------------
            // CAPSULE COLLIDER
            // ------------------------------

            CapsuleCollider capsule =
                root.GetComponent<CapsuleCollider>();

            if (capsule == null)
                capsule =
                    root.AddComponent<CapsuleCollider>();

            // Reasonable starting values.
            // We will visually check these afterward.
            capsule.center = new Vector3(0f, 0.9f, 0f);
            capsule.height = 1.8f;
            capsule.radius = 0.35f;

            // ------------------------------
            // NAVMESH AGENT
            // ------------------------------

            NavMeshAgent agent =
                root.GetComponent<NavMeshAgent>();

            if (agent == null)
                agent =
                    root.AddComponent<NavMeshAgent>();

            agent.speed = 2.5f;
            agent.angularSpeed = 240f;
            agent.acceleration = 8f;
            agent.stoppingDistance = 1.2f;

            agent.autoBraking = true;

            // ------------------------------
            // NPC NAV AGENT
            // ------------------------------

            NpcNavAgent navAgent =
                root.GetComponent<NpcNavAgent>();

            if (navAgent == null)
                navAgent =
                    root.AddComponent<NpcNavAgent>();

            // ------------------------------
            // NPC ANIMATOR BRIDGE
            // ------------------------------

            NpcAnimatorBridge animatorBridge =
                root.GetComponent<NpcAnimatorBridge>();

            if (animatorBridge == null)
                animatorBridge =
                    root.AddComponent<NpcAnimatorBridge>();

            // Assign private serialized references
            SerializedObject animatorSerialized =
                new SerializedObject(animatorBridge);

            SerializedProperty animatorProperty =
                animatorSerialized.FindProperty("animator");

            if (animatorProperty != null)
                animatorProperty.objectReferenceValue = animator;

            SerializedProperty navProperty =
                animatorSerialized.FindProperty("navAgent");

            if (navProperty != null)
                navProperty.objectReferenceValue = navAgent;

            SerializedProperty speedParameter =
                animatorSerialized.FindProperty("speedParameter");

            if (speedParameter != null)
                speedParameter.stringValue = "Speed";

            SerializedProperty sitTrigger =
                animatorSerialized.FindProperty("sitTrigger");

            if (sitTrigger != null)
                sitTrigger.stringValue = "Sit";

            animatorSerialized.ApplyModifiedProperties();

            // ------------------------------
            // RECEPTION NPC CONTROLLER
            // ------------------------------

            ReceptionNpcController controller =
                root.GetComponent<ReceptionNpcController>();

            if (controller == null)
                controller =
                    root.AddComponent<ReceptionNpcController>();

            // ------------------------------
            // RECEPTION NPC INTERACTION
            // ------------------------------

            ReceptionNpcInteraction interaction =
                root.GetComponent<ReceptionNpcInteraction>();

            if (interaction == null)
                interaction =
                    root.AddComponent<ReceptionNpcInteraction>();

            // Assign controller references
            SerializedObject controllerSerialized =
                new SerializedObject(controller);

            SerializedProperty navigationProperty =
                controllerSerialized.FindProperty("navigation");

            if (navigationProperty != null)
                navigationProperty.objectReferenceValue = navAgent;

            SerializedProperty interactionProperty =
                controllerSerialized.FindProperty("interaction");

            if (interactionProperty != null)
                interactionProperty.objectReferenceValue = interaction;

            SerializedProperty waitSeconds =
                controllerSerialized.FindProperty("unattendedWaitSeconds");

            if (waitSeconds != null)
                waitSeconds.floatValue = 18f;

            SerializedProperty cooldown =
                controllerSerialized.FindProperty("returnCooldownRange");

            if (cooldown != null)
                cooldown.vector2Value =
                    new Vector2(5f, 10f);

            SerializedProperty refreshRate =
                controllerSerialized.FindProperty("followRefreshRate");

            if (refreshRate != null)
                refreshRate.floatValue = 0.2f;

            controllerSerialized.ApplyModifiedProperties();

            // ------------------------------
            // SAVE PREFAB
            // ------------------------------

            PrefabUtility.SaveAsPrefabAsset(
                root,
                path
            );

            PrefabUtility.UnloadPrefabContents(
                root
            );

            completed++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "NPC prefab setup completed for " +
            completed +
            " prefab(s)."
        );
    }
}