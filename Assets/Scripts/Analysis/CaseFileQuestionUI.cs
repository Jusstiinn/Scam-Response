using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseFileQuestionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text supportHintText;
    private CaseFileQuestionData data;
    public void Configure(CaseFileQuestionData value, bool learned)
    {
        data = value; promptText.text = data.prompt; dropdown.ClearOptions(); dropdown.AddOptions(new List<string>(data.options)); dropdown.value = 0;
        if (supportHintText != null) { supportHintText.gameObject.SetActive(!learned); supportHintText.text = learned ? "" : "Some useful interview information was not obtained."; }
        background.color = Color.white; dropdown.interactable = true;
    }
    public bool IsCorrect() => dropdown.value == data.correctOptionIndex;
    public void ShowValidation(bool correct) { background.color = correct ? Color.green : Color.red; dropdown.interactable = false; }
    public void ReplaceWithCorrectAnswer() { dropdown.value = data.correctOptionIndex; dropdown.RefreshShownValue(); background.color = Color.green; }
}
