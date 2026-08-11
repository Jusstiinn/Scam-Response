using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupReceptionSystem
{
    [MenuItem(
        "Tools/Scam Response/Setup Reception System"
    )]
    public static void Setup()
    {
        // ============================================
        // FIND ROUTE POINTS
        // ============================================

        GameObject routeParent =
            GameObject.Find(
                "NPC_Route_Points"
            );

        if (routeParent == null)
        {
            Debug.LogError(
                "NPC_Route_Points was not found."
            );

            return;
        }

        Transform spawnPoint =
            routeParent.transform.Find(
                "NpcSpawnPoint"
            );

        Transform idle1 =
            routeParent.transform.Find(
                "IdlePoint1"
            );

        Transform idle2 =
            routeParent.transform.Find(
                "IdlePoint2"
            );

        Transform idle3 =
            routeParent.transform.Find(
                "IdlePoint3"
            );

        Transform receptionPoint =
            routeParent.transform.Find(
                "ReceptionPoint"
            );

        Transform exitPoint =
            routeParent.transform.Find(
                "ExitPoint"
            );

        // ============================================
        // FIND PLAYER
        // ============================================

        GameObject player =
            GameObject.Find(
                "PlayerCapsule"
            );

        if (player == null)
        {
            Debug.LogError(
                "PlayerCapsule was not found."
            );

            return;
        }

        Transform followTarget =
            player.transform.Find(
                "PlayerFollowTarget"
            );

        // ============================================
        // VALIDATE POINTS
        // ============================================

        if (spawnPoint == null ||
            idle1 == null ||
            idle2 == null ||
            idle3 == null ||
            receptionPoint == null ||
            exitPoint == null ||
            followTarget == null)
        {
            Debug.LogError(
                "One or more NPC route " +
                "points are missing."
            );

            return;
        }

        // ============================================
        // RECEPTION SYSTEM
        // ============================================

        GameObject receptionSystem =
            GameObject.Find(
                "ReceptionSystem"
            );

        if (receptionSystem == null)
        {
            receptionSystem =
                new GameObject(
                    "ReceptionSystem"
                );
        }

        ReceptionManager manager =
            receptionSystem.GetComponent<
                ReceptionManager>();

        if (manager == null)
        {
            manager =
                receptionSystem.AddComponent<
                    ReceptionManager>();
        }

        // ============================================
        // QUEUE NUMBER CANVAS
        // ============================================

        GameObject queueCanvas =
            GameObject.Find(
                "QueueNumberCanvas"
            );

        if (queueCanvas == null)
        {
            queueCanvas =
                new GameObject(
                    "QueueNumberCanvas",
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

            Canvas canvas =
                queueCanvas.GetComponent<
                    Canvas>();

            canvas.renderMode =
                RenderMode
                    .ScreenSpaceOverlay;

            CanvasScaler scaler =
                queueCanvas.GetComponent<
                    CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode
                    .ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(
                    1920f,
                    1080f
                );

            scaler.matchWidthOrHeight =
                0.5f;
        }

        // ============================================
        // QUEUE NUMBER TEXT
        // ============================================

        GameObject queueTextObject =
            GameObject.Find(
                "QueueNumberText"
            );

        TextMeshProUGUI queueText;

        if (queueTextObject == null)
        {
            queueTextObject =
                new GameObject(
                    "QueueNumberText",
                    typeof(RectTransform),
                    typeof(TextMeshProUGUI)
                );

            queueTextObject.transform
                .SetParent(
                    queueCanvas.transform,
                    false
                );

            RectTransform rect =
                queueTextObject
                    .GetComponent<
                        RectTransform>();

            rect.anchorMin =
                new Vector2(
                    0.5f,
                    1f
                );

            rect.anchorMax =
                new Vector2(
                    0.5f,
                    1f
                );

            rect.pivot =
                new Vector2(
                    0.5f,
                    1f
                );

            rect.anchoredPosition =
                new Vector2(
                    0f,
                    -50f
                );

            rect.sizeDelta =
                new Vector2(
                    500f,
                    100f
                );

            queueText =
                queueTextObject
                    .GetComponent<
                        TextMeshProUGUI>();

            queueText.text = "---";

            queueText.alignment =
                TextAlignmentOptions
                    .Center;

            queueText.fontSize = 48f;
        }
        else
        {
            queueText =
                queueTextObject
                    .GetComponent<
                        TextMeshProUGUI>();

            if (queueText == null)
            {
                Debug.LogError(
                    "QueueNumberText has no " +
                    "TextMeshProUGUI."
                );

                return;
            }
        }

        // ============================================
        // WIRE RECEPTION MANAGER
        // ============================================

        SerializedObject managerSO =
            new SerializedObject(
                manager
            );

        managerSO.FindProperty(
            "npcSpawnPoint"
        ).objectReferenceValue =
            spawnPoint;

        SerializedProperty idleArray =
            managerSO.FindProperty(
                "idlePoints"
            );

        idleArray.arraySize = 3;

        idleArray
            .GetArrayElementAtIndex(0)
            .objectReferenceValue =
            idle1;

        idleArray
            .GetArrayElementAtIndex(1)
            .objectReferenceValue =
            idle2;

        idleArray
            .GetArrayElementAtIndex(2)
            .objectReferenceValue =
            idle3;

        managerSO.FindProperty(
            "receptionPoint"
        ).objectReferenceValue =
            receptionPoint;

        managerSO.FindProperty(
            "exitPoint"
        ).objectReferenceValue =
            exitPoint;

        managerSO.FindProperty(
            "playerFollowTarget"
        ).objectReferenceValue =
            followTarget;

        managerSO.FindProperty(
            "queueNumberText"
        ).objectReferenceValue =
            queueText;

        managerSO
            .ApplyModifiedProperties();

        // ============================================
        // FIND EXISTING RED BUTTON
        // ============================================

        GameObject redButton =
            GameObject.Find(
                "ReceptionCallButton"
            );

        if (redButton == null)
        {
            Debug.LogError(
                "ReceptionCallButton was not " +
                "found in the Lobby scene."
            );

            return;
        }

        // ============================================
        // INTERACTABLE LAYER
        // ============================================

        int interactableLayer =
            LayerMask.NameToLayer(
                "Interactable"
            );

        if (interactableLayer == -1)
        {
            Debug.LogError(
                "Interactable layer does " +
                "not exist."
            );

            return;
        }

        SetLayerRecursively(
            redButton,
            interactableLayer
        );

        // ============================================
        // COLLIDER
        // ============================================

        Collider collider =
            redButton.GetComponentInChildren<
                Collider>();

        if (collider == null)
        {
            BoxCollider box =
                redButton.AddComponent<
                    BoxCollider>();

            box.isTrigger = false;

            Debug.LogWarning(
                "No collider was found on " +
                "ReceptionCallButton. " +
                "A BoxCollider was added."
            );
        }

        // ============================================
        // BUTTON SCRIPT
        // ============================================

        ReceptionCallButton
            callButtonScript =
                redButton.GetComponent<
                    ReceptionCallButton>();

        if (callButtonScript == null)
        {
            callButtonScript =
                redButton.AddComponent<
                    ReceptionCallButton>();
        }

        SerializedObject buttonSO =
            new SerializedObject(
                callButtonScript
            );

        buttonSO.FindProperty(
            "receptionManager"
        ).objectReferenceValue =
            manager;

        buttonSO.ApplyModifiedProperties();

        // ============================================
        // SAVE
        // ============================================

        EditorUtility.SetDirty(
            receptionSystem
        );

        EditorUtility.SetDirty(
            redButton
        );

        EditorUtility.SetDirty(
            queueCanvas
        );

        EditorSceneManager
            .MarkSceneDirty(
                player.scene
            );

        EditorSceneManager
            .SaveScene(
                player.scene
            );

        Selection.activeGameObject =
            receptionSystem;

        Debug.Log(
            "Sequential Reception System " +
            "setup completed successfully."
        );
    }

    private static void
        SetLayerRecursively(
            GameObject obj,
            int layer)
    {
        obj.layer = layer;

        foreach (
            Transform child
            in obj.transform)
        {
            SetLayerRecursively(
                child.gameObject,
                layer
            );
        }
    }
}