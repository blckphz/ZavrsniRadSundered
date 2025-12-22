using UnityEngine;
using System;
using System.Collections;

public class DestructibleBarrel : MonoBehaviour, IDamageable
{
    public int maxHealth = 1;
    private int currentHealth;

    public float fuseTime = 0.5f;
    public float explosionRadius = 5f;
    public int explosionDamage = 50;

    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private AudioClip destroySound;

    public static event Action<DestructibleBarrel, GameObject> OnBarrelDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        wooshScript.OnWooshHit += HandleWooshHit;
    }

    private void OnDisable()
    {
        wooshScript.OnWooshHit -= HandleWooshHit;
    }

    public void TakeDamage(int damage, bool isCrit, Vector3 hitPos, GameObject attacker, bool applyEffects = false)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        ShowDamageNumber(damage, isCrit);

        if (currentHealth <= 0)
        {
            StartCoroutine(DelayedDestroy(attacker));
        }
    }

    private void HandleWooshHit(wooshScript woosh, GameObject target)
    {
        if (target != gameObject) return;

        int dmg = woosh.ownerAbility.damage;
        TakeDamage(dmg, false, woosh.transform.position, woosh.owner.gameObject);
    }

    public void HandleProjectileHit(projectileScript proj, GameObject target)
    {
        if (target != gameObject) return;

        int dmg = proj.ownerAbility.damage;
        TakeDamage(dmg, false, proj.transform.position, proj.owner);
    }

    private IEnumerator DelayedDestroy(GameObject attacker)
    {
        yield return new WaitForSeconds(fuseTime);

        if (destructionEffect) Instantiate(destructionEffect, transform.position, Quaternion.identity);
        if (destroySound) AudioManager.Instance.PlayClip(destroySound);

        DealExplosionDamage();

        OnBarrelDestroyed?.Invoke(this, attacker);
        Destroy(gameObject);
    }

    private void DealExplosionDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue; 

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage, false, transform.position, gameObject);
            }
        }
    }

    private void ShowDamageNumber(int damage, bool isCrit)
    {
        if (DamageNumberPool.Instance)
            DamageNumberPool.Instance.Get().Show(damage, transform.position, 1f, isCrit);
    }

}