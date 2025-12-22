using UnityEngine;
using TMPro;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float duration = 1f;
    public float fadeOutTime = 0.25f;

    public float baseScale = 0.35f;
    public float critScale = 1.5f;

    public bool usePhysics = true;
    public float minForce = 1f;
    public float maxForce = 3f;

    private Rigidbody2D rb;
    private Action<DamageNumber> returnToPool;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialize(Action<DamageNumber> returnAction)
    {
        returnToPool = returnAction;
    }

    public void Show(int damage, Vector3 worldPos, float yOffset, bool isCrit = false)
    {
        transform.position = worldPos + Vector3.up * yOffset;

        if (text != null)
        {
            text.text = string.Empty;
            text.ForceMeshUpdate(true);
            text.SetAllDirty();

            text.text = damage.ToString();
            text.color = isCrit ? Color.yellow : Color.white;
            text.alpha = 1f;
        }

        transform.localScale = Vector3.one * baseScale;
        if (isCrit)
            transform.localScale *= critScale;

        gameObject.SetActive(true);

        if (usePhysics && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            float angle = UnityEngine.Random.Range(-45f, 45f);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            float force = UnityEngine.Random.Range(minForce, maxForce);
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(HideAfterDuration());
    }

    private IEnumerator HideAfterDuration()
    {
        yield return new WaitForSeconds(duration - fadeOutTime);

        if (text != null)
        {
            float elapsed = 0f;
            float startAlpha = text.alpha;

            while (elapsed < fadeOutTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeOutTime;
                text.alpha = Mathf.Lerp(startAlpha, 0f, t);
                yield return null;
            }
        }

        returnToPool?.Invoke(this);
    }
}
