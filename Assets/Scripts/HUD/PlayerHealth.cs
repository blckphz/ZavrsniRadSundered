using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject particleSystemPrefab;

    public static event Action OnPlayerDied;
    public static event Action<int, int> OnHealthChanged;

    public float HealthPercentage
    {
        get
        {
            if (maxHealth <= 0) return 0f;
            return (float)currentHealth / maxHealth;
        }
    }

    public bool IsDead
    {
        get { return currentHealth <= 0; }
    }

    void Start()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, bool isCrit, Vector3 hitPos, GameObject attacker, bool applyEffects = false)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (DamageNumberPool.Instance != null)
        {
            DamageNumberPool.Instance.Get().Show(damage, hitPos, 1f, isCrit);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, false, transform.position, null, false);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (particleSystemPrefab != null)
        {
            GameObject psInstance = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            Destroy(psInstance, 3f);
        }

        OnPlayerDied?.Invoke();
        Destroy(gameObject);
    }
}

