using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SetupResultUI
{
    [MenuItem("Tools/Scam Response/Setup Result UI")]
    public static void Setup()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != "Lobby")
        {
            Debug.LogError(
                "Setup Result UI must be run inside the Lobby scene."
            );
            return;
        }

        // =========================================================
        // FIND ANALYSIS SYSTEM
        // =========================================================

        GameObject analysisSystem =
            GameObject.Find("AnalysisSystem");

        if (analysisSystem == null)
        {
            Debug.LogError(
                "AnalysisSystem was not found. Complete Step 12 first."
            );
            return;
        }

        CaseFileManager caseFileManager =
            analysisSystem.GetComponent<CaseFileManager>();

        if (caseFileManager == null)
        {
            Debug.LogError(
                "CaseFileManager was not found on AnalysisSystem."
            );
            return;
        }

        // =========================================================
        // CREATE / FIND RESULT CANVAS
        // =========================================================

        GameObject canvasGO =
            GameObject.Find("ResultCanvas");

        if (canvasGO == null)
        {
            canvasGO =
                new GameObject(
                    "ResultCanvas",
                    typeof(RectTransform),
                    typeof(Canvas),
                    typeof(CanvasScaler),
                    typeof(GraphicRaycaster)
                );

            Undo.RegisterCreatedObjectUndo(
                canvasGO,
                "Create ResultCanvas"
            );

            Canvas canvas =
                canvasGO.GetComponent<Canvas>();

            canvas.renderMode =
                RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler =
                canvasGO.GetComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920f, 1080f);

            scaler.matchWidthOrHeight = 0.5f;
        }

        // =========================================================
        // CASE RESULT ROOT
        // =========================================================

        GameObject caseResultRoot =
            CreateUIObject(
                "CaseResultRoot",
                canvasGO.transform
            );

        Image caseResultBackground =
            GetOrAddImage(caseResultRoot);

        caseResultBackground.color =
            new Color(
                0.06f,
                0.08f,
                0.12f,
                0.97f
            );

        RectTransform caseResultRT =
            caseResultRoot.GetComponent<RectTransform>();

        caseResultRT.anchorMin =
            new Vector2(0.08f, 0.05f);

        caseResultRT.anchorMax =
            new Vector2(0.92f, 0.95f);

        caseResultRT.offsetMin =
            Vector2.zero;

        caseResultRT.offsetMax =
            Vector2.zero;

        CaseResultUI caseResultUI =
            caseResultRoot.GetComponent<CaseResultUI>();

        if (caseResultUI == null)
        {
            caseResultUI =
                caseResultRoot.AddComponent<CaseResultUI>();
        }

        // =========================================================
        // CASE RESULT TEXTS
        // =========================================================

        TMP_Text titleText =
            CreateText(
                "Title",
                caseResultRoot.transform,
                "CASE COMPLETE",
                40,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        SetAnchors(
            titleText.rectTransform,
            0.05f, 0.86f,
            0.95f, 0.96f
        );

        TMP_Text scoreText =
            CreateText(
                "Score",
                caseResultRoot.transform,
                "Score: 0",
                28,
                FontStyles.Bold,
                TextAlignmentOptions.Left
            );

        SetAnchors(
            scoreText.rectTransform,
            0.07f, 0.78f,
            0.45f, 0.85f
        );

        TMP_Text incorrectText =
            CreateText(
                "IncorrectCount",
                caseResultRoot.transform,
                "Incorrect Answers: 0",
                24,
                FontStyles.Normal,
                TextAlignmentOptions.Left
            );

        SetAnchors(
            incorrectText.rectTransform,
            0.50f, 0.78f,
            0.93f, 0.85f
        );

        TMP_Text howItWorksText =
            CreateSection(
                caseResultRoot.transform,
                "HowScamWorks",
                "How the scam works",
                0.07f, 0.58f,
                0.47f, 0.75f
            );

        TMP_Text warningSignsText =
            CreateSection(
                caseResultRoot.transform,
                "WarningSigns",
                "Warning signs",
                0.53f, 0.58f,
                0.93f, 0.75f
            );

        TMP_Text preventionText =
            CreateSection(
                caseResultRoot.transform,
                "Prevention",
                "How to avoid it",
                0.07f, 0.34f,
                0.47f, 0.54f
            );

        TMP_Text victimActionText =
            CreateSection(
                caseResultRoot.transform,
                "WhatToDo",
                "What to do if victim",
                0.53f, 0.34f,
                0.93f, 0.54f
            );

        // =========================================================
        // CONTINUE BUTTON
        // =========================================================

        Button continueButton =
            CreateButton(
                "ContinueButton",
                caseResultRoot.transform,
                "CONTINUE"
            );

        SetAnchors(
            continueButton
                .GetComponent<RectTransform>(),
            0.38f, 0.08f,
            0.62f, 0.18f
        );

        // =========================================================
        // FINAL SUMMARY ROOT
        // =========================================================

        GameObject summaryRoot =
            CreateUIObject(
                "FinalSummaryRoot",
                canvasGO.transform
            );

        Image summaryBackground =
            GetOrAddImage(summaryRoot);

        summaryBackground.color =
            new Color(
                0.05f,
                0.07f,
                0.10f,
                0.98f
            );

        RectTransform summaryRT =
            summaryRoot.GetComponent<RectTransform>();

        summaryRT.anchorMin =
            new Vector2(0.18f, 0.12f);

        summaryRT.anchorMax =
            new Vector2(0.82f, 0.88f);

        summaryRT.offsetMin =
            Vector2.zero;

        summaryRT.offsetMax =
            Vector2.zero;

        FinalSummaryUI finalSummaryUI =
            summaryRoot.GetComponent<FinalSummaryUI>();

        if (finalSummaryUI == null)
        {
            finalSummaryUI =
                summaryRoot.AddComponent<FinalSummaryUI>();
        }

        // =========================================================
        // SUMMARY TEXTS
        // =========================================================

        TMP_Text summaryTitle =
            CreateText(
                "SummaryTitle",
                summaryRoot.transform,
                "FINAL SUMMARY",
                42,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        SetAnchors(
            summaryTitle.rectTransform,
            0.08f, 0.82f,
            0.92f, 0.94f
        );

        TMP_Text totalScoreText =
            CreateText(
                "TotalScore",
                summaryRoot.transform,
                "Total Score: 0",
                32,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        SetAnchors(
            totalScoreText.rectTransform,
            0.10f, 0.68f,
            0.90f, 0.80f
        );

        TMP_Text breakdownText =
            CreateText(
                "CaseBreakdown",
                summaryRoot.transform,
                "Case breakdown will appear here.",
                24,
                FontStyles.Normal,
                TextAlignmentOptions.TopLeft
            );

        SetAnchors(
            breakdownText.rectTransform,
            0.10f, 0.25f,
            0.90f, 0.64f
        );

        Button restartButton =
            CreateButton(
                "RestartButton",
                summaryRoot.transform,
                "RESTART"
            );

        SetAnchors(
            restartButton
                .GetComponent<RectTransform>(),
            0.35f, 0.08f,
            0.65f, 0.18f
        );

        // =========================================================
        // WIRE FINAL SUMMARY UI
        // =========================================================

        SerializedObject summarySO =
            new SerializedObject(finalSummaryUI);

        SetReference(
            summarySO,
            "root",
            summaryRoot
        );

        SetReference(
            summarySO,
            "totalScoreText",
            totalScoreText
        );

        SetReference(
            summarySO,
            "breakdownText",
            breakdownText
        );

        SetReference(
            summarySO,
            "restartButton",
            restartButton
        );

        summarySO.ApplyModifiedPropertiesWithoutUndo();

        // =========================================================
        // WIRE CASE RESULT UI
        // =========================================================

        SerializedObject resultSO =
            new SerializedObject(caseResultUI);

        SetReference(
            resultSO,
            "root",
            caseResultRoot
        );

        SetReference(
            resultSO,
            "titleText",
            titleText
        );

        SetReference(
            resultSO,
            "scoreText",
            scoreText
        );

        SetReference(
            resultSO,
            "incorrectText",
            incorrectText
        );

        SetReference(
            resultSO,
            "howItWorksText",
            howItWorksText
        );

        SetReference(
            resultSO,
            "warningSignsText",
            warningSignsText
        );

        SetReference(
            resultSO,
            "preventionText",
            preventionText
        );

        SetReference(
            resultSO,
            "victimActionText",
            victimActionText
        );

        SetReference(
            resultSO,
            "continueButton",
            continueButton
        );

        SetReference(
            resultSO,
            "finalSummaryUI",
            finalSummaryUI
        );

        resultSO.ApplyModifiedPropertiesWithoutUndo();

        // =========================================================
        // ASSIGN RESULT UI TO CASE FILE MANAGER
        // =========================================================

        SerializedObject caseFileSO =
            new SerializedObject(caseFileManager);

        SetReference(
            caseFileSO,
            "resultUI",
            caseResultUI
        );

        caseFileSO.ApplyModifiedPropertiesWithoutUndo();

        // =========================================================
        // EVENT SYSTEM
        // =========================================================

        if (
            Object.FindFirstObjectByType<EventSystem>()
            == null
        )
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
        // START HIDDEN
        // =========================================================

        caseResultRoot.SetActive(false);
        summaryRoot.SetActive(false);

        // =========================================================
        // SAVE
        // =========================================================

        EditorUtility.SetDirty(
            caseResultUI
        );

        EditorUtility.SetDirty(
            finalSummaryUI
        );

        EditorUtility.SetDirty(
            caseFileManager
        );

        EditorUtility.SetDirty(
            canvasGO
        );

        EditorSceneManager.MarkSceneDirty(
            scene
        );

        EditorSceneManager.SaveScene(
            scene
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject =
            canvasGO;

        Debug.Log(
            "Result UI and Final Summary UI setup completed successfully."
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
        Transform existing =
            parent.Find(name);

        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject go =
            new GameObject(
                name,
                typeof(RectTransform)
            );

        go.transform.SetParent(
            parent,
            false
        );

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
        FontStyles style,
        TextAlignmentOptions alignment
    )
    {
        GameObject go =
            CreateUIObject(
                name,
                parent
            );

        TextMeshProUGUI tmp =
            go.GetComponent<TextMeshProUGUI>();

        if (tmp == null)
        {
            tmp =
                go.AddComponent<TextMeshProUGUI>();
        }

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = alignment;
        tmp.color = Color.white;

        return tmp;
    }

    private static TMP_Text CreateSection(
        Transform parent,
        string name,
        string placeholder,
        float minX,
        float minY,
        float maxX,
        float maxY
    )
    {
        TMP_Text text =
            CreateText(
                name,
                parent,
                placeholder,
                22,
                FontStyles.Normal,
                TextAlignmentOptions.TopLeft
            );

        SetAnchors(
            text.rectTransform,
            minX,
            minY,
            maxX,
            maxY
        );

        return text;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        string label
    )
    {
        GameObject go =
            CreateUIObject(
                name,
                parent
            );

        Image image =
            GetOrAddImage(go);

        image.color =
            new Color(
                0.15f,
                0.45f,
                0.75f,
                1f
            );

        Button button =
            go.GetComponent<Button>();

        if (button == null)
        {
            button =
                go.AddComponent<Button>();
        }

        TMP_Text labelText =
            CreateText(
                "Label",
                go.transform,
                label,
                24,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        RectTransform labelRT =
            labelText.rectTransform;

        labelRT.anchorMin =
            Vector2.zero;

        labelRT.anchorMax =
            Vector2.one;

        labelRT.offsetMin =
            Vector2.zero;

        labelRT.offsetMax =
            Vector2.zero;

        return button;
    }

    private static Image GetOrAddImage(
        GameObject go
    )
    {
        Image image =
            go.GetComponent<Image>();

        if (image == null)
        {
            image =
                go.AddComponent<Image>();
        }

        return image;
    }

    private static void SetAnchors(
        RectTransform rt,
        float minX,
        float minY,
        float maxX,
        float maxY
    )
    {
        rt.anchorMin =
            new Vector2(minX, minY);

        rt.anchorMax =
            new Vector2(maxX, maxY);

        rt.offsetMin =
            Vector2.zero;

        rt.offsetMax =
            Vector2.zero;
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
            property.objectReferenceValue =
                value;
        }
        else
        {
            Debug.LogWarning(
                "Could not find serialized field: " +
                propertyName
            );
        }
    }
}