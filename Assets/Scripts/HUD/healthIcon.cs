using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;

public class HealthUIController : MonoBehaviour
{
    public Image filledHealthImage;
    public Image backgroundHealthImage;
    public PlayerHealth playerHealthScript;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 8f;
    public float shakeSpeed = 30f;

    private float lastHealth;
    private Vector3 originalFillPos;
    private Vector3 originalBGPos;

    async void Start()
    {
        while (playerHealthScript == null)
        {
            playerHealthScript = FindAnyObjectByType<PlayerHealth>();
            if (playerHealthScript != null) break;
            await Task.Yield();
        }

        if (filledHealthImage == null || backgroundHealthImage == null)
        {
            Debug.LogError("Health UI not assigned");
            enabled = false;
            return;
        }

        originalFillPos = filledHealthImage.rectTransform.anchoredPosition;
        originalBGPos = backgroundHealthImage.rectTransform.anchoredPosition;

        lastHealth = playerHealthScript.currentHealth;
        UpdateUI();
    }

    void Update()
    {
        if (!playerHealthScript) return;

        UpdateUI();

        if (playerHealthScript.currentHealth < lastHealth)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeHealthIcons());
        }

        lastHealth = playerHealthScript.currentHealth;
    }

    private void UpdateUI()
    {
        float healthRatio = playerHealthScript.HealthPercentage;
        filledHealthImage.fillAmount = healthRatio;
    }

    private IEnumerator ShakeHealthIcons()
    {
        RectTransform fillIcon = filledHealthImage.rectTransform;
        RectTransform bgIcon = backgroundHealthImage.rectTransform;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Mathf.Sin(elapsed * shakeSpeed) * shakeMagnitude;
            float offsetY = Mathf.Sin(elapsed * shakeSpeed) * shakeMagnitude;

            Vector3 offset = new Vector3(offsetX, offsetY, 0);

            fillIcon.anchoredPosition = originalFillPos + offset;
            bgIcon.anchoredPosition = originalBGPos + offset;

            yield return null;
        }

        fillIcon.anchoredPosition = originalFillPos;
        bgIcon.anchoredPosition = originalBGPos;
    }
}
