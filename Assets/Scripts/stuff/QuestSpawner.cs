using UnityEngine;
using System.Collections.Generic;

public class QuestSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public string id;
        public GameObject prefab;
        public Transform spawnPoint;
    }

    public SpawnableObject[] spawnableObjects;

    private List<string> spawnedObjectIDs = new List<string>();

    void OnEnable()
    {
        DialogueManager.OnSpawnQuestObject += HandleSpawnRequest;
    }

    void OnDisable()
    {
        DialogueManager.OnSpawnQuestObject -= HandleSpawnRequest;
    }

    void HandleSpawnRequest(string id)
    {
        if (spawnedObjectIDs.Contains(id)) return;
        ExecuteSpawn(id);
    }

    void ExecuteSpawn(string id)
    {
        SpawnableObject objData = null;
        foreach (var s in spawnableObjects)
        {
            if (s.id.ToLower() == id.ToLower())
            {
                objData = s;
                break;
            }
        }

        Vector3 spawnPos = objData.spawnPoint.position;
        Quaternion spawnRot = objData.spawnPoint.rotation;

        Instantiate(objData.prefab, spawnPos, spawnRot);
        spawnedObjectIDs.Add(id);
    }

    public List<string> GetSpawnedObjectSaveData()
    {
        return new List<string>(spawnedObjectIDs);
    }

    public void LoadSpawnedObjects(List<string> loadedIDs)
    {
        foreach (string id in loadedIDs)
        {
            if (!spawnedObjectIDs.Contains(id))
            {
                ExecuteSpawn(id);
            }
        }
    }

    public void NotifyObjectRemoved(string id)
    {
        if (spawnedObjectIDs.Contains(id))
        {
            spawnedObjectIDs.Remove(id);
        }
    }
}
