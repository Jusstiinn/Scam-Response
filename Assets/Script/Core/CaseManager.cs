using System.Collections.Generic;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    public static CaseManager Instance { get; private set; }

    [SerializeField] private List<CaseData> availableCases = new List<CaseData>();
    [SerializeField] private bool selectCaseOnStart = true;

    public IReadOnlyList<CaseData> AvailableCases => availableCases;

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

    private void Start()
    {
        if (selectCaseOnStart)
            EnsureCurrentCase();
    }

    public CaseData EnsureCurrentCase()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing.");
            return null;
        }

        if (GameSession.Instance.CurrentCase != null)
            return GameSession.Instance.CurrentCase;

        List<CaseData> remaining = new List<CaseData>();

        foreach (CaseData caseData in availableCases)
        {
            if (caseData != null && !GameSession.Instance.IsCaseCompleted(caseData))
                remaining.Add(caseData);
        }

        if (remaining.Count == 0)
        {
            Debug.Log("All cases have been completed.");
            return null;
        }

        CaseData selected = remaining[Random.Range(0, remaining.Count)];
        GameSession.Instance.SetCurrentCase(selected);
        return selected;
    }

    public bool AreAllCasesCompleted()
    {
        foreach (CaseData caseData in availableCases)
        {
            if (caseData != null && !GameSession.Instance.IsCaseCompleted(caseData))
                return false;
        }

        return true;
    }
}
