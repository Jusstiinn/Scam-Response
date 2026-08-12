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

    public ScamCaseData SelectNextIncompleteCase()
    {
        if (GameManager.Instance.CurrentCase != null)
            return GameManager.Instance.CurrentCase;

        // Go through the cases in Inspector order.
        for (int i = 0; i < cases.Count; i++)
        {
            ScamCaseData c = cases[i];

            if (c != null &&
                !GameManager.Instance.IsCaseCompleted(c))
            {
                GameManager.Instance.StartCase(c);
                return c;
            }
        }

        return null;
    }

    public bool AllCasesCompleted()
    {
        if (cases.Count == 0) return false;
        foreach (var c in cases) if (c != null && !GameManager.Instance.IsCaseCompleted(c)) return false;
        return true;
    }
}
