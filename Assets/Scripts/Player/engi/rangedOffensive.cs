using UnityEngine;
using System.Collections.Generic;

public abstract class rangedOffensive : offensiveSO
{
    public GameObject projectilePrefab;

    public float range = 5f;
    public float projectileSpeed = 10f;
    public float fireRate = 1f;

    protected Queue<GameObject> projectilePool = new Queue<GameObject>();

    public virtual void InitializePool(GameObject user, GameObject prefab, int poolSize)
    {
        if (projectilePool.Count > 0) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);

            var proj = obj.GetComponent<projectileScript>();
            proj.ownerAbility = this;

            projectilePool.Enqueue(obj);
        }
    }

    public GameObject GetProjectile(Vector2 startPos, Vector2 direction, float speed, float range, int damage, float angleOffset)
    {
        GameObject projectileGO;
        projectileScript projScript;

        if (projectilePool.Count > 0)
        {
            projectileGO = projectilePool.Dequeue();
            projectileGO.SetActive(true);
            projectileGO.transform.position = startPos;
            projectileGO.transform.rotation = Quaternion.identity;
        }
        else
        {
            projectileGO = Instantiate(projectilePrefab, startPos, Quaternion.identity);
            projScript = projectileGO.GetComponent<projectileScript>();
            projScript.ownerAbility = this;
        }

        projScript = projectileGO.GetComponent<projectileScript>();
        projScript.Launch(direction, speed, range, damage, angleOffset);

        return projectileGO;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        var projScript = projectile.GetComponent<projectileScript>();
        projScript.ownerAbility = null;
        projScript.owner = null;

        projectile.transform.SetParent(null);
        projectile.SetActive(false);

        projectilePool.Enqueue(projectile);
    }
}
