using UnityEngine;
using System.Collections;

public class enemyFindTarget : MonoBehaviour
{
    public float targetCheckInterval = 1.0f;

    [HideInInspector]
    public Transform currentTarget;

    void Start()
    {
        StartCoroutine(TargetingRoutine());
    }

    private IEnumerator TargetingRoutine()
    {
        while (true)
        {
            FindBestTarget();
            yield return new WaitForSeconds(targetCheckInterval);
        }
    }

    private void FindBestTarget()
    {
        Transform newTarget = FindClosestTargetWithHealth();

        if (newTarget != currentTarget)
        {
            currentTarget = newTarget;

            if (currentTarget == null)
            {
                Debug.Log("No valid target with 'health' script found.");
            }
        }
    }

    private Transform FindClosestTargetWithHealth()
    {
        PlayerHealth[] allHealthObjects = FindObjectsOfType<PlayerHealth>();
        if (allHealthObjects.Length == 0)
            return null;

        Transform closest = null;
        float minDistanceSq = float.MaxValue;
        Vector3 currentPos = transform.position;

        foreach (PlayerHealth healthComponent in allHealthObjects)
        {
            if (healthComponent == null || !healthComponent.gameObject.activeInHierarchy || healthComponent.gameObject == gameObject) continue;

            Transform potentialTarget = healthComponent.transform;
            float distSq = (potentialTarget.position - currentPos).sqrMagnitude;

            if (distSq < minDistanceSq)
            {
                minDistanceSq = distSq;
                closest = potentialTarget;
            }
        }

        return closest;
    }
}
