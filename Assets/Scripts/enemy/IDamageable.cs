using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, bool isCrit, Vector3 hitPos, GameObject attacker, bool applyEffects = false);
}
