using UnityEngine;
using System.Collections.Generic;

public class ExplosionPool : MonoBehaviour
{
    public static ExplosionPool Instance;

    public GameObject explosionPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(explosionPrefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetExplosion()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("pool empty");
            GameObject newObj = Instantiate(explosionPrefab);
            newObj.transform.SetParent(transform);
            newObj.SetActive(false);
            pool.Enqueue(newObj);
        }

        GameObject explosion = pool.Dequeue();

        if (explosion == null)
        {
            Debug.LogWarning("null in pool");

            return GetExplosion();
        }

        explosion.transform.position = Vector3.zero;
        explosion.SetActive(true);
        return explosion;
    }

    public void ReturnExplosion(GameObject explosion)
    {
        if (explosion == null)
        {
            Debug.LogWarning("null return");
            return;
        }

        explosion.SetActive(false);
        explosion.transform.SetParent(transform);
        pool.Enqueue(explosion);
    }
}
