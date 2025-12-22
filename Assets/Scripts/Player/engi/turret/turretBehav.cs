using UnityEngine;
using System.Collections.Generic;

public class turretBehav : MonoBehaviour
{
    [HideInInspector] public offensiveSO abilityData;

    public float range = 10f;
    public float fireRate = 1f;
    public int damage;
    public float bulletSpeed = 20f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float additionalangle = -90f;

    public AudioClip shootSound;
    private AudioSource audioSource;

    private float fireTimer;

    public void Setup(float lifetime)
    {
        if (lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (abilityData != null)
            damage = abilityData.damage;

        fireTimer = 0f;
    }

    private void Update()
    {
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;

        Transform target = FindNearestEnemy();

        if (target != null && fireTimer <= 0f)
        {
            Shoot(target.gameObject);
            fireTimer = 1f / fireRate;
        }
    }

    private Transform FindNearestEnemy()
    {
        List<Transform> enemies = PlayerCombat.AggroedEnemies;

        Transform nearest = null;
        float shortestDistSq = range * range;
        Vector2 turretPosition = transform.position;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Transform enemyTransform = enemies[i];

            if (enemyTransform == null)
            {
                enemies.RemoveAt(i);
                continue;
            }

            float distSq = ((Vector2)enemyTransform.position - turretPosition).sqrMagnitude;

            if (distSq < shortestDistSq)
            {
                shortestDistSq = distSq;
                nearest = enemyTransform;
            }
        }

        return nearest;
    }

    private void Shoot(GameObject target)
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 dir = (target.transform.position - firePoint.position).normalized;
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        projectileScript projectile = bulletGO.GetComponent<projectileScript>();
        if (projectile != null)
        {
            projectile.ownerAbility = abilityData;
            projectile.owner = gameObject;
            projectile.Launch(dir, bulletSpeed, range, damage, additionalangle);
        }

        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
