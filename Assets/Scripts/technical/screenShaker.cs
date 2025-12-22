using UnityEngine;
using Unity.Cinemachine;

public class screenShaker : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin perlin;
    public float defaultIntensity = 1f;
    public float defaultDuration = 0.2f;

    private void OnEnable()
    {
        wooshScript.OnWooshHit += HandleWooshHit;
        chainLightningBehav.OnChainHit += HandleChainHit;
    }

    private void OnDisable()
    {
        wooshScript.OnWooshHit -= HandleWooshHit;
        chainLightningBehav.OnChainHit -= HandleChainHit;
    }

    private void HandleWooshHit(wooshScript woosh, GameObject target)
    {
        if (target.GetComponent<EnemyHealth>() != null)
            ShakeCamera(defaultIntensity, defaultDuration);
    }

    private void HandleChainHit(GameObject target)
    {
        if (target.GetComponent<EnemyHealth>() != null)
            ShakeCamera(defaultIntensity, defaultDuration);
    }

    public void ShakeCamera(float intensity, float shakeDuration)
    {
        perlin.AmplitudeGain = intensity;
        CancelInvoke("StopShake");
        Invoke("StopShake", shakeDuration);
    }

    private void StopShake()
    {
        perlin.AmplitudeGain = 0f;
    }
}
