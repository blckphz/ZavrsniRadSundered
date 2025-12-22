using UnityEngine;

public class RockHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 30;
    private int currentHealth;

    public DestroyableObject destobject;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, bool isCrit, Vector3 hitPos, GameObject attacker, bool applyEffects = false)
    {

        if (!IsValidAttacker(attacker))
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
            BreakRock();
    }

    private bool IsValidAttacker(GameObject attacker)
    {
        if (attacker == null) return false;

        bool isBarrel = attacker.GetComponent<DestructibleBarrel>() != null ||
                        attacker.name.ToLower().Contains("barrel");

        return isBarrel;
    }

    public void BreakRock()
    {
        if (destobject != null)
        {
            destobject.DestroyObject();
        }

        this.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.ToLower().Contains("barrel"))
        {
            BreakRock();
        }
    }
}