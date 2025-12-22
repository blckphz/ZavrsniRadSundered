using UnityEngine;
using System.Collections;
using Pathfinding;

public class EnemyStatusEffect : MonoBehaviour
{
    public float defaultSlowPercent = 0.5f;
    public float defaultSlowDuration = 2.5f;

    public int damageLow = 4;
    public int damageHigh = 6;
    public bool useRandomBetween = true;
    public float tickInterval = 0.5f;

    private AIPath aiPath;
    private EnemyVisuals visuals;
    private EnemyHealth health;
    private float originalSpeed;

    public bool IsSlowed { get; private set; } = false;
    private Coroutine slowRoutine;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        visuals = GetComponent<EnemyVisuals>();
        health = GetComponent<EnemyHealth>();

        originalSpeed = aiPath.maxSpeed;
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowRoutine(slowPercent, duration));
    }

    private IEnumerator SlowRoutine(float slowPercent, float duration)
    {
        IsSlowed = true;

        aiPath.maxSpeed = originalSpeed * (1f - slowPercent);

        yield return StartCoroutine(visuals.AnimateStunVisuals(true, 0.15f));

        float elapsed = 0f;
        while (elapsed < duration)
        {
            ApplyTickDamage();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        aiPath.maxSpeed = originalSpeed;
        yield return StartCoroutine(visuals.AnimateStunVisuals(false, 0.3f));

        IsSlowed = false;
        slowRoutine = null;
    }

    private void ApplyTickDamage()
    {
        int damage = useRandomBetween ?
            Random.Range(damageLow, damageHigh + 1) :
            (Random.value < 0.5f ? damageLow : damageHigh);

        health.TakeDamage(damage, false, transform.position, null, false);
    }
}