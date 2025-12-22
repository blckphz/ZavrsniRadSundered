using System;
using UnityEngine;

[Serializable]
public class QuestProgressData
{
    public string questID;
    public QuestType type;
    public bool isComplete;

    public int currentCollectAmount;

    public bool hasSpokenToTarget;

    public int currentKills;

    public QuestProgressData(string id, QuestType t)
    {
        questID = id;
        type = t;
        isComplete = false;
        currentCollectAmount = 0;
        hasSpokenToTarget = false;
        currentKills = 0;
    }
}

public enum QuestType
{
    SpeakWith,
    Collect,
    Kill
}
