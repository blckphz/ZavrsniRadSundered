using UnityEngine;
using System;

public class projectileScript : MonoBehaviour
{
    public offensiveSO ownerAbility;
    [HideInInspector] public GameObject owner;

    private Vector2 direction;
    private Vector2 startPos;

    [HideInInspector] public float speed;
    public float maxDistance;
    [HideInInspector] public int damage = 10;
    public bool canCrit = false; 

    private float rotationOffset = 0f;

    public GameObject hitEffectPrefab;
    public AudioClip hitSFX;


    public void Launch(Vector2 dir, float spd, float range, int dmg, float angleOffset)
    {
        direction = dir.normalized;
        speed = spd;
        maxDistance = range;
        damage = dmg;
        startPos = transform.position;
        rotationOffset = angleOffset;

        UpdateRotation();
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
        {
            EndProjectile();
        }
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 localScale = transform.localScale;
        float finalZRotation;
        float flipScaleX = 1f;

        if (direction.x < 0)
        {
            flipScaleX = -1f;
            finalZRotation = angle + rotationOffset + 270f;
        }
        else
        {
            flipScaleX = 1f;
            finalZRotation = angle + rotationOffset;
        }

        transform.localScale = new Vector3(flipScaleX * Mathf.Abs(localScale.x), Mathf.Abs(localScale.y), localScale.z);
        transform.rotation = Quaternion.Euler(0f, 0f, finalZRotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage, false, transform.position, owner, false);

            EndProjectile();
            return;
        }

    }

    public void EndProjectile()
    {

        if (ownerAbility is rangedOffensive ranged)
        {
            ranged.ReturnProjectile(gameObject);
            ownerAbility = null;
            owner = null;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}