using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseFileManager : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text caseTitleText;
    [SerializeField] private TMP_Text victimInfoText;

    [Header("Paragraph Text")]
    [SerializeField] private TMP_Text[] textSegments;

    [Header("Dropdowns")]
    [SerializeField] private Transform dropdownContainer;
    [SerializeField] private CaseFileDropdownUI dropdownPrefab;

    [Header("Buttons / Results")]
    [SerializeField] private Button submitButton;
    [SerializeField] private CaseResultUI resultUI;

    [Header("Scoring")]
    [SerializeField] private int pointsPerCorrectAnswer = 100;
    [SerializeField] private int wrongAnswerPenalty = 20;
    [SerializeField] private float feedbackDuration = 2.5f;

    private readonly List<CaseFileDropdownUI> dropdowns = new();

    private ScamCaseData activeCase;

    private void Awake()
    {
        submitButton.onClick.AddListener(
            () => StartCoroutine(Evaluate())
        );

        root.SetActive(false);
    }

    public void OpenCaseFile()
    {
        activeCase = GameManager.Instance.CurrentCase;

        if (activeCase == null)
        {
            Debug.LogError(
                "CaseFileManager: No active case found.",
                this
            );

            return;
        }

        dropdowns.Clear();

        // Clear old generated dropdowns.
        foreach (Transform child in dropdownContainer)
        {
            Destroy(child.gameObject);
        }

        // -----------------------------
        // CASE INFORMATION
        // -----------------------------

        caseTitleText.text =
            activeCase.caseTitle;

        victimInfoText.text =
            $"{activeCase.victimName}, Age {activeCase.victimAge}\n" +
            activeCase.victimOccupation;

        // -----------------------------
        // PARAGRAPH TEXT SEGMENTS
        // -----------------------------

        for (int i = 0; i < textSegments.Length; i++)
        {
            if (
                activeCase.caseFileTextSegments != null &&
                i < activeCase.caseFileTextSegments.Length
            )
            {
                textSegments[i].text =
                    activeCase.caseFileTextSegments[i];

                textSegments[i].gameObject.SetActive(true);
            }
            else
            {
                textSegments[i].text = "";
                textSegments[i].gameObject.SetActive(false);
            }
        }

        // -----------------------------
        // CREATE DROPDOWNS
        // -----------------------------

        if (activeCase.caseFileDropdowns != null)
        {
            foreach (
                CaseFileDropdownData dropdownData
                in activeCase.caseFileDropdowns
            )
            {
                CaseFileDropdownUI ui =
                    Instantiate(
                        dropdownPrefab,
                        dropdownContainer
                    );

                bool learned =
                    string.IsNullOrWhiteSpace(
                        dropdownData.supportingFactId
                    )
                    ||
                    GameManager.Instance.IsFactUnlocked(
                        dropdownData.supportingFactId
                    );

                ui.Configure(
                    dropdownData,
                    learned
                );

                dropdowns.Add(ui);
            }
        }

        submitButton.interactable = true;

        root.SetActive(true);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    private IEnumerator Evaluate()
    {
        submitButton.interactable = false;

        int correct = 0;
        int wrong = 0;

        foreach (
            CaseFileDropdownUI dropdown
            in dropdowns
        )
        {
            bool ok =
                dropdown.IsCorrect();

            if (ok)
            {
                correct++;
            }
            else
            {
                wrong++;
            }

            dropdown.ShowValidation(ok);
        }

        yield return new WaitForSeconds(
            feedbackDuration
        );

        foreach (
            CaseFileDropdownUI dropdown
            in dropdowns
        )
        {
            if (!dropdown.IsCorrect())
            {
                dropdown.ReplaceWithCorrectAnswer();
            }
        }

        int score =
            Mathf.Max(
                0,
                correct * pointsPerCorrectAnswer
                -
                wrong * wrongAnswerPenalty
            );

        GameManager.Instance.SetCaseScore(
            score,
            wrong
        );

        GameManager.Instance.SetPhase(
            GamePhase.ShowingResult
        );

        root.SetActive(false);

        resultUI.Show(
            activeCase,
            score,
            wrong
        );
    }
}