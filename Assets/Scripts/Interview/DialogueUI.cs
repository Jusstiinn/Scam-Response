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
    public void ShowDecision(string line, DialogueChoiceData[] choices, Action<int> selected)
    {
        dialogueText.text = line; Clear(); continueButton.gameObject.SetActive(false);
        for (int i = 0; i < choices.Length; i++) { int n = i; var b = Instantiate(choiceButtonPrefab, choiceContainer); b.GetComponentInChildren<TMP_Text>().text = choices[i].playerChoice; b.onClick.AddListener(() => selected(n)); }
    }
    public void ShowResponse(string response, Action next) { dialogueText.text = response; Clear(); continueAction = next; continueButton.gameObject.SetActive(true); }
    private void Clear() { for (int i = choiceContainer.childCount - 1; i >= 0; i--) Destroy(choiceContainer.GetChild(i).gameObject); }
}
