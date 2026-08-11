using System;
using UnityEngine;

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

    [Header("Interview")]
    public InterviewDecisionData[] interviewDecisions;

    [Header("Case File")]
    public CaseFileQuestionData[] caseFileQuestions;

    [Header("Education")]
    [TextArea(3, 8)] public string howTheScamWorks;
    [TextArea(3, 8)] public string warningSigns;
    [TextArea(3, 8)] public string howToAvoid;
    [TextArea(3, 8)] public string whatToDoIfVictim;
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
public class CaseFileQuestionData
{
    public string questionId;
    [TextArea(2, 5)] public string prompt;
    public string[] options;
    [Min(0)] public int correctOptionIndex;
    public string supportingFactId;
    [TextArea(2, 5)] public string explanation;
}
