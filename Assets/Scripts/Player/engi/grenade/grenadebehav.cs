using UnityEngine;
using System.Collections;

public class grenadebehav : MonoBehaviour
{
    public grenadeData grenadeData;
    private bool exploded = false;
    private float armingTimer = 0f;

    private void Start()
    {
        if (grenadeData != null)
        {
            StartCoroutine(FuseCoroutine());
        }
    }

    private void Update()
    {
        if (armingTimer < grenadeData.armingTime)
            armingTimer += Time.deltaTime;

        bool isArmed = armingTimer >= grenadeData.armingTime;

        if (Input.GetKeyDown(grenadeData.manualDetonationKey) && isArmed)
        {
            Explode();
        }
    }

    private IEnumerator FuseCoroutine()
    {
        yield return new WaitForSeconds(grenadeData.fuseTime);
        if (!exploded) Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        if (!exploded) Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (ExplosionPool.Instance != null)
        {
            GameObject explosion = ExplosionPool.Instance.GetExplosion();
            explosion.transform.position = transform.position;
        }

        Vector2 explosionCenter = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionCenter, grenadeData.explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                float distance = Vector2.Distance(explosionCenter, hit.transform.position);
                float distanceFactor = 1f - Mathf.Clamp01(distance / grenadeData.explosionRadius);
                int finalDamage = Mathf.RoundToInt(grenadeData.damage * distanceFactor);

                if (finalDamage > 0)
                {
                    damageable.TakeDamage(finalDamage, false, explosionCenter, gameObject);
                }
            }
        }

        Destroy(gameObject);
    }

}
