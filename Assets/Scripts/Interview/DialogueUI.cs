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
    private Action continueAction;
    private void Awake() { continueButton.onClick.AddListener(() => { var a = continueAction; continueAction = null; a?.Invoke(); }); Close(); }
    public void Open(string speaker) { speakerNameText.text = speaker; root.SetActive(true); }
    public void Close() { root.SetActive(false); Clear(); continueButton.gameObject.SetActive(false); }

    public void ShowContinueDialogue(
    string line,
    Action onContinue)
    {
        dialogueText.text = line;

        Clear();

        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            onContinue?.Invoke();
        });
    }

    public void ShowDecision(
    string line,
    DialogueChoiceData[] choices,
    Action<int> selected)
    {
        dialogueText.text = line;

        Clear();

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        Debug.Log(
            $"ShowDecision called. " +
            $"Choices = {(choices == null ? "NULL" : choices.Length.ToString())}"
        );

        if (choiceContainer == null)
        {
            Debug.LogError(
                "DialogueUI: Choice Container is NULL!",
                this
            );
            return;
        }

        if (choiceButtonPrefab == null)
        {
            Debug.LogError(
                "DialogueUI: Choice Button Prefab is NULL!",
                this
            );
            return;
        }

        if (choices == null || choices.Length == 0)
        {
            Debug.LogError(
                "DialogueUI: Current interview decision has NO choices!",
                this
            );
            return;
        }

        for (int i = 0; i < choices.Length; i++)
        {
            int choiceIndex = i;

            Button button =
                Instantiate(
                    choiceButtonPrefab,
                    choiceContainer
                );

            Debug.Log(
                $"Spawned choice button {i}: {button.name}",
                button
            );

            TMP_Text text =
                button.GetComponentInChildren<TMP_Text>(true);

            if (text == null)
            {
                Debug.LogError(
                    "DialogueUI: ChoiceButton prefab has no TMP_Text child!",
                    button
                );

                continue;
            }

            text.text =
                choices[i].playerChoice;

            button.onClick.AddListener(
                () => selected(choiceIndex)
            );

            button.gameObject.SetActive(true);
        }
    }
    public void ShowResponse(string response, Action next) { dialogueText.text = response; Clear(); continueAction = next; continueButton.gameObject.SetActive(true); }
    private void Clear() { for (int i = choiceContainer.childCount - 1; i >= 0; i--) Destroy(choiceContainer.GetChild(i).gameObject); }
}
