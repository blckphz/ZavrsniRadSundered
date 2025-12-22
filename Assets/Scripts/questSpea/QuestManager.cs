using UnityEngine;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour
{
    public static event Action<string, string, int> OnQuestCompleted;
    public static event Action OnQuestProgressUpdated;

    public List<QuestSO> availableQuests = new List<QuestSO>();

    public List<QuestSO> activeQuests = new List<QuestSO>();

    private void Awake()
    {
    }

    void OnEnable()
    {
        DialogueManager.OnQuestTagFound += StartNewQuestByName;
        ItemEvents.OnItemCollected += NotifyItemCollected;
        QuestEvents.OnEnemyKilled += NotifyEnemyKilled;
    }

    void OnDisable()
    {
        DialogueManager.OnQuestTagFound -= StartNewQuestByName;
        ItemEvents.OnItemCollected -= NotifyItemCollected;
        QuestEvents.OnEnemyKilled -= NotifyEnemyKilled;
    }

    private void StartNewQuestByName(string questID)
    {
        QuestSO questToStart = null;
        foreach (var q in availableQuests)
        {
            if (q.questID.ToLower() == questID.ToLower())
            {
                questToStart = q;
                break;
            }
        }

        if (questToStart != null)
        {
            ResetQuestRuntimeState(questToStart);
            questToStart.Initialize();
            activeQuests.Add(questToStart);
            OnQuestProgressUpdated?.Invoke();

            if (questToStart.CheckCompletion())
            {
                questToStart.MarkComplete();
                string turnInNPC = GetTurnInNPCForQuest(questToStart);
                int nextIndex = GetNextDialogueIndexForQuest(questToStart);
                OnQuestCompleted?.Invoke(questToStart.questID, turnInNPC, nextIndex);
                OnQuestProgressUpdated?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning("quest not found: " + questID);
        }
    }

    private void ResetQuestRuntimeState(QuestSO quest)
    {
        if (quest is CollectQuestSO collectQuest) collectQuest.currentAmount = 0;
        else if (quest is SpeakWithQuestSO speakQuest) speakQuest.hasSpokenToTarget = false;
        else if (quest is KillQuestSO killQuest) killQuest.currentKills = 0;
    }

    private QuestType GetQuestType(QuestSO quest)
    {
        if (quest is CollectQuestSO) return QuestType.Collect;
        if (quest is SpeakWithQuestSO) return QuestType.SpeakWith;
        if (quest is KillQuestSO) return QuestType.Kill;
        throw new Exception("Unknown quest type");
    }

    public void NotifyItemCollected(string itemID, int amount = 1)
    {
        bool progressMade = false;

        List<CollectQuestSO> collectQuests = new List<CollectQuestSO>();
        foreach (var quest in activeQuests)
        {
            if (quest is CollectQuestSO cq)
                collectQuests.Add(cq);
        }

        foreach (var quest in collectQuests)
        {
            if (quest.requiredItemID.ToLower() == itemID.ToLower())
            {
                int previousAmount = quest.currentAmount;
                quest.ItemCollected(itemID, amount);

                if (previousAmount != quest.currentAmount)
                    progressMade = true;

                if (quest.CheckCompletion())
                {
                    if (!quest.isComplete)
                    {
                        quest.MarkComplete();
                    }
                    OnQuestCompleted?.Invoke(quest.questID, quest.turnInNPC_ID, quest.nextDialoguePhaseIndex);
                    OnQuestProgressUpdated?.Invoke();
                }
            }
        }

        if (progressMade) OnQuestProgressUpdated?.Invoke();
    }

    public void NotifyEnemyKilled(string enemyID)
    {
        bool progressMade = false;

        List<KillQuestSO> killQuests = new List<KillQuestSO>();
        foreach (var quest in activeQuests)
        {
            if (quest is KillQuestSO kq)
                killQuests.Add(kq);
        }

        foreach (var quest in killQuests)
        {
            int previousKills = quest.currentKills;
            quest.EnemyKilled(enemyID, 1);

            if (previousKills != quest.currentKills)
                progressMade = true;

            if (quest.CheckCompletion())
            {
                if (!quest.isComplete)
                {
                    quest.MarkComplete();
                }
                OnQuestCompleted?.Invoke(quest.questID, quest.turnInNPC_ID, quest.nextDialoguePhaseIndex);
                OnQuestProgressUpdated?.Invoke();
            }
        }

        if (progressMade) OnQuestProgressUpdated?.Invoke();
    }

    public void NotifySpokenToNPC(string npcID)
    {
        List<SpeakWithQuestSO> speakQuests = new List<SpeakWithQuestSO>();
        foreach (var quest in activeQuests)
        {
            if (quest is SpeakWithQuestSO sq)
                speakQuests.Add(sq);
        }

        foreach (var quest in speakQuests)
        {
            if (quest.targetNPC_ID.ToLower() == npcID.ToLower())
            {
                if (!quest.CheckCompletion())
                {
                    quest.MarkTargetSpoken();
                    OnQuestProgressUpdated?.Invoke();

                    if (quest.CheckCompletion())
                    {
                        quest.MarkComplete();
                        OnQuestCompleted?.Invoke(quest.questID, quest.targetNPC_ID, quest.nextDialoguePhaseIndex);
                        OnQuestProgressUpdated?.Invoke();
                    }
                }
            }
        }
    }

    public List<string> TurnInCompletedQuestsAtNPC(string npcID)
    {
        List<string> turnedInQuestIDs = new List<string>();

        List<QuestSO> questsCopy = new List<QuestSO>(activeQuests);
        foreach (var quest in questsCopy)
        {
            bool isTurnInTarget = false;

            if (quest is CollectQuestSO cQ && cQ.turnInNPC_ID.ToLower() == npcID.ToLower())
                isTurnInTarget = true;
            else if (quest is KillQuestSO kQ && kQ.turnInNPC_ID.ToLower() == npcID.ToLower())
                isTurnInTarget = true;
            else if (quest is SpeakWithQuestSO sQ && sQ.targetNPC_ID.ToLower() == npcID.ToLower())
                isTurnInTarget = true;

            if (isTurnInTarget && quest.isComplete)
            {
                turnedInQuestIDs.Add(quest.questID);
                activeQuests.Remove(quest);
            }
        }

        if (turnedInQuestIDs.Count > 0)
        {
            OnQuestProgressUpdated?.Invoke();
        }

        return turnedInQuestIDs;
    }

    public List<QuestProgressData> GetQuestProgressData()
    {
        List<QuestProgressData> saveData = new List<QuestProgressData>();

        foreach (var quest in activeQuests)
        {
            QuestProgressData data = new QuestProgressData(quest.questID, GetQuestType(quest));

            if (quest is CollectQuestSO cQ)
            {
                data.currentCollectAmount = cQ.currentAmount;
            }
            else if (quest is SpeakWithQuestSO sQ)
            {
                data.hasSpokenToTarget = sQ.hasSpokenToTarget;
            }
            else if (quest is KillQuestSO kQ)
            {
                data.currentKills = kQ.currentKills;
            }

            data.isComplete = quest.CheckCompletion();
            saveData.Add(data);
        }

        return saveData;
    }

    public void ApplyQuestProgressData(List<QuestProgressData> loadedData)
    {
        activeQuests.Clear();

        foreach (var data in loadedData)
        {
            QuestSO questAsset = null;
            foreach (var q in availableQuests)
            {
                if (q.questID.ToLower() == data.questID.ToLower())
                {
                    questAsset = q;
                    break;
                }
            }

            if (questAsset == null)
            {
                continue;
            }

            activeQuests.Add(questAsset);

            if (data.type == QuestType.Collect && questAsset is CollectQuestSO cQ)
            {
                cQ.currentAmount = data.currentCollectAmount;
            }
            else if (data.type == QuestType.SpeakWith && questAsset is SpeakWithQuestSO sQ)
            {
                sQ.hasSpokenToTarget = data.hasSpokenToTarget;
            }
            else if (data.type == QuestType.Kill && questAsset is KillQuestSO kQ)
            {
                kQ.currentKills = data.currentKills;
            }

            if (questAsset.CheckCompletion())
            {
                questAsset.isComplete = true;
            }
        }

        OnQuestProgressUpdated?.Invoke();
    }

    private string GetTurnInNPCForQuest(QuestSO quest)
    {
        if (quest is CollectQuestSO c) return c.turnInNPC_ID;
        if (quest is KillQuestSO k) return k.turnInNPC_ID;
        if (quest is SpeakWithQuestSO s) return s.targetNPC_ID;
        return "";
    }

    private int GetNextDialogueIndexForQuest(QuestSO quest)
    {
        if (quest is CollectQuestSO c) return c.nextDialoguePhaseIndex;
        if (quest is KillQuestSO k) return k.nextDialoguePhaseIndex;
        if (quest is SpeakWithQuestSO s) return s.nextDialoguePhaseIndex;
        return 0;
    }
}
