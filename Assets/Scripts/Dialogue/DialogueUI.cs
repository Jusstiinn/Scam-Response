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

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        Close();
    }

    public void Open(string speakerName)
    {
        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        root.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);

        ClearChoices();

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowQuestion(
        string questionText,
        InterviewChoice[] choices,
        Action<int> onChoiceSelected)
    {
        dialogueText.text = questionText;
        ClearChoices();

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (choices == null)
            return;

        for (int i = 0; i < choices.Length; i++)
        {
            int capturedIndex = i;
            Button button = Instantiate(choiceButtonPrefab, choiceContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = choices[i].playerChoice;

            button.onClick.AddListener(() => onChoiceSelected?.Invoke(capturedIndex));
        }
    }

    public void ShowResponse(string responseText, Action onContinue)
    {
        dialogueText.text = responseText;
        ClearChoices();
        continueAction = onContinue;

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);
    }

    private void OnContinueClicked()
    {
        Action action = continueAction;
        continueAction = null;
        action?.Invoke();
    }

    private void ClearChoices()
    {
        if (choiceContainer == null)
            return;

        for (int i = choiceContainer.childCount - 1; i >= 0; i--)
            Destroy(choiceContainer.GetChild(i).gameObject);
    }
}
