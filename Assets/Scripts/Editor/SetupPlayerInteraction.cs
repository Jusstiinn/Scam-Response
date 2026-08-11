using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupPlayerInteraction
{
    [MenuItem("Tools/Scam Response/Setup Player Interaction")]
    public static void Setup()
    {
        // --------------------------------------------------
        // 1. FIND PLAYER
        // --------------------------------------------------

        GameObject player =
            GameObject.Find("PlayerCapsule");

        if (player == null)
        {
            Debug.LogError(
                "Could not find a GameObject named PlayerCapsule."
            );
            return;
        }

        // --------------------------------------------------
        // 2. FIND CAMERA
        // --------------------------------------------------

        Camera playerCamera = Camera.main;

        if (playerCamera == null)
        {
            playerCamera =
                Object.FindFirstObjectByType<Camera>();
        }

        if (playerCamera == null)
        {
            Debug.LogError(
                "Could not find a Camera in the scene."
            );
            return;
        }

        // --------------------------------------------------
        // 3. ADD / FIND PLAYER INTERACTION
        // --------------------------------------------------

        PlayerInteraction playerInteraction =
            player.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
        {
            playerInteraction =
                player.AddComponent<PlayerInteraction>();
        }

        // --------------------------------------------------
        // 4. CREATE INTERACTION CANVAS
        // --------------------------------------------------

        GameObject canvasObject =
            GameObject.Find("InteractionCanvas");

        Canvas canvas;

        if (canvasObject == null)
        {
            canvasObject =
                new GameObject(
                    "InteractionCanvas",
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler =
                canvasObject.GetComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920, 1080);

            scaler.matchWidthOrHeight = 0.5f;
        }
        else
        {
            canvas = canvasObject.GetComponent<Canvas>();
        }

        // --------------------------------------------------
        // 5. CREATE PROMPT ROOT
        // --------------------------------------------------

        Transform promptRootTransform =
            canvasObject.transform.Find("InteractionPromptRoot");

        GameObject promptRoot;

        if (promptRootTransform == null)
        {
            promptRoot =
                new GameObject(
                    "InteractionPromptRoot",
                    typeof(RectTransform)
                );

            promptRoot.transform.SetParent(
                canvasObject.transform,
                false
            );

            RectTransform rootRect =
                promptRoot.GetComponent<RectTransform>();

            rootRect.anchorMin =
                new Vector2(0.5f, 0f);

            rootRect.anchorMax =
                new Vector2(0.5f, 0f);

            rootRect.pivot =
                new Vector2(0.5f, 0.5f);

            rootRect.anchoredPosition =
                new Vector2(0f, 100f);

            rootRect.sizeDelta =
                new Vector2(700f, 100f);
        }
        else
        {
            promptRoot =
                promptRootTransform.gameObject;
        }

        // --------------------------------------------------
        // 6. CREATE TMP TEXT
        // --------------------------------------------------

        Transform promptTextTransform =
            promptRoot.transform.Find("InteractionPromptText");

        GameObject promptTextObject;
        TextMeshProUGUI promptText;

        if (promptTextTransform == null)
        {
            promptTextObject =
                new GameObject(
                    "InteractionPromptText",
                    typeof(RectTransform),
                    typeof(TextMeshProUGUI)
                );

            promptTextObject.transform.SetParent(
                promptRoot.transform,
                false
            );

            RectTransform textRect =
                promptTextObject.GetComponent<RectTransform>();

            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            promptText =
                promptTextObject.GetComponent<TextMeshProUGUI>();

            promptText.text =
                "Press E to interact";

            promptText.alignment =
                TextAlignmentOptions.Center;

            promptText.fontSize = 36;

            promptText.enableAutoSizing = true;
            promptText.fontSizeMin = 20;
            promptText.fontSizeMax = 36;
        }
        else
        {
            promptTextObject =
                promptTextTransform.gameObject;

            promptText =
                promptTextObject.GetComponent<TextMeshProUGUI>();
        }

        // --------------------------------------------------
        // 7. ADD / FIND INTERACTION PROMPT UI
        // --------------------------------------------------

        InteractionPromptUI promptUI =
            promptRoot.GetComponent<InteractionPromptUI>();

        if (promptUI == null)
        {
            promptUI =
                promptRoot.AddComponent<InteractionPromptUI>();
        }

        // --------------------------------------------------
        // 8. ASSIGN PRIVATE SERIALIZED FIELDS
        // --------------------------------------------------

        SerializedObject promptSerialized =
            new SerializedObject(promptUI);

        promptSerialized
            .FindProperty("root")
            .objectReferenceValue = promptRoot;

        promptSerialized
            .FindProperty("promptText")
            .objectReferenceValue = promptText;

        promptSerialized.ApplyModifiedProperties();

        SerializedObject playerSerialized =
            new SerializedObject(playerInteraction);

        playerSerialized
            .FindProperty("playerCamera")
            .objectReferenceValue = playerCamera;

        playerSerialized
            .FindProperty("interactionDistance")
            .floatValue = 3f;

        playerSerialized
            .FindProperty("promptUI")
            .objectReferenceValue = promptUI;

        // --------------------------------------------------
        // 9. SET INTERACTION LAYER MASK
        // --------------------------------------------------

        int interactableLayer =
            LayerMask.NameToLayer("Interactable");

        if (interactableLayer == -1)
        {
            Debug.LogWarning(
                "Interactable layer does not exist yet. " +
                "Please create a User Layer named Interactable, " +
                "then assign it manually in PlayerInteraction."
            );

            playerSerialized
                .FindProperty("interactionLayers")
                .intValue = ~0;
        }
        else
        {
            playerSerialized
                .FindProperty("interactionLayers")
                .intValue =
                1 << interactableLayer;
        }

        playerSerialized.ApplyModifiedProperties();

        // --------------------------------------------------
        // 10. SAVE
        // --------------------------------------------------

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(promptRoot);
        EditorUtility.SetDirty(canvasObject);

        EditorSceneManager.MarkSceneDirty(
            player.scene
        );

        EditorSceneManager.SaveScene(
            player.scene
        );

        Selection.activeGameObject = player;

        Debug.Log(
            "Player Interaction setup completed successfully."
        );
    }
}