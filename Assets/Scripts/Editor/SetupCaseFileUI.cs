using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SetupCaseFileUI
{
    [MenuItem("Tools/Scam Response/Setup Case File UI")]
    public static void Setup()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != "Lobby")
        {
            Debug.LogError(
                "Setup Case File UI must be run inside the Lobby scene."
            );
            return;
        }

        // =========================================================
        // FIND ANALYSIS SYSTEM
        // =========================================================

        GameObject analysisSystem = GameObject.Find("AnalysisSystem");

        if (analysisSystem == null)
        {
            Debug.LogError(
                "Could not find AnalysisSystem. Complete Step 12 first."
            );
            return;
        }

        CaseFileManager manager =
            analysisSystem.GetComponent<CaseFileManager>();

        if (manager == null)
        {
            Debug.LogError(
                "AnalysisSystem does not contain CaseFileManager."
            );
            return;
        }

        // =========================================================
        // CREATE / FIND CANVAS
        // =========================================================

        GameObject canvasGO = GameObject.Find("CaseFileCanvas");

        if (canvasGO == null)
        {
            canvasGO = new GameObject(
                "CaseFileCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

            Undo.RegisterCreatedObjectUndo(
                canvasGO,
                "Create Case File Canvas"
            );

            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler =
                canvasGO.GetComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920, 1080);

            scaler.matchWidthOrHeight = 0.5f;
        }

        // =========================================================
        // CREATE ROOT PANEL
        // =========================================================

        Transform existingRoot =
            canvasGO.transform.Find("CaseFileRoot");

        GameObject root;

        if (existingRoot == null)
        {
            root = CreateUIObject(
                "CaseFileRoot",
                canvasGO.transform
            );

            Image rootImage = root.AddComponent<Image>();

            rootImage.color =
                new Color(0.07f, 0.09f, 0.12f, 0.97f);

            RectTransform rt =
                root.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(0.1f, 0.07f);
            rt.anchorMax = new Vector2(0.9f, 0.93f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        else
        {
            root = existingRoot.gameObject;
        }

        // Keep enabled while constructing it.
        root.SetActive(true);

        // =========================================================
        // TITLE
        // =========================================================

        TMP_Text caseTitle =
            CreateText(
                "CaseTitle",
                root.transform,
                "CASE FILE",
                40,
                FontStyles.Bold
            );

        RectTransform titleRT =
            caseTitle.GetComponent<RectTransform>();

        titleRT.anchorMin = new Vector2(0.05f, 0.86f);
        titleRT.anchorMax = new Vector2(0.95f, 0.96f);
        titleRT.offsetMin = Vector2.zero;
        titleRT.offsetMax = Vector2.zero;

        // =========================================================
        // VICTIM INFO
        // =========================================================

        TMP_Text victimInfo =
            CreateText(
                "VictimInfo",
                root.transform,
                "Victim Information",
                24,
                FontStyles.Normal
            );

        RectTransform victimRT =
            victimInfo.GetComponent<RectTransform>();

        victimRT.anchorMin = new Vector2(0.05f, 0.74f);
        victimRT.anchorMax = new Vector2(0.95f, 0.85f);
        victimRT.offsetMin = Vector2.zero;
        victimRT.offsetMax = Vector2.zero;

        // =========================================================
        // QUESTION CONTAINER
        // =========================================================

        GameObject questionContainerGO =
            CreateUIObject(
                "QuestionContainer",
                root.transform
            );

        RectTransform containerRT =
            questionContainerGO.GetComponent<RectTransform>();

        containerRT.anchorMin = new Vector2(0.05f, 0.16f);
        containerRT.anchorMax = new Vector2(0.95f, 0.72f);
        containerRT.offsetMin = Vector2.zero;
        containerRT.offsetMax = Vector2.zero;

        VerticalLayoutGroup vertical =
            questionContainerGO.GetComponent<VerticalLayoutGroup>();

        if (vertical == null)
            vertical =
                questionContainerGO.AddComponent<VerticalLayoutGroup>();

        vertical.spacing = 15;
        vertical.padding = new RectOffset(10, 10, 10, 10);
        vertical.childControlHeight = true;
        vertical.childControlWidth = true;
        vertical.childForceExpandHeight = false;
        vertical.childForceExpandWidth = true;

        ContentSizeFitter fitter =
            questionContainerGO.GetComponent<ContentSizeFitter>();

        if (fitter == null)
            fitter =
                questionContainerGO.AddComponent<ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter.FitMode.PreferredSize;

        // =========================================================
        // SUBMIT BUTTON
        // =========================================================

        GameObject submitGO =
            CreateUIObject(
                "SubmitButton",
                root.transform
            );

        Image submitImage =
            submitGO.GetComponent<Image>();

        if (submitImage == null)
            submitImage = submitGO.AddComponent<Image>();

        submitImage.color =
            new Color(0.15f, 0.45f, 0.75f, 1f);

        Button submitButton =
            submitGO.GetComponent<Button>();

        if (submitButton == null)
            submitButton = submitGO.AddComponent<Button>();

        RectTransform submitRT =
            submitGO.GetComponent<RectTransform>();

        submitRT.anchorMin = new Vector2(0.38f, 0.04f);
        submitRT.anchorMax = new Vector2(0.62f, 0.12f);
        submitRT.offsetMin = Vector2.zero;
        submitRT.offsetMax = Vector2.zero;

        TMP_Text submitLabel =
            CreateText(
                "Label",
                submitGO.transform,
                "SUBMIT ANALYSIS",
                24,
                FontStyles.Bold
            );

        RectTransform submitLabelRT =
            submitLabel.GetComponent<RectTransform>();

        submitLabelRT.anchorMin = Vector2.zero;
        submitLabelRT.anchorMax = Vector2.one;
        submitLabelRT.offsetMin = Vector2.zero;
        submitLabelRT.offsetMax = Vector2.zero;

        submitLabel.alignment =
            TextAlignmentOptions.Center;

        // =========================================================
        // CREATE QUESTION PREFAB TEMPORARILY
        // =========================================================

        GameObject question =
            CreateUIObject(
                "CaseFileQuestion",
                canvasGO.transform
            );

        Image background =
            question.GetComponent<Image>();

        if (background == null)
            background = question.AddComponent<Image>();

        background.color = Color.white;

        LayoutElement layout =
            question.GetComponent<LayoutElement>();

        if (layout == null)
            layout = question.AddComponent<LayoutElement>();

        layout.preferredHeight = 180;
        layout.minHeight = 150;

        // Prompt
        TMP_Text prompt =
            CreateText(
                "Prompt",
                question.transform,
                "Question Prompt",
                22,
                FontStyles.Bold
            );

        prompt.color = Color.black;

        RectTransform promptRT =
            prompt.GetComponent<RectTransform>();

        promptRT.anchorMin = new Vector2(0.03f, 0.60f);
        promptRT.anchorMax = new Vector2(0.97f, 0.94f);
        promptRT.offsetMin = Vector2.zero;
        promptRT.offsetMax = Vector2.zero;

        // Dropdown
        GameObject dropdownGO =
            CreateDropdown(
                question.transform
            );

        TMP_Dropdown dropdown =
            dropdownGO.GetComponent<TMP_Dropdown>();

        RectTransform dropdownRT =
            dropdownGO.GetComponent<RectTransform>();

        dropdownRT.anchorMin =
            new Vector2(0.03f, 0.25f);

        dropdownRT.anchorMax =
            new Vector2(0.97f, 0.57f);

        dropdownRT.offsetMin = Vector2.zero;
        dropdownRT.offsetMax = Vector2.zero;

        // Support Hint
        TMP_Text hint =
            CreateText(
                "SupportHint",
                question.transform,
                "",
                16,
                FontStyles.Italic
            );

        hint.color =
            new Color(0.55f, 0.1f, 0.1f, 1f);

        RectTransform hintRT =
            hint.GetComponent<RectTransform>();

        hintRT.anchorMin =
            new Vector2(0.03f, 0.03f);

        hintRT.anchorMax =
            new Vector2(0.97f, 0.22f);

        hintRT.offsetMin = Vector2.zero;
        hintRT.offsetMax = Vector2.zero;

        // =========================================================
        // ADD QUESTION SCRIPT
        // =========================================================

        CaseFileQuestionUI questionUI =
            question.GetComponent<CaseFileQuestionUI>();

        if (questionUI == null)
            questionUI =
                question.AddComponent<CaseFileQuestionUI>();

        SerializedObject questionSO =
            new SerializedObject(questionUI);

        SetReference(
            questionSO,
            "promptText",
            prompt
        );

        SetReference(
            questionSO,
            "dropdown",
            dropdown
        );

        SetReference(
            questionSO,
            "background",
            background
        );

        SetReference(
            questionSO,
            "supportHintText",
            hint
        );

        questionSO.ApplyModifiedPropertiesWithoutUndo();

        // =========================================================
        // CREATE PREFAB
        // =========================================================

        string folder = "Assets/Prefabs/UI";

        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets/Prefabs", "UI");

        string prefabPath =
            folder + "/CaseFileQuestion.prefab";

        GameObject prefab =
            PrefabUtility.SaveAsPrefabAsset(
                question,
                prefabPath
            );

        CaseFileQuestionUI prefabQuestionUI =
            prefab.GetComponent<CaseFileQuestionUI>();

        Object.DestroyImmediate(question);

        // =========================================================
        // ASSIGN CASE FILE MANAGER
        // =========================================================

        SerializedObject managerSO =
            new SerializedObject(manager);

        SetReference(managerSO, "root", root);
        SetReference(managerSO, "caseTitleText", caseTitle);
        SetReference(managerSO, "victimInfoText", victimInfo);

        SetReference(
            managerSO,
            "questionContainer",
            questionContainerGO.transform
        );

        SetReference(
            managerSO,
            "questionPrefab",
            prefabQuestionUI
        );

        SetReference(
            managerSO,
            "submitButton",
            submitButton
        );

        // Result UI intentionally left alone.
        // Step 14 will create and assign it.

        SerializedProperty points =
            managerSO.FindProperty(
                "pointsPerCorrectAnswer"
            );

        SerializedProperty penalty =
            managerSO.FindProperty(
                "wrongAnswerPenalty"
            );

        SerializedProperty feedback =
            managerSO.FindProperty(
                "feedbackDuration"
            );

        if (points != null)
            points.intValue = 100;

        if (penalty != null)
            penalty.intValue = 20;

        if (feedback != null)
            feedback.floatValue = 2.5f;

        managerSO.ApplyModifiedPropertiesWithoutUndo();

        // =========================================================
        // EVENT SYSTEM
        // =========================================================

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem =
                new GameObject(
                    "EventSystem",
                    typeof(EventSystem),
                    typeof(StandaloneInputModule)
                );

            Undo.RegisterCreatedObjectUndo(
                eventSystem,
                "Create EventSystem"
            );
        }

        // =========================================================
        // CASE FILE STARTS HIDDEN
        // =========================================================

        root.SetActive(false);

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(canvasGO);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = root;

        Debug.Log(
            "Case File UI setup completed successfully. " +
            "Result UI remains unassigned until Step 14."
        );
    }

    // =============================================================
    // HELPERS
    // =============================================================

    private static GameObject CreateUIObject(
        string name,
        Transform parent
    )
    {
        Transform existing = parent.Find(name);

        if (existing != null)
            return existing.gameObject;

        GameObject go =
            new GameObject(
                name,
                typeof(RectTransform)
            );

        go.transform.SetParent(parent, false);

        Undo.RegisterCreatedObjectUndo(
            go,
            "Create " + name
        );

        return go;
    }

    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string text,
        float fontSize,
        FontStyles style
    )
    {
        GameObject go =
            CreateUIObject(name, parent);

        TextMeshProUGUI tmp =
            go.GetComponent<TextMeshProUGUI>();

        if (tmp == null)
            tmp = go.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        tmp.alignment =
            TextAlignmentOptions.MidlineLeft;

        return tmp;
    }

    private static GameObject CreateDropdown(
        Transform parent
    )
    {
        GameObject root =
            CreateUIObject(
                "Dropdown",
                parent
            );

        Image image =
            root.GetComponent<Image>();

        if (image == null)
            image = root.AddComponent<Image>();

        image.color = Color.white;

        TMP_Dropdown dropdown =
            root.GetComponent<TMP_Dropdown>();

        if (dropdown == null)
            dropdown =
                root.AddComponent<TMP_Dropdown>();

        // Caption
        TMP_Text caption =
            CreateText(
                "Label",
                root.transform,
                "Select Answer",
                18,
                FontStyles.Normal
            );

        caption.color = Color.black;

        RectTransform captionRT =
            caption.GetComponent<RectTransform>();

        captionRT.anchorMin =
            new Vector2(0.03f, 0);

        captionRT.anchorMax =
            new Vector2(0.90f, 1);

        captionRT.offsetMin = Vector2.zero;
        captionRT.offsetMax = Vector2.zero;

        // Arrow
        GameObject arrow =
            CreateUIObject(
                "Arrow",
                root.transform
            );

        Image arrowImage =
            arrow.AddComponent<Image>();

        RectTransform arrowRT =
            arrow.GetComponent<RectTransform>();

        arrowRT.anchorMin =
            new Vector2(0.92f, 0.25f);

        arrowRT.anchorMax =
            new Vector2(0.98f, 0.75f);

        arrowRT.offsetMin = Vector2.zero;
        arrowRT.offsetMax = Vector2.zero;

        // Template
        GameObject template =
            CreateUIObject(
                "Template",
                root.transform
            );

        Image templateImage =
            template.AddComponent<Image>();

        templateImage.color =
            new Color(0.95f, 0.95f, 0.95f, 1);

        ScrollRect scroll =
            template.AddComponent<ScrollRect>();

        RectTransform templateRT =
            template.GetComponent<RectTransform>();

        templateRT.anchorMin =
            new Vector2(0, 0);

        templateRT.anchorMax =
            new Vector2(1, 0);

        templateRT.pivot =
            new Vector2(0.5f, 1);

        templateRT.sizeDelta =
            new Vector2(0, 180);

        templateRT.anchoredPosition =
            new Vector2(0, 0);

        // Viewport
        GameObject viewport =
            CreateUIObject(
                "Viewport",
                template.transform
            );

        Image viewportImage =
            viewport.AddComponent<Image>();

        viewportImage.color = Color.white;

        Mask mask =
            viewport.AddComponent<Mask>();

        mask.showMaskGraphic = false;

        RectTransform viewportRT =
            viewport.GetComponent<RectTransform>();

        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.offsetMin = Vector2.zero;
        viewportRT.offsetMax = Vector2.zero;

        // Content
        GameObject content =
            CreateUIObject(
                "Content",
                viewport.transform
            );

        RectTransform contentRT =
            content.GetComponent<RectTransform>();

        contentRT.anchorMin =
            new Vector2(0, 1);

        contentRT.anchorMax =
            new Vector2(1, 1);

        contentRT.pivot =
            new Vector2(0.5f, 1);

        contentRT.sizeDelta =
            new Vector2(0, 30);

        // Item
        GameObject item =
            CreateUIObject(
                "Item",
                content.transform
            );

        Toggle toggle =
            item.AddComponent<Toggle>();

        RectTransform itemRT =
            item.GetComponent<RectTransform>();

        itemRT.anchorMin =
            new Vector2(0, 0.5f);

        itemRT.anchorMax =
            new Vector2(1, 0.5f);

        itemRT.sizeDelta =
            new Vector2(0, 30);

        // Item Background
        GameObject itemBackground =
            CreateUIObject(
                "Item Background",
                item.transform
            );

        Image itemBGImage =
            itemBackground.AddComponent<Image>();

        itemBGImage.color = Color.white;

        RectTransform itemBGRT =
            itemBackground.GetComponent<RectTransform>();

        itemBGRT.anchorMin = Vector2.zero;
        itemBGRT.anchorMax = Vector2.one;
        itemBGRT.offsetMin = Vector2.zero;
        itemBGRT.offsetMax = Vector2.zero;

        // Item Checkmark
        GameObject checkmark =
            CreateUIObject(
                "Item Checkmark",
                item.transform
            );

        Image checkImage =
            checkmark.AddComponent<Image>();

        RectTransform checkRT =
            checkmark.GetComponent<RectTransform>();

        checkRT.anchorMin =
            new Vector2(0, 0.1f);

        checkRT.anchorMax =
            new Vector2(0.06f, 0.9f);

        checkRT.offsetMin = Vector2.zero;
        checkRT.offsetMax = Vector2.zero;

        // Item Label
        TMP_Text itemLabel =
            CreateText(
                "Item Label",
                item.transform,
                "Option",
                17,
                FontStyles.Normal
            );

        itemLabel.color = Color.black;

        RectTransform itemLabelRT =
            itemLabel.GetComponent<RectTransform>();

        itemLabelRT.anchorMin =
            new Vector2(0.08f, 0);

        itemLabelRT.anchorMax =
            new Vector2(1, 1);

        itemLabelRT.offsetMin = Vector2.zero;
        itemLabelRT.offsetMax = Vector2.zero;

        // Configure toggle
        toggle.targetGraphic = itemBGImage;
        toggle.graphic = checkImage;

        // Configure scroll
        scroll.viewport = viewportRT;
        scroll.content = contentRT;
        scroll.horizontal = false;

        // Configure dropdown
        dropdown.targetGraphic = image;
        dropdown.captionText = caption;
        dropdown.template = templateRT;
        dropdown.itemText = itemLabel;

        template.SetActive(false);

        return root;
    }

    private static void SetReference(
        SerializedObject serializedObject,
        string propertyName,
        Object value
    )
    {
        SerializedProperty property =
            serializedObject.FindProperty(
                propertyName
            );

        if (property != null)
        {
            property.objectReferenceValue = value;
        }
        else
        {
            Debug.LogWarning(
                "Could not find serialized field: " +
                propertyName
            );
        }
    }

    private static void EnsureFolder(
        string parent,
        string child
    )
    {
        string path = parent + "/" + child;

        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(
                parent,
                child
            );
        }
    }
}