using UnityEngine;
using System.Collections;

public class shakeHealthbar : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 0.2f;

    [SerializeField]
    private float shakeMagnitude = 1.0f;

    [SerializeField]
    private float shakeFrequency = 50f;

    private Vector3 originalPosition;

    private bool isShaking = false;

    public Transform healthbarIconsTransform;


    void Start()
    {
        if (healthbarIconsTransform != null)
        {
            originalPosition = healthbarIconsTransform.localPosition;
        }
        else
        {
            Debug.LogError("healthbar not assigned");
            originalPosition = this.transform.localPosition;
        }
    }

    public void TriggerShake()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0.0f;

        Transform targetTransform = healthbarIconsTransform != null ? healthbarIconsTransform : this.transform;

        while (elapsed < shakeDuration)
        {
            float x = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * shakeMagnitude;
            float y = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * shakeMagnitude;

            targetTransform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        targetTransform.localPosition = originalPosition;
        isShaking = false;
    }
}
