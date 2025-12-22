using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuestProgressWrapper
{
    public List<QuestProgressData> questData;
}

public class GameSaveManager : MonoBehaviour
{
    private string QUEST_SAVE_KEY = "QuestProgress_Data";
    private QuestManager questManager;

    void Awake()
    {
        questManager = FindFirstObjectByType<QuestManager>();
    }

    public void SaveGame()
    {
        if (questManager == null) return;

        List<QuestProgressData> questDataList = questManager.GetQuestProgressData();
        QuestProgressWrapper wrapper = new QuestProgressWrapper { questData = questDataList };
        string jsonToSave = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(QUEST_SAVE_KEY, jsonToSave);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (questManager == null) return;

        if (!PlayerPrefs.HasKey(QUEST_SAVE_KEY))
        {
            return;
        }

        string jsonToLoad = PlayerPrefs.GetString(QUEST_SAVE_KEY);
        QuestProgressWrapper loadedWrapper = JsonUtility.FromJson<QuestProgressWrapper>(jsonToLoad);

        if (loadedWrapper == null || loadedWrapper.questData == null)
        {
            return;
        }

        questManager.ApplyQuestProgressData(loadedWrapper.questData);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(QUEST_SAVE_KEY);
        PlayerPrefs.Save();
    }
}
