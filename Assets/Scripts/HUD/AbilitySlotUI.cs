using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private AudioClip readySound;
    [Range(0f, 1f)] public float readySoundVolume = 1f;
    [Range(0f, 0.5f)] public float readyPitchVariation = 0.1f;

    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 6f;
    public float shakeSpeed = 30f;

    public float popScale = 1.2f;
    public float popDuration = 0.15f;
    public float popRotationAngle = 10f;

    public float cooldownDownOffset = 14f;
    public float cooldownMoveSmooth = 0.08f;

    public float cooldownScaleMultiplier = 0.85f;
    public float cooldownScaleSmooth = 0.08f;

    public float bgCooldownDownOffset = 10f;
    public float bgCooldownScaleMultiplier = 0.9f;
    public float bgCooldownMoveSmooth = 0.08f;
    public float bgCooldownScaleSmooth = 0.08f;

    private AbilitySO _ability;
    private bool _wasOnCooldown = false;
    private bool _isInitialized = false;

    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private Vector3 _bgOriginalScale;

    private Vector3 _positionVelocity;
    private Vector3 _scaleVelocity;
    private Vector3 _bgPositionVelocity;
    private Vector3 _bgScaleVelocity;

    public AbilitySO Ability { get { return _ability; } }
    public bool HasAbility { get { return _ability != null; } }

    void Awake()
    {
        CacheOriginalTransforms();
    }

    private void CacheOriginalTransforms()
    {
        RectTransform rt = iconImage.rectTransform;
        _originalPosition = rt.anchoredPosition;
        _originalScale = rt.localScale;
        _originalRotation = rt.localRotation;

        _bgOriginalScale = backgroundImage.rectTransform.localScale;

        _isInitialized = true;
    }

    public void SetAbility(AbilitySO ability)
    {
        _ability = ability;

        if (!_isInitialized)
            CacheOriginalTransforms();

        if (_ability != null)
        {
            iconImage.sprite = _ability.Icon;
            iconImage.enabled = true;

            fadeImage.sprite = _ability.Icon;
            fadeImage.color = Color.black;
            fadeImage.enabled = _ability.cooldownTimer > 0;
            fadeImage.fillAmount = _ability.cooldown > 0 ? _ability.cooldownTimer / _ability.cooldown : 0;

            backgroundImage.enabled = true;

            _wasOnCooldown = _ability.cooldownTimer > 0;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        _ability = null;

        iconImage.enabled = false;
        fadeImage.enabled = false;
        backgroundImage.enabled = false;
    }

    public void SetVisible(bool visible)
    {
        iconImage.enabled = visible && _ability != null;
        fadeImage.enabled = visible && _ability != null;
        backgroundImage.enabled = visible && _ability != null;
    }

    void Update()
    {
        if (_ability == null) return;

        UpdateCooldownVisuals();
    }

    private void UpdateCooldownVisuals()
    {
        float ratio = _ability.cooldown > 0
            ? Mathf.Clamp01(_ability.cooldownTimer / _ability.cooldown)
            : 0f;

        fadeImage.fillAmount = ratio;
        fadeImage.enabled = ratio > 0f;

        bool onCooldown = ratio > 0f;

        Vector3 targetPos = _originalPosition + (onCooldown ? Vector3.down * cooldownDownOffset : Vector3.zero);
        Vector3 smoothedPos = Vector3.SmoothDamp(
            iconImage.rectTransform.anchoredPosition,
            targetPos,
            ref _positionVelocity,
            cooldownMoveSmooth
        );

        iconImage.rectTransform.anchoredPosition = smoothedPos;
        fadeImage.rectTransform.anchoredPosition = smoothedPos;

        float scaleFactor = Mathf.Lerp(1f, cooldownScaleMultiplier, ratio);
        Vector3 targetScale = _originalScale * scaleFactor;
        Vector3 smoothedScale = Vector3.SmoothDamp(
            iconImage.rectTransform.localScale,
            targetScale,
            ref _scaleVelocity,
            cooldownScaleSmooth
        );

        iconImage.rectTransform.localScale = smoothedScale;
        fadeImage.rectTransform.localScale = smoothedScale;

        Transform bgTransform = backgroundImage.transform;

        Vector3 targetWorldPos = iconImage.transform.position + Vector3.down * bgCooldownDownOffset;
        Vector3 smoothedWorldPos = Vector3.SmoothDamp(
            bgTransform.position,
            targetWorldPos,
            ref _bgPositionVelocity,
            bgCooldownMoveSmooth
        );
        bgTransform.position = smoothedWorldPos;

        Vector3 targetBgScale = _bgOriginalScale * (onCooldown ? bgCooldownScaleMultiplier : 1f);
        Vector3 smoothedBgScale = Vector3.SmoothDamp(
            bgTransform.localScale,
            targetBgScale,
            ref _bgScaleVelocity,
            bgCooldownScaleSmooth
        );
        bgTransform.localScale = smoothedBgScale;

        if (!_wasOnCooldown && onCooldown)
            StartCoroutine(ShakeEffect());

        if (_wasOnCooldown && !onCooldown)
            StartCoroutine(PopAndFlashEffect());

        _wasOnCooldown = onCooldown;
    }

    private IEnumerator ShakeEffect()
    {
        RectTransform icon = iconImage.rectTransform;
        RectTransform fade = fadeImage.rectTransform;
        Transform bg = backgroundImage.transform;

        Vector3 basePos = icon.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float x = Mathf.Sin(elapsed * shakeSpeed) * shakeMagnitude;
            float y = Mathf.Cos(elapsed * shakeSpeed * 1.3f) * shakeMagnitude * 0.6f;
            Vector3 offset = new Vector3(x, y, 0);

            icon.anchoredPosition = basePos + offset;
            fade.anchoredPosition = basePos + offset;
            bg.position = icon.transform.position + offset;

            yield return null;
        }
    }

    private IEnumerator PopAndFlashEffect()
    {
        PlayReadySound();

        RectTransform icon = iconImage.rectTransform;
        Image img = iconImage;

        Vector3 baseScale = _originalScale;
        Quaternion baseRot = _originalRotation;
        Color baseColor = img.color;
        Vector3 bgBaseScale = _bgOriginalScale;

        float elapsed = 0f;
        float randomRot = Random.Range(-popRotationAngle, popRotationAngle);

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;

            float scale = Mathf.Lerp(1f, popScale, Mathf.Sin(t * Mathf.PI));
            icon.localScale = baseScale * scale;
            fadeImage.rectTransform.localScale = baseScale * scale;
            backgroundImage.rectTransform.localScale = bgBaseScale * scale;

            float rot = Mathf.Lerp(0f, randomRot, Mathf.Sin(t * Mathf.PI));
            icon.localRotation = baseRot * Quaternion.Euler(0, 0, rot);

            float flash = 1f - Mathf.Abs(t * 2f - 1f);
            img.color = Color.Lerp(baseColor, Color.white, flash);

            yield return null;
        }

        icon.localScale = baseScale;
        icon.localRotation = baseRot;
        img.color = baseColor;

        fadeImage.rectTransform.localScale = baseScale;
        backgroundImage.rectTransform.localScale = bgBaseScale;
    }

    private void PlayReadySound()
    {
        if (readySound == null) return;

        GameObject temp = new GameObject("TempReadyAudio");
        AudioSource src = temp.AddComponent<AudioSource>();

        src.clip = readySound;
        src.volume = readySoundVolume;
        src.pitch = 1f + Random.Range(-readyPitchVariation, readyPitchVariation);
        src.spatialBlend = 0f;
        src.Play();

        Destroy(temp, readySound.length / Mathf.Abs(src.pitch));
    }
}
