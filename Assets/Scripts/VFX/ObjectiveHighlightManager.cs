using System.Collections.Generic;
using UnityEngine;

public class ObjectiveHighlightManager : MonoBehaviour
{
    public static ObjectiveHighlightManager Instance { get; private set; }

    [Header("Highlight Material")]
    [SerializeField] private Material highlightMaterial;

    [Header("Objective Order")]
    [SerializeField]
    private List<string> objectiveOrder = new List<string>()
    {
        "ReceptionButton",
        "InterviewDoor",
        "PlayerSeat",
        "AnalysisDoor",
        "Monitor"
    };

    [Header("Case Restriction")]
    [SerializeField] private string firstCaseId = "C01";

    private int currentObjectiveIndex = 0;

    private bool IsFirstCase()
    {
        if (GameManager.Instance == null)
            return false;

        if (GameManager.Instance.CurrentCase == null)
            return false;

        return GameManager.Instance.CurrentCase.caseId == firstCaseId;
    }

    private readonly Dictionary<string, ObjectiveHighlightTarget>
        registeredTargets =
        new Dictionary<string, ObjectiveHighlightTarget>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // If this object is already under your persistent
        // DontDestroyOnLoad parent, leave this commented out.
        //
        // DontDestroyOnLoad(gameObject);
    }

    public void RefreshCurrentHighlight()
    {
        RefreshHighlight();
    }

    public void RegisterTarget(ObjectiveHighlightTarget target)
    {
        if (target == null)
            return;

        if (string.IsNullOrEmpty(target.ObjectiveID))
            return;

        registeredTargets[target.ObjectiveID] = target;

        RefreshHighlight();
    }

    public void UnregisterTarget(ObjectiveHighlightTarget target)
    {
        if (target == null)
            return;

        if (registeredTargets.TryGetValue(
            target.ObjectiveID,
            out ObjectiveHighlightTarget registered))
        {
            if (registered == target)
            {
                registeredTargets.Remove(
                    target.ObjectiveID
                );
            }
        }
    }

    public void CompleteObjective(string objectiveID)
    {
        if (currentObjectiveIndex >= objectiveOrder.Count)
            return;

        string expectedObjective =
            objectiveOrder[currentObjectiveIndex];

        if (objectiveID != expectedObjective)
            return;

        if (registeredTargets.TryGetValue(
            objectiveID,
            out ObjectiveHighlightTarget currentTarget))
        {
            currentTarget.RestoreOriginalMaterial();
        }

        currentObjectiveIndex++;

        RefreshHighlight();
    }

    private void RefreshHighlight()
    {
        foreach (ObjectiveHighlightTarget targett
                in registeredTargets.Values)
        {
            if (targett != null)
            {
                targett.RestoreOriginalMaterial();
            }
        }

        // Only highlight during first case.
        if (!IsFirstCase())
            return;

        if (currentObjectiveIndex >= objectiveOrder.Count)
            return;

        string currentObjective =
            objectiveOrder[currentObjectiveIndex];

        if (registeredTargets.TryGetValue(
            currentObjective,
            out ObjectiveHighlightTarget target))
        {
            target.ApplyHighlight(
                highlightMaterial
            );

            Debug.Log(
                "Highlighting objective: " +
                currentObjective
            );
        }
        else
        {
            Debug.LogWarning(
                "No registered target for: " +
                currentObjective
            );
        }
    }

    public string GetCurrentObjectiveID()
    {
        if (currentObjectiveIndex >= objectiveOrder.Count)
            return "";

        return objectiveOrder[currentObjectiveIndex];
    }

    public bool IsCurrentObjective(string objectiveID)
    {
        return GetCurrentObjectiveID() == objectiveID;
    }
}