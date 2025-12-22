using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    public static List<Transform> AggroedEnemies = new List<Transform>();

    private void Awake()
    {
        AggroedEnemies.Clear();
    }

    public static void RegisterAggro(Transform enemyTransform)
    {
        if (enemyTransform == null) return;

        if (!AggroedEnemies.Contains(enemyTransform))
        {
            AggroedEnemies.Add(enemyTransform);
        }
    }

    public static void DeregisterAggro(Transform enemyTransform)
    {
        if (AggroedEnemies.Contains(enemyTransform))
        {
            AggroedEnemies.Remove(enemyTransform);
        }
    }
}