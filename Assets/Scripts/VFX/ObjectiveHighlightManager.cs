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

    private int currentObjectiveIndex = 0;

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

        // Prevent objectives being completed out of order.
        if (objectiveID != expectedObjective)
        {
            Debug.Log(
                "ObjectiveHighlightManager: Tried to complete " +
                objectiveID +
                " but current objective is " +
                expectedObjective
            );

            return;
        }

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
        // Remove highlight from every currently registered object.
        foreach (ObjectiveHighlightTarget targett in registeredTargets.Values)
        {
            if (targett != null)
                targett.RestoreOriginalMaterial();
        }

        if (currentObjectiveIndex >= objectiveOrder.Count)
        {
            Debug.Log(
                "ObjectiveHighlightManager: All objectives completed."
            );

            return;
        }

        string currentObjective =
            objectiveOrder[currentObjectiveIndex];

        if (registeredTargets.TryGetValue(
            currentObjective,
            out ObjectiveHighlightTarget target))
        {
            target.ApplyHighlight(highlightMaterial);
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