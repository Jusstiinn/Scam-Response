using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestigationManager : MonoBehaviour
{
    public static InvestigationManager Instance { get; private set; }

    [Header("Prefabs and Containers")]
    [SerializeField] private Transform evidenceContainer;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private EvidenceItemUI evidenceItemPrefab;
    [SerializeField] private EvidenceDropZone evidenceSlotPrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text caseTitleText;
    [SerializeField] private TMP_Text victimDetailsText;
    [SerializeField] private Button submitButton;
    [SerializeField] private EducationPanel educationPanel;

    [Header("Scoring")]
    [SerializeField] private int correctScore = 100;
    [SerializeField] private int wrongPenalty = -20;
    [SerializeField] private float feedbackFlashDuration = 0.6f;

    public Transform EvidenceContainer => evidenceContainer;

    private readonly Dictionary<string, EvidenceDropZone> zones =
        new Dictionary<string, EvidenceDropZone>();

    private readonly Dictionary<string, EvidenceItemUI> items =
        new Dictionary<string, EvidenceItemUI>();

    private readonly Dictionary<string, string> placements =
        new Dictionary<string, string>();

    private CaseData activeCase;
    private bool submitted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(Submit);

        BuildCurrentCase();
    }

    public void BuildCurrentCase()
    {
        activeCase = GameSession.Instance?.CurrentCase;

        if (activeCase == null)
        {
            Debug.LogError("No active case is available for investigation.");
            return;
        }

        if (caseTitleText != null)
            caseTitleText.text = activeCase.caseTitle;

        if (victimDetailsText != null)
        {
            victimDetailsText.text =
                $"{activeCase.victimName}, Age {activeCase.victimAge}\n" +
                activeCase.victimOccupation;
        }

        BuildSlots();
        BuildEvidence();
    }

    private void BuildSlots()
    {
        zones.Clear();

        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        if (activeCase.evidenceSlots == null)
            return;

        foreach (EvidenceSlotDefinition slot in activeCase.evidenceSlots)
        {
            EvidenceDropZone zone = Instantiate(evidenceSlotPrefab, slotContainer);
            zone.Configure(slot);
            zones[slot.slotId] = zone;
        }
    }

    private void BuildEvidence()
    {
        items.Clear();
        placements.Clear();

        foreach (Transform child in evidenceContainer)
            Destroy(child.gameObject);

        if (activeCase.evidence == null)
            return;

        foreach (EvidenceEntry evidence in activeCase.evidence)
        {
            bool unlocked = evidence.unlockedByDefault ||
                            GameSession.Instance.IsEvidenceUnlocked(evidence.evidenceId);

            if (!unlocked)
                continue;

            EvidenceItemUI item = Instantiate(evidenceItemPrefab, evidenceContainer);
            item.Configure(evidence);
            items[evidence.evidenceId] = item;
        }
    }

    public void RegisterPlacement(EvidenceItemUI item, EvidenceDropZone zone)
    {
        if (item?.Evidence == null || zone == null)
            return;

        placements[item.Evidence.evidenceId] = zone.SlotId;
    }

    public void Submit()
    {
        if (submitted || activeCase == null)
            return;

        submitted = true;

        if (submitButton != null)
            submitButton.interactable = false;

        StartCoroutine(EvaluateRoutine());
    }

    private IEnumerator EvaluateRoutine()
    {
        foreach (EvidenceEntry evidence in activeCase.evidence)
        {
            if (!items.TryGetValue(evidence.evidenceId, out EvidenceItemUI item))
                continue;

            bool correct =
                placements.TryGetValue(evidence.evidenceId, out string placedSlot) &&
                placedSlot == evidence.correctSlotId;

            ScoreManager.Instance?.AddScore(correct ? correctScore : wrongPenalty);

            yield return item.Flash(correct, feedbackFlashDuration);

            if (!correct && zones.TryGetValue(evidence.correctSlotId, out EvidenceDropZone correctZone))
            {
                yield return item.FadeOutIn();
                correctZone.PlaceItem(item);
            }
        }

        if (educationPanel != null)
            educationPanel.Show(activeCase);
    }

    public void FinishCaseAndReturn(string lobbySceneName)
    {
        GameSession.Instance?.CompleteCurrentCase();
        SceneTransitionManager.Instance?.LoadScene(lobbySceneName);
    }
}
