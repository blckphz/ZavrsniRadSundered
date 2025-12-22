using UnityEngine;
using System;

public class wooshScript : MonoBehaviour
{
    [HideInInspector] public meleeAbil ownerAbility;
    [HideInInspector] public GameObject owner;

    public float range = 1f;
    public Vector2 offset = Vector2.zero;
    public Vector2 direction = Vector2.right;

    public static event Action<wooshScript, GameObject> OnWooshHit;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        UpdateRotation();
    }

    private void Update()
    {
        if (owner != null)
        {
            Vector2 targetPos = (Vector2)owner.transform.position + direction * range + offset;
            transform.position = targetPos;
        }
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    public void endWoosh()
    {
        if (ownerAbility != null)
        {
            ownerAbility.ReturnWoosh(gameObject);
            ownerAbility = null;
            owner = null;
        }
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();


        OnWooshHit?.Invoke(this, other.gameObject);
    }
}
