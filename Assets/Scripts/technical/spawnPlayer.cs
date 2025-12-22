using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpawnPlayer : MonoBehaviour
{
    public static bool ShouldLoadSave = true;

    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            return;
        }

        Vector3 spawnPosition = Vector3.zero;
        bool playerSpawned = false;

        ShouldLoadSave = SaveScript.IsLoadingGame;

        if (ShouldLoadSave)
        {
            PlayerStateData saved = SaveScript.Instance.LoadPlayerState();
            bool validSave = (saved != null && saved.SceneName == SceneManager.GetActiveScene().name);

            if (validSave)
            {
                spawnPosition = new Vector3(saved.PosX, saved.PosY, 0);
                playerSpawned = true;
            }
        }

        if (!playerSpawned)
        {
            HandleDefaultSpawn(ref spawnPosition);
        }

        ShouldLoadSave = false;

        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    private void HandleDefaultSpawn(ref Vector3 spawnPosition)
    {
        EntryPoints[] allPoints = FindObjectsOfType<EntryPoints>();
        EntryPoints chosen = null;

        string lastID = TransitionManager.LastUsedTransition;

        if (lastID != "" && lastID != null)
        {
            foreach (var p in allPoints)
            {
                if (p.EntryID == lastID)
                {
                    chosen = p;
                    break;
                }
            }
        }

        if (chosen == null && allPoints.Length > 0)
        {
            chosen = allPoints[0];
        }

        spawnPosition = chosen.transform.position;

        TransitionScript door = chosen.GetComponent<TransitionScript>();
        door.DisableTriggerOnSpawn();
    }
}
