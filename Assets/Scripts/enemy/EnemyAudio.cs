using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    public AudioClip damageSound;
    public float damageVolume = 1f;
    public float damagePitchVariation = 0.1f;

    public AudioClip deathSound;
    public float deathVolume = 1f;
    public float deathPitchVariation = 0.1f;

    public AudioClip reactionSound;
    public float reactionVolume = 1f;
    public float reactionPitchVariation = 0.1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    public void PlayDamageSound()
    {
        PlayClip(damageSound, damageVolume, damagePitchVariation);
    }

    public void PlayDeathSound()
    {
        PlayClip(deathSound, deathVolume, deathPitchVariation);
    }

    public void PlayReactionSound()
    {
        PlayClip(reactionSound, reactionVolume, reactionPitchVariation);
    }

    private void PlayClip(AudioClip clip, float volume, float pitchVar)
    {
        if (clip == null) return;

        audioSource.pitch = 1f + Random.Range(-pitchVar, pitchVar);
        audioSource.PlayOneShot(clip, volume);
    }
}
