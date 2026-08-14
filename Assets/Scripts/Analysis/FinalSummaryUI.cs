using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalSummaryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;

    [Header("Text")]
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text breakdownText;

    [Header("Button")]
    [SerializeField] private Button continueButton;

    [Header("Next Screen")]
    [SerializeField] private ThankYouUI thankYouUI;

    private void Awake()
    {
        continueButton.onClick.AddListener(Continue);

        root.SetActive(false);
    }

    public void Show()
    {
        // Hide the persistent top-right score HUD
        if (ScoreHUD.Instance != null)
        {
            ScoreHUD.Instance.HideScoreHUD();
        }

        root.SetActive(true);

        totalScoreText.text =
            $"Total Score: {GameManager.Instance.GetTotalScore()}";

        StringBuilder b = new();

    int caseNumber = 1;

    foreach (
        var result
        in GameManager.Instance.CompletedResults
    )
    {
        ScamCaseData data = result.caseData;

        b.AppendLine(
            $"<b>CASE {caseNumber:00} — {result.caseTitle.ToUpper()}</b>"
        );

        if (data != null)
        {
            b.AppendLine(
                $"Victim: {data.victimName}"
            );

            b.AppendLine(
                $"Scam Type: {data.scamType}"
            );
        }

        b.AppendLine();

        b.AppendLine(
            $"<b>Score: {result.score} pts</b>"
        );

        b.AppendLine(
            $"Incorrect Answers: {result.incorrectAnswers}"
        );

        b.AppendLine();

        if (data != null &&
            data.caseFileDropdowns != null &&
            data.caseFileDropdowns.Length > 0)
        {
            b.AppendLine(
                "<b>Correct Case File:</b>"
            );

            foreach (
                CaseFileDropdownData dropdown
                in data.caseFileDropdowns
            )
            {
                if (dropdown == null ||
                    dropdown.options == null ||
                    dropdown.options.Length == 0)
                {
                    continue;
                }

                int correctIndex =
                    dropdown.correctOptionIndex;

                if (correctIndex < 0 ||
                    correctIndex >= dropdown.options.Length)
                {
                    continue;
                }

                string answer =
                    dropdown.options[correctIndex];

                b.AppendLine(
                    $"• {FormatFieldName(dropdown.fieldId)}: {answer}"
                );
            }
        }

        b.AppendLine();

        if (data != null &&
            !string.IsNullOrWhiteSpace(data.howToAvoid))
        {
            b.AppendLine(
                "<b>Key Prevention:</b>"
            );

            b.AppendLine(
                data.howToAvoid
            );
        }

        b.AppendLine();
        b.AppendLine(
            "────────────────────────────"
        );
        b.AppendLine();

        caseNumber++;
    }

    breakdownText.text = b.ToString();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private string FormatFieldName(string fieldId)
    {
        if (string.IsNullOrWhiteSpace(fieldId))
            return "Answer";

        string formatted =
            fieldId.Replace("_", " ");

        return System.Globalization
            .CultureInfo
            .CurrentCulture
            .TextInfo
            .ToTitleCase(formatted);
    }

    private void Continue()
    {
        root.SetActive(false);

        if (thankYouUI != null)
        {
            thankYouUI.Show();
        }
        else
        {
            Debug.LogError(
                "FinalSummaryUI: ThankYouUI has not been assigned.",
                this
            );
        }
    }
}