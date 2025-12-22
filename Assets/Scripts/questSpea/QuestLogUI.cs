using UnityEngine;
using System.Collections.Generic;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;

    [SerializeField] private GameObject questItemPrefab;

    [SerializeField] private GameObject toggleObject;

    private Dictionary<string, QuestLogItemUI> activeUIItems = new Dictionary<string, QuestLogItemUI>();
    private QuestManager manager;

    private void OnEnable()
    {
        QuestManager.OnQuestProgressUpdated += HandleQuestUpdate;
        manager = FindFirstObjectByType<QuestManager>();
        if (manager != null)
        {
            HandleQuestUpdate();
        }
    }

    private void OnDisable()
    {
        QuestManager.OnQuestProgressUpdated -= HandleQuestUpdate;
    }

    private void Update()
    {
        if (toggleObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            toggleObject.SetActive(!toggleObject.activeSelf);
        }
    }

    private void HandleQuestUpdate()
    {
        if (manager != null)
        {
            RefreshQuestLog(manager.activeQuests);
        }
    }

    private void RefreshQuestLog(List<QuestSO> activeQuests)
    {
        List<string> activeQuestIDs = new List<string>();
        foreach (var q in activeQuests)
        {
            activeQuestIDs.Add(q.questID);
        }

        List<string> keysToRemove = new List<string>();

        foreach (var pair in activeUIItems)
        {
            if (!activeQuestIDs.Contains(pair.Key))
            {
                keysToRemove.Add(pair.Key);
                Destroy(pair.Value.gameObject);
            }
        }
        foreach (string key in keysToRemove)
        {
            activeUIItems.Remove(key);
        }

        foreach (var quest in activeQuests)
        {
            if (activeUIItems.ContainsKey(quest.questID))
            {
                activeUIItems[quest.questID].Setup(quest);
            }
            else
            {
                GameObject newItemGO = Instantiate(questItemPrefab, contentParent);
                QuestLogItemUI itemUI = newItemGO.GetComponent<QuestLogItemUI>();
                itemUI.Setup(quest);
                activeUIItems.Add(quest.questID, itemUI);
            }
        }
    }

    public bool IsOpen()
    {
        return toggleObject != null && toggleObject.activeSelf;
    }
}
