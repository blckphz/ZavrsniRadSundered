using UnityEngine;
using System.Collections.Generic;

public class SpawnedObjectData : MonoBehaviour
{
    public static SpawnedObjectData Instance;
    public List<GameObject> activeSpawnedObjects = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterSpawn(GameObject spawnedObject)
    {
        if (!activeSpawnedObjects.Contains(spawnedObject))
        {
            activeSpawnedObjects.Add(spawnedObject);
        }
    }

    public void UnregisterSpawn(GameObject despawnedObject)
    {
        activeSpawnedObjects.Remove(despawnedObject);
    }
}