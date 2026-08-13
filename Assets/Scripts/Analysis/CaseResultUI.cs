using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseResultUI : MonoBehaviour
{
    [SerializeField] private GameObject root;

    [Header("Result Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text incorrectText;

    [Header("Education")]
    [SerializeField] private TMP_Text howItWorksText;
    [SerializeField] private TMP_Text warningSignsText;
    [SerializeField] private TMP_Text preventionText;
    [SerializeField] private TMP_Text victimActionText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Final Screen")]
    [SerializeField] private FinalSummaryUI finalSummaryUI;

    private void Awake()
    {
        continueButton.onClick.AddListener(Continue);
        root.SetActive(false);
    }

    public void Show(
        ScamCaseData data,
        int score,
        int wrong)
    {
        root.SetActive(true);

        titleText.text =
            $"Case Complete: {data.caseTitle}";

        incorrectText.text =
            $"Incorrect Answers: {wrong}";

        howItWorksText.text =
            data.howTheScamWorks;

        warningSignsText.text =
            data.warningSigns;

        preventionText.text =
            data.howToAvoid;

        victimActionText.text =
            data.whatToDoIfVictim;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    private void Continue()
    {
        GameManager.Instance.CompleteCurrentCase();

        root.SetActive(false);

        if (CaseManager.Instance.AllCasesCompleted())
        {
            GameManager.Instance.SetPhase(
                GamePhase.Complete
            );

            finalSummaryUI.Show();
        }
        else
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;

            SceneTransitionManager.Instance.LoadScene(
                "Lobby"
            );
        }
    }
}