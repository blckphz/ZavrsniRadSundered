using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class comboScript : MonoBehaviour
{
    public int comboCount = 0;
    public float comboResetTime = 3f;

    public TextMeshProUGUI comboText;
    public Image comboSkullIcon;
    public CanvasGroup comboCanvasGroup;

    public float fadeOutSpeed = 1.5f;
    public float popScale = 1.5f;
    public float scaleReturnSpeed = 10f;

    public float baseRotationAngle = 5f;
    public float maxRotationAngle = 30f;
    public float shakeIntensityCurve = 2f;

    public Rigidbody2D rb;
    public float fadeGravityScale = 1f;

    private Vector3 originalTextScale = Vector3.one;
    private Vector3 originalSkullScale = Vector3.one;
    private Vector3 originalTextPosition;
    private float comboTimer = 0f;

    private float currentShakeTargetAngle = 0f;
    private float currentSkullRotation = 0f;
    private float currentTextRotation = 0f;
    private bool isFadingOut = false;

    private Vector3 originalRbPosition;

    void OnEnable()
    {
        EnemyHealth.OnEnemyDied += AddCombo;
    }

    void OnDisable()
    {
        EnemyHealth.OnEnemyDied -= AddCombo;
    }

    void Start()
    {
        if (comboText != null)
        {
            originalTextScale = comboText.transform.localScale;
            originalTextPosition = comboText.transform.localPosition;
            comboText.text = "";
        }

        if (comboSkullIcon != null)
        {
            originalSkullScale = comboSkullIcon.transform.localScale;
            comboSkullIcon.enabled = false;
        }

        if (comboCanvasGroup != null)
            comboCanvasGroup.alpha = 0f;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            originalRbPosition = rb.transform.position;
        }
    }

    void Update()
    {
        HandleComboTimer();
        UpdateShakeIntensity();
        HandleAnimations();
        HandleFade();
    }

    void HandleComboTimer()
    {
        if (comboCount <= 1) return;

        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
        }
        else if (!isFadingOut)
        {
            isFadingOut = true;

            if (rb != null)
                rb.gravityScale = fadeGravityScale;
        }
    }

    void AddCombo(EnemyHealth enemy, GameObject attacker)
    {
        comboCount++;
        comboTimer = comboResetTime;
        isFadingOut = false;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.transform.position = originalRbPosition;
        }

        PopElements();
        WiggleSkull();
        WiggleText();
    }

    void UpdateShakeIntensity()
    {
        if (comboCount <= 1)
        {
            currentShakeTargetAngle = 0f;
            return;
        }

        float timeRatio = 1f - Mathf.Clamp01(comboTimer / comboResetTime);
        float intensityFactor = Mathf.Pow(timeRatio, shakeIntensityCurve);
        currentShakeTargetAngle = Mathf.Lerp(baseRotationAngle, maxRotationAngle, intensityFactor);
    }

    void HandleAnimations()
    {
        if (comboText != null)
        {
            comboText.transform.localScale = Vector3.Lerp(comboText.transform.localScale, originalTextScale, Time.deltaTime * scaleReturnSpeed);
            currentTextRotation = Mathf.Lerp(currentTextRotation, 0f, Time.deltaTime * scaleReturnSpeed);
            ApplyTextRotationPivot();
            if (Mathf.Abs(currentTextRotation) < 0.1f)
                comboText.transform.localPosition = originalTextPosition;
        }

        if (comboSkullIcon != null)
        {
            comboSkullIcon.transform.localScale = Vector3.Lerp(comboSkullIcon.transform.localScale, originalSkullScale, Time.deltaTime * scaleReturnSpeed);
            currentSkullRotation = Mathf.Lerp(currentSkullRotation, 0f, Time.deltaTime * scaleReturnSpeed);
            comboSkullIcon.transform.rotation = Quaternion.Euler(0f, 0f, currentSkullRotation);
        }
    }

    void PopElements()
    {
        if (comboText != null)
            comboText.transform.localScale = originalTextScale * popScale;

        if (comboSkullIcon != null)
            comboSkullIcon.transform.localScale = originalSkullScale * popScale;
    }

    void WiggleSkull()
    {
        if (comboSkullIcon != null)
            currentSkullRotation = Random.Range(-currentShakeTargetAngle, currentShakeTargetAngle);
    }

    void WiggleText()
    {
        if (comboText != null)
            currentTextRotation = Random.Range(-currentShakeTargetAngle, currentShakeTargetAngle);
    }

    void ApplyTextRotationPivot()
    {
        if (comboText == null) return;

        float offset = comboText.bounds.size.x;
        comboText.transform.localPosition = originalTextPosition - new Vector3(offset, 0, 0);
        comboText.transform.rotation = Quaternion.Euler(0f, 0f, currentTextRotation);
        comboText.transform.localPosition += new Vector3(offset, 0, 0);
    }

    void HandleFade()
    {
        if (comboCanvasGroup == null) return;

        if (comboCount > 1 && !isFadingOut)
        {
            comboCanvasGroup.alpha = 1f;
            if (comboText != null)
                comboText.text = comboCount + " HIT!";
            if (comboSkullIcon != null)
                comboSkullIcon.enabled = true;
        }

        if (isFadingOut)
        {
            comboCanvasGroup.alpha = Mathf.MoveTowards(comboCanvasGroup.alpha, 0f, Time.deltaTime * fadeOutSpeed);

            if (comboCanvasGroup.alpha <= 0.01f)
            {
                comboCanvasGroup.alpha = 0f;
                isFadingOut = false;

                comboCount = 0;
                comboTimer = 0f;

                if (comboText != null)
                {
                    comboText.text = "";
                    comboText.transform.localPosition = originalTextPosition;
                }

                if (comboSkullIcon != null)
                    comboSkullIcon.enabled = false;

                if (rb != null)
                {
                    rb.gravityScale = 0f;
                    rb.linearVelocity = Vector2.zero;
                    rb.transform.position = originalRbPosition;
                }
            }
        }
    }
}
