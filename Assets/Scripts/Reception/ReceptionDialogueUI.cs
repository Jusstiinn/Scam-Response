using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class ReceptionDialogueUI : MonoBehaviour
{
    public static ReceptionDialogueUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button continueButton;
    [Header("Player")]
    [SerializeField] private FirstPersonController firstPersonController;

    private Action onDialogueFinished;
    private bool dialogueOpen;

    private void Awake()
    {
        Instance = this;

        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueDialogue);
    }

    public void ShowDialogue(
        string speakerName,
        string message,
        Action finishedCallback = null)
    {
        if (dialogueRoot == null)
        {
            Debug.LogError(
                "ReceptionDialogueUI: Dialogue Root is not assigned.",
                this
            );
            return;
        }

        if (speakerNameText == null)
        {
            Debug.LogError(
                "ReceptionDialogueUI: Speaker Name Text is not assigned.",
                this
            );
            return;
        }

        if (dialogueText == null)
        {
            Debug.LogError(
                "ReceptionDialogueUI: Dialogue Text is not assigned.",
                this
            );
            return;
        }

        speakerNameText.text = speakerName;
        dialogueText.text = message;

        onDialogueFinished = finishedCallback;
        dialogueOpen = true;

        dialogueRoot.SetActive(true);

        // Free the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement + camera control
        if (firstPersonController != null)
        {
            firstPersonController.enabled = false;
        }
    }

    public void ContinueDialogue()
    {
        if (!dialogueOpen)
            return;

        dialogueOpen = false;

        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);

        // Re-enable player controls
        if (firstPersonController != null)
        {
            firstPersonController.enabled = true;
        }

        // Lock mouse back to gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Action callback = onDialogueFinished;
        onDialogueFinished = null;

        callback?.Invoke();
    }

    public void CloseDialogue()
    {
        dialogueOpen = false;

        if (dialogueRoot != null)
            dialogueRoot.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onDialogueFinished = null;
    }

    public bool IsDialogueOpen()
    {
        return dialogueOpen;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueDialogue);
    }
}
