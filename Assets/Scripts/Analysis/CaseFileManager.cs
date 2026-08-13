using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseFileManager : MonoBehaviour
{

    [Header("Main UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text caseTitleText;
    [SerializeField] private TMP_Text victimInfoText;

    [Header("Paragraph")]
    [SerializeField] private Transform paragraphContainer;
    [SerializeField] private TMP_Text textPrefab;
    [SerializeField] private CaseFileDropdownUI dropdownPrefab;

    [Header("Buttons / Results")]
    [SerializeField] private Button submitButton;
    [SerializeField] private CaseResultUI resultUI;

    [Header("Player")]
    [SerializeField] private Behaviour firstPersonController;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject queueNumberCanvas;
    [SerializeField] private GameObject interactionCanvas;
    [SerializeField] private GameObject crosshairCanvas;

    [Header("Scoring")]
    [SerializeField] private int pointsPerCorrectAnswer = 100;
    [SerializeField] private int wrongAnswerPenalty = 20;
    [SerializeField] private float feedbackDuration = 2.5f;

    [Header("Monitor Score VFX")]
    [SerializeField] private GameObject monitorVfxPrefab;
    [SerializeField] private Transform monitorVfxSpawnPoint;
    [SerializeField] private float monitorVfxLifetime = 3f;

    private GameObject activeMonitorVfx;

    private readonly List<CaseFileDropdownUI> dropdowns = new();

    private ScamCaseData activeCase;

    private void PlayMonitorVfx()
    {
        if (monitorVfxPrefab == null)
            return;

        Transform spawn =
            monitorVfxSpawnPoint != null
                ? monitorVfxSpawnPoint
                : transform;

        GameObject vfx = Instantiate(
            monitorVfxPrefab,
            spawn.position,
            spawn.rotation
        );

        Destroy(
            vfx,
            monitorVfxLifetime
        );
    }

    private void Awake()
    {
        submitButton.onClick.AddListener(
            () => StartCoroutine(Evaluate())
        );

        root.SetActive(false);
    }

    public void OpenCaseFile()
    {
        activeCase = GameManager.Instance.CurrentCase;

        if (activeCase == null)
        {
            Debug.LogError(
                "CaseFileManager: No active case found.",
                this
            );

            return;
        }

        // Clear old generated UI
        foreach (Transform child in paragraphContainer)
        {
            Destroy(child.gameObject);
        }

        dropdowns.Clear();

        // Case information
        caseTitleText.text =
            activeCase.caseTitle;

        victimInfoText.text =
            $"{activeCase.victimName}, Age {activeCase.victimAge}\n" +
            activeCase.victimOccupation;

        // -----------------------------
        // BUILD PARAGRAPH + DROPDOWNS
        // -----------------------------

        if (activeCase.caseFileParagraph != null)
        {
            for (int i = 0; i < activeCase.caseFileParagraph.Length; i++)
            {
                CaseFileParagraphPart part =
                    activeCase.caseFileParagraph[i];

                // Spawn paragraph text
                if (!string.IsNullOrWhiteSpace(part.textBefore))
                {
                    TMP_Text text =
                        Instantiate(
                            textPrefab,
                            paragraphContainer
                        );

                    text.text = part.textBefore;
                }

                // Spawn a dropdown after this paragraph section
                // ONLY if a matching dropdown exists.
                if (activeCase.caseFileDropdowns != null &&
                    i < activeCase.caseFileDropdowns.Length)
                {
                    CaseFileDropdownData dropdownData =
                        activeCase.caseFileDropdowns[i];

                    CaseFileDropdownUI ui =
                        Instantiate(
                            dropdownPrefab,
                            paragraphContainer
                        );

                    bool learned =
                        string.IsNullOrWhiteSpace(
                            dropdownData.supportingFactId
                        )
                        ||
                        GameManager.Instance.IsFactUnlocked(
                            dropdownData.supportingFactId
                        );

                    ui.Configure(
                        dropdownData,
                        learned
                    );

                    dropdowns.Add(ui);
                }
            }
        }

        submitButton.interactable = true;

        root.SetActive(true);

        // Freeze normal FPS gameplay
        EnterAnalysisMode();
    }

    private IEnumerator Evaluate()
    {
        submitButton.interactable = false;

        int correct = 0;
        int wrong = 0;

        foreach (
            CaseFileDropdownUI dropdown
            in dropdowns
        )
        {
            bool ok =
                dropdown.IsCorrect();

            if (ok)
                correct++;
            else
                wrong++;

            dropdown.ShowValidation(ok);
        }

        yield return new WaitForSeconds(
            feedbackDuration
        );

        foreach (
            CaseFileDropdownUI dropdown
            in dropdowns
        )
        {
            if (!dropdown.IsCorrect())
            {
                dropdown.ReplaceWithCorrectAnswer();
            }
        }

        int score =
            Mathf.Max(
                0,
                correct * pointsPerCorrectAnswer
                -
                wrong * wrongAnswerPenalty
            );

        GameManager.Instance.SetCaseScore(
            score,
            wrong
        );

        GameManager.Instance.SetPhase(
            GamePhase.ShowingResult
        );

        root.SetActive(false);

        PlayMonitorVfx();

        //Restore FPS movement/HUD.
        ExitAnalysisMode();

        //Calculate score transition.
        int previousTotal =
            GameManager.Instance.TotalScore;

        GameManager.Instance
            .AddCurrentCaseScoreToTotal();

        int newTotal =
            GameManager.Instance.TotalScore;

        /*
        * Animate score first.
        * Result screen appears afterwards.
        */
        if (ScoreHUD.Instance != null)
        {
            ScoreHUD.Instance.AnimateScore(
                previousTotal,
                newTotal,
                () =>
                {
                    resultUI.Show(
                        activeCase,
                        score,
                        wrong
                    );
                }
            );
        }
        else
        {
            /*
            * Safety fallback if ScoreHUD
            * wasn't added to the scene.
            */
            resultUI.Show(
                activeCase,
                score,
                wrong
            );
        }
    }

        private void EnterAnalysisMode()
    {
        if (firstPersonController != null)
            firstPersonController.enabled = false;

        if (playerInteraction != null)
            playerInteraction.enabled = false;

        if (queueNumberCanvas != null)
            queueNumberCanvas.SetActive(false);

        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        if (crosshairCanvas != null)
            crosshairCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ExitAnalysisMode()
    {
        if (firstPersonController != null)
            firstPersonController.enabled = true;

        if (playerInteraction != null)
            playerInteraction.enabled = true;

        if (queueNumberCanvas != null)
            queueNumberCanvas.SetActive(true);

        if (interactionCanvas != null)
            interactionCanvas.SetActive(true);

        if (crosshairCanvas != null)
            crosshairCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}