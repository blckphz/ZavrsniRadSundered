using UnityEngine;
using System.Collections.Generic;

public abstract class QuestSO : ScriptableObject
{
    public string questID = "DEFAULT_QUEST_ID";

    public int nextDialoguePhaseIndex = 1;

    public string questName = "New Quest Title";

    [TextArea(3, 10)]
    public string description = "A detailed description of what the quest requires.";

    public int experienceReward = 100;
    public int goldReward = 50;

    public bool isComplete = false;

    public virtual void Initialize()
    {
        isComplete = false;
    }

    public virtual void ResetQuest()
    {
        isComplete = false;
    }

    public abstract bool CheckCompletion();

    public virtual void OnQuestCompleted()
    {
        Debug.Log("quest done: " + questName);
    }

    public void MarkComplete()
    {
        isComplete = true;
        OnQuestCompleted();
    }
}
