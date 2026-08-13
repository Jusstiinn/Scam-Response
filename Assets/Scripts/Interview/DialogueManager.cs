using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private ScamCaseData activeCase;
    private int index;
    private Action complete;

    public void StartInterview(ScamCaseData data, Action onComplete)
    {
        activeCase = data;
        index = 0;
        complete = onComplete;

        if (activeCase.interviewDecisions == null ||
            activeCase.interviewDecisions.Length == 0)
        {
            complete?.Invoke();
            return;
        }

        dialogueUI.Open(activeCase.victimName);
        ShowDecision();
    }

    private void ShowDecision()
    {
        var d = activeCase.interviewDecisions[index];

        // No choices → show dialogue with Continue button
        if (d.choices == null || d.choices.Length == 0)
        {
            dialogueUI.ShowContinueDialogue(
                d.npcLine,
                Continue
            );

            return;
        }

        // Has choices → show choice buttons
        dialogueUI.ShowDecision(
            d.npcLine,
            d.choices,
            Choose
        );
    }

    private void Choose(int optionIndex)
    {
        var c = activeCase
            .interviewDecisions[index]
            .choices[optionIndex];

        if (c.unlockedFactIds != null)
        {
            foreach (var id in c.unlockedFactIds)
            {
                GameManager.Instance.UnlockFact(id);
            }
        }

        // Show NPC response, then Continue
        dialogueUI.ShowResponse(
            c.npcResponse,
            Continue
        );
    }

    private void Continue()
    {
        index++;

        if (index >= activeCase.interviewDecisions.Length)
        {
            dialogueUI.Close();
            complete?.Invoke();
        }
        else
        {
            ShowDecision();
        }
    }
}