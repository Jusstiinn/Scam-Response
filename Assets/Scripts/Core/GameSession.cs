using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public CaseData CurrentCase { get; private set; }
    public IReadOnlyCollection<string> CompletedCaseIds => completedCaseIds;

    private readonly HashSet<string> completedCaseIds = new HashSet<string>();
    private readonly HashSet<string> unlockedEvidenceIds = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentCase(CaseData caseData)
    {
        CurrentCase = caseData;
        unlockedEvidenceIds.Clear();

        if (caseData == null || caseData.evidence == null)
            return;

        foreach (EvidenceEntry entry in caseData.evidence)
        {
            if (entry != null && entry.unlockedByDefault && !string.IsNullOrWhiteSpace(entry.evidenceId))
                unlockedEvidenceIds.Add(entry.evidenceId);
        }
    }

    public void UnlockEvidence(string evidenceId)
    {
        if (!string.IsNullOrWhiteSpace(evidenceId))
            unlockedEvidenceIds.Add(evidenceId);
    }

    public bool IsEvidenceUnlocked(string evidenceId)
    {
        return unlockedEvidenceIds.Contains(evidenceId);
    }

    public void CompleteCurrentCase()
    {
        if (CurrentCase != null && !string.IsNullOrWhiteSpace(CurrentCase.caseId))
            completedCaseIds.Add(CurrentCase.caseId);

        CurrentCase = null;
        unlockedEvidenceIds.Clear();
    }

    public bool IsCaseCompleted(CaseData caseData)
    {
        return caseData != null &&
               !string.IsNullOrWhiteSpace(caseData.caseId) &&
               completedCaseIds.Contains(caseData.caseId);
    }

    public void ResetRun()
    {
        CurrentCase = null;
        completedCaseIds.Clear();
        unlockedEvidenceIds.Clear();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();
    }
}
