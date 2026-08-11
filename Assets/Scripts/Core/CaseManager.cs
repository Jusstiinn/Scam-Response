using System.Collections.Generic;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    public static CaseManager Instance { get; private set; }
    [SerializeField] private List<ScamCaseData> cases = new();
    public IReadOnlyList<ScamCaseData> Cases => cases;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public ScamCaseData SelectRandomIncompleteCase()
    {
        if (GameManager.Instance.CurrentCase != null) return GameManager.Instance.CurrentCase;
        List<ScamCaseData> remaining = new();
        foreach (var c in cases) if (c != null && !GameManager.Instance.IsCaseCompleted(c)) remaining.Add(c);
        if (remaining.Count == 0) return null;
        var selected = remaining[Random.Range(0, remaining.Count)];
        GameManager.Instance.StartCase(selected);
        return selected;
    }

    public bool AllCasesCompleted()
    {
        if (cases.Count == 0) return false;
        foreach (var c in cases) if (c != null && !GameManager.Instance.IsCaseCompleted(c)) return false;
        return true;
    }
}
