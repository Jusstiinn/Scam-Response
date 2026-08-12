using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseFileDropdownUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown dropDown;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text supportHintText;

    private CaseFileDropdownData data;

    public void Configure(CaseFileDropdownData value, bool learned)
    {
        data = value;

        dropDown.ClearOptions();

        dropDown.AddOptions(
            new List<string>(data.options)
        );

        dropDown.value = 0;
        dropDown.RefreshShownValue();

        if (supportHintText != null)
        {
            supportHintText.gameObject.SetActive(!learned);

            supportHintText.text = learned
                ? ""
                : "Some useful interview information was not obtained.";
        }

        if (background != null)
        {
            background.color = Color.white;
        }

        dropDown.interactable = true;
    }

    public bool IsCorrect()
    {
        return data != null &&
               dropDown.value == data.correctOptionIndex;
    }

    public void ShowValidation(bool correct)
    {
        if (background != null)
        {
            background.color =
                correct ? Color.green : Color.red;
        }

        dropDown.interactable = false;
    }

    public void ReplaceWithCorrectAnswer()
    {
        if (data == null)
            return;

        dropDown.value = data.correctOptionIndex;
        dropDown.RefreshShownValue();

        if (background != null)
        {
            background.color = Color.green;
        }
    }
}