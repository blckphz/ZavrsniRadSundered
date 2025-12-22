using UnityEngine;

[CreateAssetMenu(fileName = "New Grenade Ability", menuName = "Abilities/Grenade")]
public class grenade : rangedOffensive
{
    public GameObject grenadePrefab;
    public GameObject explosionPrefab;
    public float fuseTime = 2f;
    public float explosionRadius = 3f;

    public float armingTime = 0.15f;

    protected override void ExecuteAbility(GameObject user, Vector2 aimDir)
    {
        if (!IsReady()) return;

        if (grenadePrefab == null)
        {
            Debug.LogError("no prefab");
            return;
        }

        GameObject grenadeObj = Instantiate(grenadePrefab, user.transform.position, Quaternion.identity);

        grenadebehav behav = grenadeObj.GetComponent<grenadebehav>();
        if (behav != null)
        {
            behav.grenadeData = new grenadeData
            {
                fuseTime = fuseTime,
                explosionRadius = explosionRadius,
                damage = damage,
                explosionPrefab = explosionPrefab,
                armingTime = armingTime,
                manualDetonationKey = GetDetonationKey(user)
            };
        }

        Rigidbody2D rb = grenadeObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = aimDir.normalized * projectileSpeed;
        }

        TriggerCooldown();
    }

    private KeyCode GetDetonationKey(GameObject user)
    {
        var pa = user.GetComponent<PlayerAbilities>();
        if (pa == null) return KeyCode.Mouse0;

        foreach (var abInput in pa.abilities)
        {
            if (abInput.ability == this)
                return abInput.key;
        }
        return KeyCode.Mouse0;
    }
}
