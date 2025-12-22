using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class circleanimator : MonoBehaviour
{
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public float scaleSpeed = 10f;

    public AudioClip pulseSound;
    public GameObject particlePrefab;
    public float particleOffsetZ = -1f;

    public float maxPitchMin = 1.0f;
    public float maxPitchMax = 1.2f;
    public float minPitchMin = 0.8f;
    public float minPitchMax = 1.0f;

    private Vector3 originalScale;
    private bool isMoving = false;
    private AudioSource audioSource;

    private bool hasTriggeredMax = false;
    private bool hasTriggeredMin = false;

    void Start()
    {
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isMoving)
        {
            float sineValue = Mathf.Sin(Time.time * scaleSpeed);
            float normalScale = (sineValue + 1f) * 0.5f;
            float currentScale = Mathf.Lerp(minScale, maxScale, normalScale);

            transform.localScale = originalScale * currentScale;

            if (sineValue > 0.98f && !hasTriggeredMax)
            {
                TriggerScaleEffects(isMax: true);
                hasTriggeredMax = true;
                hasTriggeredMin = false;
            }
            else if (sineValue < -0.98f && !hasTriggeredMin)
            {
                TriggerScaleEffects(isMax: false);
                hasTriggeredMin = true;
                hasTriggeredMax = false;
            }
        }
        else
        {
            transform.localScale = Vector2.Lerp(transform.localScale, originalScale, Time.deltaTime * scaleSpeed * 0.5f);

            hasTriggeredMax = false;
            hasTriggeredMin = false;
        }
    }

    private void TriggerScaleEffects(bool isMax)
    {
        if (pulseSound != null && audioSource != null)
        {
            float pitchMin = isMax ? maxPitchMin : minPitchMin;
            float pitchMax = isMax ? maxPitchMax : minPitchMax;
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.PlayOneShot(pulseSound);
        }
        LaunchParticles();
    }

    private void LaunchParticles()
    {
        if (particlePrefab != null)
        {
            Vector2 spawnPos = transform.position;

            GameObject particles = Instantiate(particlePrefab, spawnPos, Quaternion.identity);

            ParticleSystem ps = particles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(particles, 1f);
            }
        }
    }

    public void SetPaused(bool paused)
    {
        isMoving = !paused;
        if (audioSource != null)
        {
            if (paused) audioSource.Pause();
            else audioSource.UnPause();
        }
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }
}
