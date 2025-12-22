using System;
using UnityEngine;

public static class QuestEvents
{
    public static event Action<string> OnEnemyKilled;

    public static void EnemyKilled(string enemyID)
    {
        Debug.Log("killed " + enemyID);
        OnEnemyKilled?.Invoke(enemyID);
    }
}
