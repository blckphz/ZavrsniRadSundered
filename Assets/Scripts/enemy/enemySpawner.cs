using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnInterval = 3f;
    public float spawnRadius = 15f;
    public float minDistance = 10f;

    private float nextSpawnTime;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            player = p.transform;
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos;

        do
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistance, spawnRadius);
            spawnPos = (Vector2)player.position + randomCircle;
        }
        while (Vector2.Distance(spawnPos, player.position) < minDistance);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
