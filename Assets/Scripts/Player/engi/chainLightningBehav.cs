using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class chainLightningBehav : MonoBehaviour
{
    public static event Action<GameObject> OnChainHit;

    private chainLightningAbil ability;
    private GameObject caster;
    private Vector2 direction;
    private float traveledDistance = 0f;

    private int remainingBounces;
    private int currentDamage;

    private List<GameObject> hitTargets = new List<GameObject>();
    private GameObject currentTarget;
    private float rotationOffset = 0f;

    private Material mat;
    private Coroutine intensityRoutine;

    public void Init(chainLightningAbil abil, GameObject user, Vector2 startDir, float rotOffset)
    {
        ability = abil;
        caster = user;
        direction = startDir.normalized;
        remainingBounces = abil.maxBounces;

        currentDamage = abil.damage;
        rotationOffset = rotOffset;


        RotateTowards(direction);
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            Vector3 move = direction * ability.projectileSpeed * Time.deltaTime;
            transform.position += move;
            traveledDistance += move.magnitude;

            RotateTowards(direction);

            if (traveledDistance >= ability.range && hitTargets.Count == 0)
                Destroy(gameObject);
        }
        else
        {
            Vector3 targetPos = currentTarget.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, ability.projectileSpeed * Time.deltaTime);
            RotateTowards((targetPos - transform.position).normalized);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                ArriveAtTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentTarget != null || other.gameObject == caster) return;

        if (other.TryGetComponent(out IDamageable damageable))
            HitTarget(other.gameObject, damageable);
    }

    private void HitTarget(GameObject targetObj, IDamageable damageable)
    {
        if (hitTargets.Contains(targetObj)) return;

        int finalDamage = ability.ApplyCrit(currentDamage, out bool isCrit);

        damageable.TakeDamage(finalDamage, isCrit, transform.position, caster, true);

        ability.PlayHitSound(transform.position);
        hitTargets.Add(targetObj);
        remainingBounces--;

        OnChainHit?.Invoke(targetObj);
        TriggerFlashEffect();

        if (remainingBounces > 0)
            SeekNextTarget(targetObj);
        else
            Destroy(gameObject, 0.05f);
    }

    private void SeekNextTarget(GameObject from)
    {
        GameObject next = FindNextTarget(from);
        if (next != null)
        {
            currentDamage = Mathf.RoundToInt(currentDamage * ability.damageFalloff);
            currentTarget = next;
        }
        else
        {
            Destroy(gameObject, 0.05f);
        }
    }

    private void ArriveAtTarget()
    {
        GameObject hit = currentTarget;
        currentTarget = null;

        if (hit != null && hit.TryGetComponent(out IDamageable damageable))
            HitTarget(hit, damageable);
    }

    private GameObject FindNextTarget(GameObject from)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(from.transform.position, ability.range);
        GameObject nearest = null;
        float shortestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IDamageable damageable)) continue;

            float dist = Vector2.Distance(from.transform.position, hit.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearest = hit.gameObject;
            }
        }
        return nearest;
    }

    private void RotateTowards(Vector2 dir)
    {
        if (dir == Vector2.zero) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    private void TriggerFlashEffect()
    {
        if (mat == null) return;
        if (intensityRoutine != null) StopCoroutine(intensityRoutine);
        intensityRoutine = StartCoroutine(PulseIntensity());
    }

    private IEnumerator PulseIntensity()
    {
        mat.SetFloat("_Intensity", 3f);
        yield return new WaitForSeconds(0.1f);
        mat.SetFloat("_Intensity", 0f);
    }
}
