using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private ScamCaseData activeCase;
    private int index;
    private Action complete;

    public void StartInterview(
        ScamCaseData data,
        Action onComplete)
    {
        activeCase = data;
        index = 0;
        complete = onComplete;

        if (activeCase == null ||
            activeCase.interviewDecisions == null ||
            activeCase.interviewDecisions.Length == 0)
        {
            FinishInterview();
            return;
        }

        dialogueUI.Open(activeCase.victimName);

        ShowDecision();
    }

    private void ShowDecision()
    {
        var d =
            activeCase.interviewDecisions[index];

        // No choices -> normal dialogue with Continue.
        if (d.choices == null ||
            d.choices.Length == 0)
        {
            dialogueUI.ShowContinueDialogue(
                d.npcLine,
                Continue
            );

            return;
        }

        // Has choices.
        dialogueUI.ShowDecision(
            d.npcLine,
            d.choices,
            Choose
        );
    }

    private void Choose(int optionIndex)
    {
        var c =
            activeCase
                .interviewDecisions[index]
                .choices[optionIndex];

        // Unlock facts.
        if (c.unlockedFactIds != null)
        {
            foreach (var id in c.unlockedFactIds)
            {
                GameManager.Instance.UnlockFact(id);
            }
        }

        // Show the NPC response.
        // Continue button will appear afterwards.
        dialogueUI.ShowResponse(
            c.npcResponse,
            Continue
        );
    }

    private void Continue()
    {
        index++;

        // Finished all interview dialogue.
        if (index >=
            activeCase.interviewDecisions.Length)
        {
            FinishInterview();
            return;
        }

        ShowDecision();
    }

    private void FinishInterview()
    {
        dialogueUI.Close();

        Action callback = complete;
        complete = null;

        callback?.Invoke();
    }
}