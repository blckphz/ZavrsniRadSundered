using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Ranged Ability", menuName = "Abilities/Ranged")]
public class rangedAbil : rangedOffensive
{
    public int poolSize = 10;

    private float rotationOffset = 45f;

    protected override void ExecuteAbility(GameObject user, Vector2 aimDir)
    {
        InitializePool(user, projectilePrefab, poolSize);
        if (projectilePool.Count == 0) return;

        TriggerCooldown();

        GetProjectile(
            user.transform.position,
            aimDir.normalized,
            projectileSpeed,
            range,
            damage,
            rotationOffset
        );
    }
}
