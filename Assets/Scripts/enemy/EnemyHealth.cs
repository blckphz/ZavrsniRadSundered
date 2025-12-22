using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public enum ReputationCircle
    {
        Empty,
        Iron,
        Jade,
        Lapis,
        Obsidian
    }

    public string enemyID;
    public int maxHealth = 100;
    private int currentHealth;
    public ReputationCircle circle;

    public Image healthBarFill;
    public Color fullHealthColor = Color.green;
    public Color emptyHealthColor = Color.red;
    public float damageNumberOffset = 1f;

    public GameObject hitParticlePrefab;
    public GameObject lootPrefab;
    [Range(0f, 100f)] public float lootDropChance = 25f;
    public Vector3 spawnOffset = new Vector3(0, 0.25f, 0);

    [SerializeField] private EnemyVisuals visuals;
    public EnemyAudio audioClip;
    [SerializeField] private EnemyStatusEffect status;
    private shakeHealthbar healthbarShaker;
    private slowmo slowmoEffect;

    public static event Action<EnemyHealth, GameObject> OnEnemyDied;

    private void Awake()
    {
        currentHealth = maxHealth;

        TryGetComponent(out visuals);
        TryGetComponent(out audioClip);
        TryGetComponent(out status);
        TryGetComponent(out healthbarShaker);

        slowmoEffect = FindFirstObjectByType<slowmo>();
        UpdateHealthBar();
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

        visuals.TriggerHit();
        audioClip.PlayDamageSound();
        healthbarShaker.TriggerShake();
        UpdateHealthBar();
        ShowHitEffects(damage, isCrit, hitPos);

        if (applyEffects)
            status.ApplySlow(status.defaultSlowPercent, status.defaultSlowDuration);

        GetComponent<enemyWalk>().OnDamaged();

        if (currentHealth <= 0) Die(attacker);
    }


    private void HandleWooshHit(wooshScript woosh, GameObject target)
    {
        if (target != gameObject) return;

        if (woosh.ownerAbility is meleeAbil melee)
        {
            int damage = melee.ApplyCrit(melee.damage, out bool isCrit);

            if (status.IsSlowed)
                damage = Mathf.RoundToInt(damage * melee.stunDamageMultiplier);

            TakeDamage(damage, isCrit, woosh.transform.position, melee.OwnerPlayer);
            melee.PlayHitSound(transform.position);
        }
    }

    public void HandleProjectileHit(projectileScript proj, GameObject target)
    {
        if (target != gameObject || proj.ownerAbility == null) return;

        int damage = proj.ownerAbility.ApplyCrit(proj.ownerAbility.damage, out bool isCrit);

        bool isLightning = proj.ownerAbility is chainLightningAbil;

        TakeDamage(damage, isCrit, proj.transform.position, proj.ownerAbility.OwnerPlayer, isLightning);
        proj.ownerAbility.PlayHitSound(transform.position);

        if (proj.hitEffectPrefab) Instantiate(proj.hitEffectPrefab, proj.transform.position, Quaternion.identity);
    }


    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;
        float percent = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = percent;
        healthBarFill.color = Color.Lerp(emptyHealthColor, fullHealthColor, percent);
    }

    private void ShowHitEffects(int damage, bool isCrit, Vector3 hitPos)
    {
        if (DamageNumberPool.Instance)
            DamageNumberPool.Instance.Get().Show(damage, transform.position, damageNumberOffset, isCrit);

        if (hitParticlePrefab)
        {
            Vector3 dir = (transform.position - hitPos).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Instantiate(hitParticlePrefab, transform.position + spawnOffset, Quaternion.Euler(0, 0, angle));
        }
    }

    private void Die(GameObject attacker)
    {
        QuestEvents.EnemyKilled(enemyID);
        audioClip.PlayDeathSound();
        slowmoEffect.TriggerSlowMotion();

        OnEnemyDied?.Invoke(this, attacker);
        GetComponent<enemyWalk>().OnDeath();

        if (lootPrefab && UnityEngine.Random.Range(0f, 100f) <= lootDropChance)
            Instantiate(lootPrefab, transform.position + spawnOffset, Quaternion.identity);

        gameObject.SetActive(false);
    }
}
