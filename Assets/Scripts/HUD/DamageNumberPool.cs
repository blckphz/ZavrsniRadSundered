using UnityEngine;
using System.Collections.Generic;

public class DamageNumberPool : MonoBehaviour
{
    public static DamageNumberPool Instance { get; private set; }

    public DamageNumber damagePrefab;
    public int initialSize = 10;

    private Queue<DamageNumber> pool = new Queue<DamageNumber>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < initialSize; i++)
            AddToPool();
    }

    private void AddToPool()
    {
        DamageNumber obj = Instantiate(damagePrefab, transform);
        obj.gameObject.SetActive(false);
        obj.Initialize(ReturnToPool);
        pool.Enqueue(obj);
    }

    public DamageNumber Get()
    {
        if (pool.Count == 0) AddToPool();
        return pool.Dequeue();
    }

    private void ReturnToPool(DamageNumber obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}
