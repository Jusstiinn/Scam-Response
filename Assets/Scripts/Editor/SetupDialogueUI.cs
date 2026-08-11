using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SetupDialogueUI
{
    [MenuItem("Tools/Scam Response/Setup Dialogue UI")]
    public static void Setup()
    {
        // ==================================================
        // 0. CHECK SCENE
        // ==================================================

        if (UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().name != "InterviewRoom")
        {
            Debug.LogError(
                "Open the InterviewRoom scene before running Dialogue UI setup."
            );
            return;
        }

        // ==================================================
        // 1. FIND INTERVIEW SYSTEMS
        // ==================================================

        GameObject interviewSystems =
            GameObject.Find("InterviewSystems");

        if (interviewSystems == null)
        {
            Debug.LogError(
                "InterviewSystems was not found. Complete Step 10 first."
            );
            return;
        }

        DialogueManager dialogueManager =
            interviewSystems.GetComponent<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError(
                "DialogueManager was not found on InterviewSystems."
            );
            return;
        }

        // ==================================================
        // 2. CREATE CANVAS
        // ==================================================

        GameObject canvasObject =
            GameObject.Find("DialogueCanvas");

        Canvas canvas;

        if (canvasObject == null)
        {
            canvasObject =
                new GameObject(
                    "DialogueCanvas",
                    typeof(RectTransform),
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

            canvas =
                canvasObject.GetComponent<Canvas>();

            canvas.renderMode =
                RenderMode.ScreenSpaceOverlay;

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
            canvas =
                canvasObject.GetComponent<Canvas>();
        }

        // ==================================================
        // 3. CREATE DIALOGUE ROOT
        // ==================================================

        GameObject dialogueRoot =
            FindOrCreateUIObject(
                "DialogueRoot",
                canvasObject.transform
            );

        RectTransform rootRect =
            dialogueRoot.GetComponent<RectTransform>();

        rootRect.anchorMin =
            new Vector2(0.08f, 0.05f);

        rootRect.anchorMax =
            new Vector2(0.92f, 0.42f);

        rootRect.offsetMin =
            Vector2.zero;

        rootRect.offsetMax =
            Vector2.zero;

        Image rootImage =
            dialogueRoot.GetComponent<Image>();

        if (rootImage == null)
            rootImage =
                dialogueRoot.AddComponent<Image>();

        rootImage.color =
            new Color(0.05f, 0.07f, 0.1f, 0.94f);

        // ==================================================
        // 4. ADD DIALOGUE UI SCRIPT
        // ==================================================

        DialogueUI dialogueUI =
            dialogueRoot.GetComponent<DialogueUI>();

        if (dialogueUI == null)
            dialogueUI =
                dialogueRoot.AddComponent<DialogueUI>();

        // ==================================================
        // 5. SPEAKER NAME
        // ==================================================

        TMP_Text speakerName =
            CreateTMP(
                "SpeakerName",
                dialogueRoot.transform,
                new Vector2(0.04f, 0.78f),
                new Vector2(0.96f, 0.96f),
                32,
                FontStyles.Bold,
                TextAlignmentOptions.Left
            );

        speakerName.text =
            "Speaker Name";

        // ==================================================
        // 6. DIALOGUE TEXT
        // ==================================================

        TMP_Text dialogueText =
            CreateTMP(
                "Dialogue",
                dialogueRoot.transform,
                new Vector2(0.04f, 0.50f),
                new Vector2(0.96f, 0.78f),
                26,
                FontStyles.Normal,
                TextAlignmentOptions.TopLeft
            );

        dialogueText.text =
            "Dialogue text will appear here.";

        // ==================================================
        // 7. CHOICE CONTAINER
        // ==================================================

        GameObject choiceContainer =
            FindOrCreateUIObject(
                "ChoiceContainer",
                dialogueRoot.transform
            );

        RectTransform choiceRect =
            choiceContainer.GetComponent<RectTransform>();

        choiceRect.anchorMin =
            new Vector2(0.04f, 0.08f);

        choiceRect.anchorMax =
            new Vector2(0.72f, 0.48f);

        choiceRect.offsetMin =
            Vector2.zero;

        choiceRect.offsetMax =
            Vector2.zero;

        VerticalLayoutGroup vertical =
            choiceContainer.GetComponent<VerticalLayoutGroup>();

        if (vertical == null)
            vertical =
                choiceContainer.AddComponent<VerticalLayoutGroup>();

        vertical.spacing = 10;
        vertical.childAlignment =
            TextAnchor.UpperLeft;

        vertical.childControlWidth = true;
        vertical.childControlHeight = true;
        vertical.childForceExpandWidth = true;
        vertical.childForceExpandHeight = false;

        ContentSizeFitter fitter =
            choiceContainer.GetComponent<ContentSizeFitter>();

        if (fitter == null)
            fitter =
                choiceContainer.AddComponent<ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        // ==================================================
        // 8. CONTINUE BUTTON
        // ==================================================

        Button continueButton =
            CreateButton(
                "ContinueButton",
                dialogueRoot.transform,
                "Continue"
            );

        RectTransform continueRect =
            continueButton.GetComponent<RectTransform>();

        continueRect.anchorMin =
            new Vector2(0.76f, 0.08f);

        continueRect.anchorMax =
            new Vector2(0.96f, 0.28f);

        continueRect.offsetMin =
            Vector2.zero;

        continueRect.offsetMax =
            Vector2.zero;

        // ==================================================
        // 9. CREATE CHOICE BUTTON PREFAB
        // ==================================================

        string prefabFolder =
            "Assets/Prefabs/UI";

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder(
                "Assets",
                "Prefabs"
            );
        }

        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder(
                "Assets/Prefabs",
                "UI"
            );
        }

        string prefabPath =
            prefabFolder +
            "/DialogueChoiceButton.prefab";

        Button choiceButtonPrefab =
            AssetDatabase.LoadAssetAtPath<Button>(
                prefabPath
            );

        if (choiceButtonPrefab == null)
        {
            GameObject temporaryButton =
                CreateButton(
                    "DialogueChoiceButton",
                    null,
                    "Choice"
                ).gameObject;

            RectTransform tempRect =
                temporaryButton.GetComponent<RectTransform>();

            tempRect.sizeDelta =
                new Vector2(700, 65);

            LayoutElement layout =
                temporaryButton.GetComponent<LayoutElement>();

            if (layout == null)
                layout =
                    temporaryButton.AddComponent<LayoutElement>();

            layout.preferredHeight = 65;

            GameObject prefab =
                PrefabUtility.SaveAsPrefabAsset(
                    temporaryButton,
                    prefabPath
                );

            Object.DestroyImmediate(
                temporaryButton
            );

            choiceButtonPrefab =
                prefab.GetComponent<Button>();
        }

        // ==================================================
        // 10. ASSIGN DIALOGUE UI REFERENCES
        // ==================================================

        SerializedObject dialogueSO =
            new SerializedObject(dialogueUI);

        SetReference(
            dialogueSO,
            "root",
            dialogueRoot
        );

        SetReference(
            dialogueSO,
            "speakerNameText",
            speakerName
        );

        SetReference(
            dialogueSO,
            "dialogueText",
            dialogueText
        );

        SetReference(
            dialogueSO,
            "choiceContainer",
            choiceContainer.transform
        );

        SetReference(
            dialogueSO,
            "choiceButtonPrefab",
            choiceButtonPrefab
        );

        SetReference(
            dialogueSO,
            "continueButton",
            continueButton
        );

        dialogueSO.ApplyModifiedProperties();

        // ==================================================
        // 11. ASSIGN DIALOGUE UI TO DIALOGUE MANAGER
        // ==================================================

        SerializedObject managerSO =
            new SerializedObject(dialogueManager);

        SerializedProperty uiProperty =
            managerSO.FindProperty("ui");

        if (uiProperty != null)
        {
            uiProperty.objectReferenceValue =
                dialogueUI;

            managerSO.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning(
                "Could not automatically find the 'ui' field on DialogueManager."
            );
        }

        // ==================================================
        // 12. EVENT SYSTEM
        // ==================================================

        if (Object.FindFirstObjectByType<
            UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem =
                new GameObject(
                    "EventSystem",
                    typeof(
                        UnityEngine.EventSystems.EventSystem
                    ),
                    typeof(
                        UnityEngine.EventSystems.StandaloneInputModule
                    )
                );
        }

        // ==================================================
        // 13. START HIDDEN
        // ==================================================

        dialogueRoot.SetActive(false);

        // ==================================================
        // 14. SAVE
        // ==================================================

        EditorUtility.SetDirty(
            canvasObject
        );

        EditorUtility.SetDirty(
            interviewSystems
        );

        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement
                .SceneManager
                .GetActiveScene()
        );

        EditorSceneManager.SaveOpenScenes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject =
            canvasObject;

        Debug.Log(
            "Dialogue UI setup completed successfully."
        );
    }

    // ======================================================
    // HELPER: CREATE UI OBJECT
    // ======================================================

    private static GameObject FindOrCreateUIObject(
        string name,
        Transform parent)
    {
        Transform existing =
            parent.Find(name);

        if (existing != null)
            return existing.gameObject;

        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform)
            );

        obj.transform.SetParent(
            parent,
            false
        );

        return obj;
    }

    // ======================================================
    // HELPER: CREATE TMP
    // ======================================================

    private static TMP_Text CreateTMP(
        string name,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        float fontSize,
        FontStyles style,
        TextAlignmentOptions alignment)
    {
        GameObject obj =
            FindOrCreateUIObject(
                name,
                parent
            );

        TextMeshProUGUI tmp =
            obj.GetComponent<TextMeshProUGUI>();

        if (tmp == null)
            tmp =
                obj.AddComponent<TextMeshProUGUI>();

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;

        return tmp;
    }

    // ======================================================
    // HELPER: CREATE BUTTON
    // ======================================================

    private static Button CreateButton(
        string name,
        Transform parent,
        string label)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button)
            );

        if (parent != null)
        {
            obj.transform.SetParent(
                parent,
                false
            );
        }

        Image image =
            obj.GetComponent<Image>();

        image.color =
            new Color(
                0.16f,
                0.20f,
                0.28f,
                1f
            );

        Button button =
            obj.GetComponent<Button>();

        GameObject textObject =
            new GameObject(
                "Label",
                typeof(RectTransform),
                typeof(TextMeshProUGUI)
            );

        textObject.transform.SetParent(
            obj.transform,
            false
        );

        RectTransform textRect =
            textObject.GetComponent<RectTransform>();

        textRect.anchorMin =
            Vector2.zero;

        textRect.anchorMax =
            Vector2.one;

        textRect.offsetMin =
            new Vector2(15, 5);

        textRect.offsetMax =
            new Vector2(-15, -5);

        TextMeshProUGUI tmp =
            textObject.GetComponent<
                TextMeshProUGUI>();

        tmp.text = label;
        tmp.fontSize = 24;
        tmp.alignment =
            TextAlignmentOptions.Center;

        return button;
    }

    // ======================================================
    // HELPER: SAFE SERIALIZED ASSIGNMENT
    // ======================================================

    private static void SetReference(
        SerializedObject serializedObject,
        string propertyName,
        Object value)
    {
        SerializedProperty property =
            serializedObject.FindProperty(
                propertyName
            );

        if (property != null)
        {
            property.objectReferenceValue =
                value;
        }
        else
        {
            Debug.LogWarning(
                "Could not find field: " +
                propertyName
            );
        }
    }
}