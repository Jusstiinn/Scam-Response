using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [SerializeField] private Button continueButton;

    public void Open(string speakerName)
    {
        root.SetActive(true);
        speakerNameText.text = speakerName;
    }

    public void Close()
    {
        ClearChoices();

        continueButton.onClick.RemoveAllListeners();
        continueButton.gameObject.SetActive(false);

        root.SetActive(false);
    }

    public void ShowDecision(
        string line,
        DialogueChoiceData[] choices,
        Action<int> selected)
    {
        dialogueText.text = line;

        ClearChoices();

        // Hide Continue while choices are active
        continueButton.onClick.RemoveAllListeners();
        continueButton.gameObject.SetActive(false);

        if (choices == null || choices.Length == 0)
            return;

        for (int i = 0; i < choices.Length; i++)
        {
            int choiceIndex = i;

            Button button = Instantiate(
                choiceButtonPrefab,
                choiceContainer
            );

            TMP_Text buttonText =
                button.GetComponentInChildren<TMP_Text>(true);

            if (buttonText != null)
            {
                buttonText.text =
                    choices[i].playerChoice;
            }

            button.onClick.AddListener(() =>
            {
                selected?.Invoke(choiceIndex);
            });
        }
    }

    public void ShowResponse(
        string response,
        Action onContinue)
    {
        dialogueText.text = response;

        // Remove the choice buttons
        ClearChoices();

        // Show Continue
        continueButton.gameObject.SetActive(true);

        // VERY IMPORTANT:
        // remove the previous Continue action first
        continueButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            onContinue?.Invoke();
        });
    }

    public void ShowContinueDialogue(
        string line,
        Action onContinue)
    {
        dialogueText.text = line;

        // No choices for this line
        ClearChoices();

        // Show Continue
        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            onContinue?.Invoke();
        });
    }

    private void ClearChoices()
    {
        if (choiceContainer == null)
            return;

        for (int i = choiceContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(choiceContainer.GetChild(i).gameObject);
        }
    }
}