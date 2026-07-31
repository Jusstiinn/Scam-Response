using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;

    private CaseData activeCase;
    private int questionIndex;
    private bool showingResponse;

    private void Awake()
    {
        Instance = this;
    }

    public void StartInterview(CaseData caseData)
    {
        if (caseData == null || caseData.interviewQuestions == null ||
            caseData.interviewQuestions.Length == 0)
        {
            Debug.LogWarning("This case has no interview questions.");
            return;
        }

        activeCase = caseData;
        questionIndex = 0;
        showingResponse = false;

        dialogueUI.Open(caseData.victimName);
        ShowCurrentQuestion();
    }

    private void ShowCurrentQuestion()
    {
        InterviewQuestion question = activeCase.interviewQuestions[questionIndex];
        dialogueUI.ShowQuestion(question.npcQuestion, question.choices, SelectChoice);
    }

    private void SelectChoice(int choiceIndex)
    {
        if (showingResponse)
            return;

        InterviewQuestion question = activeCase.interviewQuestions[questionIndex];

        if (choiceIndex < 0 || choiceIndex >= question.choices.Length)
            return;

        InterviewChoice choice = question.choices[choiceIndex];
        showingResponse = true;

        if (choice.unlockEvidenceIds != null && GameSession.Instance != null)
        {
            foreach (string evidenceId in choice.unlockEvidenceIds)
                GameSession.Instance.UnlockEvidence(evidenceId);
        }

        dialogueUI.ShowResponse(choice.npcResponse, ContinueInterview);
    }

    private void ContinueInterview()
    {
        showingResponse = false;
        questionIndex++;

        if (questionIndex >= activeCase.interviewQuestions.Length)
        {
            dialogueUI.Close();
            return;
        }

        ShowCurrentQuestion();
    }
}
