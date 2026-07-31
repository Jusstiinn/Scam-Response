using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EducationPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scamWorksText;
    [SerializeField] private TMP_Text warningSignsText;
    [SerializeField] private TMP_Text preventionText;
    [SerializeField] private TMP_Text victimActionText;
    [SerializeField] private Button continueButton;
    [SerializeField] private string lobbySceneName = "Lobby";

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(Continue);

        if (root != null)
            root.SetActive(false);
    }

    public void Show(CaseData caseData)
    {
        if (caseData == null)
            return;

        root.SetActive(true);

        if (titleText != null)
            titleText.text = caseData.caseTitle;

        if (scamWorksText != null)
            scamWorksText.text = caseData.howTheScamWorks;

        if (warningSignsText != null)
            warningSignsText.text = caseData.warningSigns;

        if (preventionText != null)
            preventionText.text = caseData.preventionAdvice;

        if (victimActionText != null)
            victimActionText.text = caseData.whatToDoIfVictim;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Continue()
    {
        if (InvestigationManager.Instance != null)
            InvestigationManager.Instance.FinishCaseAndReturn(lobbySceneName);
    }
}
