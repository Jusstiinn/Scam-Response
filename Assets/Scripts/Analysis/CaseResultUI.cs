using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseResultUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText, scoreText, incorrectText, howItWorksText, warningSignsText, preventionText, victimActionText;
    [SerializeField] private Button continueButton;
    [SerializeField] private FinalSummaryUI finalSummaryUI;
    private void Awake() { continueButton.onClick.AddListener(Continue); root.SetActive(false); }
    public void Show(ScamCaseData data, int score, int wrong)
    {
        root.SetActive(true); titleText.text = $"Case Complete: {data.caseTitle}"; scoreText.text = $"Score: {score}"; incorrectText.text = $"Incorrect Answers: {wrong}";
        howItWorksText.text = data.howTheScamWorks; warningSignsText.text = data.warningSigns; preventionText.text = data.howToAvoid; victimActionText.text = data.whatToDoIfVictim;
    }
    private void Continue()
    {
        GameManager.Instance.CompleteCurrentCase(); root.SetActive(false);
        if (CaseManager.Instance.AllCasesCompleted()) { GameManager.Instance.SetPhase(GamePhase.Complete); finalSummaryUI.Show(); }
        else SceneTransitionManager.Instance.LoadScene("Lobby");
    }
}
