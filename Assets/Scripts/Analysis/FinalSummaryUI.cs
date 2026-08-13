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

        foreach (
            var result
            in GameManager.Instance.CompletedResults
        )
        {
            b.AppendLine(
                $"{result.caseTitle}: " +
                $"{result.score} points, " +
                $"{result.incorrectAnswers} incorrect"
            );
        }

        breakdownText.text = b.ToString();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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