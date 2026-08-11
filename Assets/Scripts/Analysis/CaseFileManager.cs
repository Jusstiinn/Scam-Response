using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseFileManager : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text caseTitleText;
    [SerializeField] private TMP_Text victimInfoText;
    [SerializeField] private Transform questionContainer;
    [SerializeField] private CaseFileQuestionUI questionPrefab;
    [SerializeField] private Button submitButton;
    [SerializeField] private CaseResultUI resultUI;
    [SerializeField] private int pointsPerCorrectAnswer = 100;
    [SerializeField] private int wrongAnswerPenalty = 20;
    [SerializeField] private float feedbackDuration = 2.5f;
    private readonly List<CaseFileQuestionUI> questions = new();
    private ScamCaseData activeCase;

    private void Awake() { submitButton.onClick.AddListener(() => StartCoroutine(Evaluate())); root.SetActive(false); }
    public void OpenCaseFile()
    {
        activeCase = GameManager.Instance.CurrentCase; questions.Clear();
        foreach (Transform child in questionContainer) Destroy(child.gameObject);
        caseTitleText.text = activeCase.caseTitle; victimInfoText.text = $"{activeCase.victimName}, Age {activeCase.victimAge}\n{activeCase.victimOccupation}";
        foreach (var q in activeCase.caseFileQuestions)
        {
            var ui = Instantiate(questionPrefab, questionContainer);
            bool learned = string.IsNullOrWhiteSpace(q.supportingFactId) || GameManager.Instance.IsFactUnlocked(q.supportingFactId);
            ui.Configure(q, learned); questions.Add(ui);
        }
        submitButton.interactable = true; root.SetActive(true); Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }
    private IEnumerator Evaluate()
    {
        submitButton.interactable = false; int correct = 0, wrong = 0;
        foreach (var q in questions) { bool ok = q.IsCorrect(); if (ok) correct++; else wrong++; q.ShowValidation(ok); }
        yield return new WaitForSeconds(feedbackDuration);
        foreach (var q in questions) if (!q.IsCorrect()) q.ReplaceWithCorrectAnswer();
        int score = Mathf.Max(0, correct * pointsPerCorrectAnswer - wrong * wrongAnswerPenalty);
        GameManager.Instance.SetCaseScore(score, wrong); GameManager.Instance.SetPhase(GamePhase.ShowingResult);
        root.SetActive(false); resultUI.Show(activeCase, score, wrong);
    }
}
