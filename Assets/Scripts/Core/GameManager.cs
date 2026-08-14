using System;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase { Reception, NpcFollowing, Interview, ReadyForAnalysis, Analysing, ShowingResult, Complete }

[Serializable]
public class CompletedCaseResult
{
    public string caseId;
    public string caseTitle;
    public int score;
    public int incorrectAnswers;

    public ScamCaseData caseData;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ScamCaseData CurrentCase { get; private set; }
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Reception;
    public int CurrentCaseScore { get; private set; }
    public int CurrentIncorrectAnswers { get; private set; }
    public int TotalScore { get; private set; }
    public IReadOnlyList<CompletedCaseResult> CompletedResults => completedResults;

    private readonly HashSet<string> unlockedFacts = new();
    private readonly HashSet<string> completedCaseIds = new();
    private readonly List<CompletedCaseResult> completedResults = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartCase(ScamCaseData caseData)
    {
        CurrentCase = caseData;
        unlockedFacts.Clear();
        CurrentCaseScore = 0;
        CurrentIncorrectAnswers = 0;
        CurrentPhase = GamePhase.Reception;
    }

    public void SetPhase(GamePhase phase) => CurrentPhase = phase;
    public void UnlockFact(string factId) { if (!string.IsNullOrWhiteSpace(factId)) unlockedFacts.Add(factId); }
    public bool IsFactUnlocked(string factId) => unlockedFacts.Contains(factId);

    public void SetCaseScore(
        int score,
        int incorrectAnswers)
    {
        CurrentCaseScore =
            Mathf.Max(0, score);

        CurrentIncorrectAnswers =
            Mathf.Max(0, incorrectAnswers);
    }

    public void AddCurrentCaseScoreToTotal()
    {
        TotalScore += CurrentCaseScore;
    }

    public void CompleteCurrentCase()
    {
        if (CurrentCase == null) return;
        if (!string.IsNullOrWhiteSpace(CurrentCase.caseId)) completedCaseIds.Add(CurrentCase.caseId);
        completedResults.Add(new CompletedCaseResult
        {
            caseId = CurrentCase.caseId,
            caseTitle = CurrentCase.caseTitle,
            score = CurrentCaseScore,
            incorrectAnswers = CurrentIncorrectAnswers,
            caseData = CurrentCase
        });
        CurrentCase = null;
        unlockedFacts.Clear();
        CurrentPhase = GamePhase.Reception;
    }

    public bool IsCaseCompleted(ScamCaseData data) => data != null && completedCaseIds.Contains(data.caseId);
    public int GetTotalScore()
    {
        return TotalScore;
    }
    public void ResetGame()
    {
        CurrentCase = null;

        unlockedFacts.Clear();
        completedCaseIds.Clear();
        completedResults.Clear();

        CurrentCaseScore = 0;
        CurrentIncorrectAnswers = 0;
        TotalScore = 0;

        CurrentPhase =
            GamePhase.Reception;

        if (ScoreHUD.Instance != null)
        {
            ScoreHUD.Instance.ResetScoreHUD();
        }
    }
}
