using System;
using UnityEngine;



    [System.Serializable]
    public class CaseFileParagraphPart
    {
        [TextArea(2, 5)]
        [Tooltip("Text shown before this dropdown.")]
        public string textBefore;

        [Tooltip("Dropdown that appears after this text.")]
        public CaseFileDropdownData dropdown;
    }

[CreateAssetMenu(fileName = "NewScamCase", menuName = "Scam Response/Scam Case")]
public class ScamCaseData : ScriptableObject
{
    [Header("Identity")]
    public string caseId;
    public string queueNumber;
    public string caseTitle;
    public string scamType;

    [Header("Victim")]
    public string victimName;
    public int victimAge;
    public string victimOccupation;
    [TextArea(2, 5)] public string initialComplaint;
    public GameObject lobbyNpcPrefab;
    public GameObject interviewNpcPrefab;
    public NpcBehaviourType behaviourType;

    [Header("Reception Dialogue")]

    [TextArea(2, 5)]
    public string receptionDialogue;

    [Header("Interview")]
    public InterviewDecisionData[] interviewDecisions;

    [Header("Education")]
    [TextArea(3, 8)] public string howTheScamWorks;
    [TextArea(3, 8)] public string warningSigns;
    [TextArea(3, 8)] public string howToAvoid;
    [TextArea(3, 8)] public string whatToDoIfVictim;

    [Header("Case File")]

    [Tooltip("Text sections that appear before, between, and after dropdowns.")]
    [TextArea(2, 5)]
    public string[] caseFileTextSegments;

    [Tooltip("Dropdowns that appear between the text sections.")]
    public CaseFileDropdownData[] caseFileDropdowns;
}

public enum NpcBehaviourType { NormalResponder, DoesNotRespond, AnxiousRush }

[Serializable]
public class InterviewDecisionData
{
    [TextArea(2, 5)] public string npcLine;
    public DialogueChoiceData[] choices;
}

[Serializable]
public class DialogueChoiceData
{
    [TextArea(1, 3)] public string playerChoice;
    [TextArea(2, 6)] public string npcResponse;
    public string[] unlockedFactIds;
}

[Serializable]
public class CaseFileDropdownData
{
    [Tooltip("Unique name for this dropdown.")]
    public string fieldId;

    [Tooltip("Answers shown in the dropdown.")]
    public string[] options;

    [Min(0)]
    [Tooltip("Correct answer index. First option = 0.")]
    public int correctOptionIndex;

    [Tooltip("Optional interview fact connected to this field.")]

    public string supportingFactId;
}