using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCaseData", menuName = "Scam Response/Case Data")]
public class CaseData : ScriptableObject
{
    [Header("Identity")]
    public string caseId;
    public string caseTitle;
    public string scamType;

    [Header("Victim")]
    public string victimName;
    public int victimAge;
    public string victimOccupation;
    [TextArea(2, 5)] public string initialComplaint;
    public GameObject npcPrefab;
    public NPCBehaviourType npcBehaviour;

    [Header("Interview")]
    public InterviewQuestion[] interviewQuestions;

    [Header("Investigation")]
    public EvidenceEntry[] evidence;
    public EvidenceSlotDefinition[] evidenceSlots;

    [Header("Education")]
    [TextArea(3, 8)] public string howTheScamWorks;
    [TextArea(3, 8)] public string warningSigns;
    [TextArea(3, 8)] public string preventionAdvice;
    [TextArea(3, 8)] public string whatToDoIfVictim;
}

public enum NPCBehaviourType
{
    NormalWaiter,
    Unresponsive,
    AnxiousRush
}

[Serializable]
public class InterviewQuestion
{
    [TextArea(2, 5)] public string npcQuestion;
    public InterviewChoice[] choices;
}

[Serializable]
public class InterviewChoice
{
    public string playerChoice;
    [TextArea(2, 5)] public string npcResponse;
    [Tooltip("Evidence IDs unlocked by selecting this answer.")]
    public string[] unlockEvidenceIds;
}

[Serializable]
public class EvidenceEntry
{
    public string evidenceId;
    public string title;
    [TextArea(2, 5)] public string description;
    public Sprite image;
    public string correctSlotId;
    public bool unlockedByDefault;
}

[Serializable]
public class EvidenceSlotDefinition
{
    public string slotId;
    public string displayName;
}
