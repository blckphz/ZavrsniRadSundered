using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveScript : MonoBehaviour
{
    public static bool IsLoadingGame = false;

    string playerSaveBase = "player_state_";
    string questSaveBase = "quest_progress_";
    string storySaveBase = "story_state_";
    string worldSaveBase = "world_state_";
    string defaultStartScene = "IntroScene";

    public string CurrentSaveName = "Slot1";

    public GameObject TargetGameObject;
    public GameObject TargetGameObjectBG;
    public GameObject GameUI;

    public static SaveScript Instance;

    private QuestManager questManager;
    private QuestSpawner questSpawner;
    private PlayerStateData pendingPlayerState;

    public static System.Action OnSaveFileDetected;

    private List<string> destroyedObjects = new List<string>();

    [System.Serializable]
    private class WorldStateData
    {
        public List<string> destroyedObjectIDs;
        public List<string> spawnedQuestObjectIDs; 
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        questManager = FindAnyObjectByType<QuestManager>();
        questSpawner = FindAnyObjectByType<QuestSpawner>(); 

        DialogueManager.OnDialogueEnded += SaveGame;

        bool exists = CheckIfSaveExists(CurrentSaveName);
        ToggleGameObjectBasedOnSaveStatus();

        if (exists) OnSaveFileDetected?.Invoke();
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueEnded -= SaveGame;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SaveGame()
    {
        SaveQuestData();
        SaveStoryState();
        SavePlayerState();
        SaveWorldState();
        PlayerPrefs.Save();
        ToggleGameObjectBasedOnSaveStatus();
    }

    public void LoadGame()
    {
        IsLoadingGame = true;


        PlayerStateData playerData = LoadPlayerState();
        if (playerData != null)
        {
            pendingPlayerState = playerData;

            LoadWorldState();

            LoadQuestData();
            LoadStoryState();

            SceneManager.LoadScene(playerData.SceneName);
        }
        else
        {
            IsLoadingGame = false;
            SceneManager.LoadScene(defaultStartScene);
        }
    }

    public void DeleteSave(string saveName)
    {
        string[] bases = { playerSaveBase, questSaveBase, storySaveBase, worldSaveBase };

        foreach (string b in bases)
        {
            string path = GetFilePath(b, saveName);
            if (File.Exists(path)) File.Delete(path);
        }

        destroyedObjects.Clear();
        ToggleGameObjectBasedOnSaveStatus();

        Debug.Log("deleted save " + saveName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsLoadingGame)
        {
            pendingPlayerState = null;
            return;
        }

        Debug.Log("loaded " + scene.name);

        questSpawner = FindFirstObjectByType<QuestSpawner>();

        if (questSpawner != null)
        {
            string path = GetFilePath(worldSaveBase);
            if (File.Exists(path))
            {
                WorldStateData data = JsonUtility.FromJson<WorldStateData>(File.ReadAllText(path));
                questSpawner.LoadSpawnedObjects(data.spawnedQuestObjectIDs);
            }
        }

        if (pendingPlayerState != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(
                    pendingPlayerState.PosX,
                    pendingPlayerState.PosY,
                    player.transform.position.z
                );
            }
        }

        StartCoroutine(ResetLoadingStateAfterFrame());
    }

    private IEnumerator ResetLoadingStateAfterFrame()
    {
        yield return null;
        IsLoadingGame = false;
        GameUI.SetActive(true);
        Debug.Log("loading done");
    }

    private void SaveWorldState()
    {
        List<string> spawnedIDs = new List<string>();
        if (questSpawner != null)
        {
            spawnedIDs = questSpawner.GetSpawnedObjectSaveData();
        }

        WorldStateData data = new WorldStateData
        {
            destroyedObjectIDs = destroyedObjects,
            spawnedQuestObjectIDs = spawnedIDs
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetFilePath(worldSaveBase), json);

        Debug.Log("world saved");
    }

    private void LoadWorldState()
    {
        string path = GetFilePath(worldSaveBase);
        if (!File.Exists(path)) return;

        WorldStateData data = JsonUtility.FromJson<WorldStateData>(File.ReadAllText(path));
        destroyedObjects = new List<string>(data.destroyedObjectIDs);

        Debug.Log("world loaded");
    }

    public void RegisterDestroyedObject(string id)
    {
        if (!destroyedObjects.Contains(id))
            destroyedObjects.Add(id);
    }

    public bool IsObjectDestroyed(string id)
    {
        return destroyedObjects.Contains(id);
    }

    private void SavePlayerState()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerStateData data = new PlayerStateData
        {
            SceneName = SceneManager.GetActiveScene().name,
            PosX = player.transform.position.x,
            PosY = player.transform.position.y
        };

        File.WriteAllText(GetFilePath(playerSaveBase), JsonUtility.ToJson(data, true));
    }

    public PlayerStateData LoadPlayerState()
    {
        string path = GetFilePath(playerSaveBase);
        if (!File.Exists(path)) return null;
        return JsonUtility.FromJson<PlayerStateData>(File.ReadAllText(path));
    }

    private void SaveQuestData()
    {
        List<QuestProgressData> list = new List<QuestProgressData>();
        foreach (var q in questManager.GetQuestProgressData())
        {
            if (!q.isComplete)
                list.Add(q);
        }
        string path = GetFilePath(questSaveBase);

        if (list.Count == 0)
        {
            if (File.Exists(path)) File.Delete(path);
            return;
        }

        File.WriteAllText(
            path,
            JsonUtility.ToJson(new QuestListWrapper { Quests = list }, true)
        );
    }

    private void LoadQuestData()
    {
        string path = GetFilePath(questSaveBase);
        if (!File.Exists(path))
        {
            questManager.ApplyQuestProgressData(new List<QuestProgressData>());
            return;
        }

        var wrapper = JsonUtility.FromJson<QuestListWrapper>(File.ReadAllText(path));
        questManager.ApplyQuestProgressData(wrapper.Quests);
    }

    private void SaveStoryState()
    {
        string json = DialogueManager.GetStoryState();
        if (json != "")
            File.WriteAllText(GetFilePath(storySaveBase), json);
    }

    private void LoadStoryState()
    {
        string path = GetFilePath(storySaveBase);
        if (File.Exists(path))
            DialogueManager.SetStoryState(File.ReadAllText(path));
    }

    public bool CheckIfSaveExists(string saveName)
    {
        return File.Exists(GetFilePath(playerSaveBase, saveName));
    }

    public void ToggleGameObjectBasedOnSaveStatus()
    {
        bool exists = CheckIfSaveExists(CurrentSaveName);
        TargetGameObject.SetActive(exists);
        TargetGameObjectBG.SetActive(exists);
    }

    private string GetFilePath(string baseName)
    {
        return GetFilePath(baseName, CurrentSaveName);
    }

    private string GetFilePath(string baseName, string saveName)
    {
        return Path.Combine(Application.persistentDataPath, baseName + saveName + ".json");
    }

    [System.Serializable]
    public class QuestListWrapper
    {
        public List<QuestProgressData> Quests;
    }
}