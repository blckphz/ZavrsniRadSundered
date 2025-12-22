using UnityEngine;
using System.Collections;

public class EnemyVisuals : MonoBehaviour
{
    public Renderer rend;
    public float flashDuration = 0.2f;

    public Color stunColor = new Color(1f, 0.9f, 0.4f);
    public float stunIntensity = 0.2f;

    private Material mat;
    private float flashTimer;

    void Awake()
    {
        if (rend == null) rend = GetComponentInChildren<Renderer>();
        if (rend != null) mat = rend.material;
    }

    void Update()
    {
        if (mat != null)
        {
            if (flashTimer > 0)
            {
                flashTimer -= Time.deltaTime;
                mat.SetFloat("_Intensity", Mathf.Clamp01(flashTimer / flashDuration));
            }
            else
            {
                mat.SetFloat("_Intensity", 0f);
            }
        }
    }

    public void TriggerHit()
    {
        flashTimer = flashDuration;
    }

    public IEnumerator AnimateStunVisuals(bool enable, float duration)
    {
        if (mat == null) yield break;

        mat.SetColor("_StunColor", stunColor);
        float targetIntensity = enable ? stunIntensity : 0f;
        float startIntensity = mat.GetFloat("_StunIntensity");
        float animDuration = enable ? 0.15f : 0.3f;

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            mat.SetFloat("_StunIntensity", Mathf.Lerp(startIntensity, targetIntensity, elapsed / animDuration));
            yield return null;
        }
        mat.SetFloat("_StunIntensity", targetIntensity);
    }
}
