using UnityEngine;
using System.Collections.Generic;

public class WooshPool : MonoBehaviour
{
    public GameObject wooshPrefab;
    public int poolSize = 5;

    private Queue<GameObject> pool = new Queue<GameObject>();

    public void InitializePool(meleeAbil ownerAbility)
    {

        if (pool.Count > 0)
        {
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(wooshPrefab, this.transform);
            obj.SetActive(false);

            var ws = obj.GetComponent<wooshScript>();
            if (ws != null)
            {
                ws.ownerAbility = ownerAbility;
            }

            pool.Enqueue(obj);
        }
    }

    public GameObject GetWoosh()
    {

        GameObject woosh = pool.Dequeue();
        woosh.transform.SetParent(null);

        return woosh;
    }

    public void ReturnWoosh(GameObject woosh)
    {
        if (woosh == null) return;

        var ws = woosh.GetComponent<wooshScript>();
        if (ws != null)
        {
            ws.ownerAbility = null;
            ws.owner = null;
        }

        woosh.SetActive(false);
        woosh.transform.SetParent(this.transform);
        pool.Enqueue(woosh);
    }
}
