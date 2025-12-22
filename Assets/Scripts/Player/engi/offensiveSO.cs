using UnityEngine;

public abstract class offensiveSO : AbilitySO
{
    [HideInInspector] public GameObject OwnerPlayer;

    public int damage;

    [Range(0f, 1f)] public float critChance = 0.2f;
    public float critMultiplier = 2f;

    public AudioClip hitSound;
    [Range(0f, 1f)] public float hitVolume = 0.6f;
    [Range(0f, 0.3f)] public float hitPitchVariation = 0.1f;

    public void PlayHitSound(Vector3 position)
    {
        if (hitSound == null || AudioManager.Instance == null) return;

        AudioManager.Instance.PlayClip(hitSound);
    }

    public int ApplyCrit(int baseDamage, out bool isCrit)
    {
        isCrit = Random.value < critChance;
        return isCrit ? Mathf.RoundToInt(baseDamage * critMultiplier) : baseDamage;
    }
}
