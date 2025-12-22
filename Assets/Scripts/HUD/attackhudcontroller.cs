using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class attackhudcontroller : MonoBehaviour
{
    public PlayerAbilities playerAbilities;

    public Image[] abilityIconImages = new Image[4];
    public Image[] abilityFade = new Image[4];
    public Image[] abilityBackground = new Image[4];

    public AudioClip[] readySounds = new AudioClip[4];
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

    private bool[] wasOnCooldown = new bool[4];
    private Vector3[] originalPositions = new Vector3[4];
    private Vector3[] originalScales = new Vector3[4];
    private Quaternion[] originalRotations = new Quaternion[4];

    private Vector3[] positionVelocity = new Vector3[4];
    private Vector3[] scaleVelocity = new Vector3[4];

    private Vector3[] bgOriginalScales = new Vector3[4];
    private Vector3[] bgPositionVelocity = new Vector3[4];
    private Vector3[] bgScaleVelocity = new Vector3[4];

    void Start()
    {

        for (int i = 0; i < 4; i++)
        {
            if (abilityIconImages[i] != null)
            {
                RectTransform rt = abilityIconImages[i].rectTransform;
                originalPositions[i] = rt.anchoredPosition;
                originalScales[i] = rt.localScale;
                originalRotations[i] = rt.localRotation;
            }

            if (abilityBackground[i] != null)
            {
                bgOriginalScales[i] = abilityBackground[i].rectTransform.localScale;
            }
        }
    }

    void Update()
    {
        if (playerAbilities == null)
        {
            playerAbilities = FindAnyObjectByType<PlayerAbilities>();
            if (playerAbilities != null)
                RefreshIcons();
        }

        UpdateCooldownVisuals();
    }

    public void RefreshIcons()
    {
        if (playerAbilities != null && playerAbilities.abilities != null)
            SetupAbilityIcons();
    }

    private void SetupAbilityIcons()
    {
        AbilityInput[] abilities = playerAbilities.abilities;

        for (int i = 0; i < 4; i++)
        {
            if (i < abilities.Length && abilities[i].ability != null)
            {
                AbilitySO ability = abilities[i].ability;

                abilityIconImages[i].sprite = ability.Icon;
                abilityIconImages[i].enabled = true;

                abilityFade[i].sprite = ability.Icon;
                abilityFade[i].color = Color.black;
                abilityFade[i].enabled = ability.cooldownTimer > 0;
                abilityFade[i].fillAmount =
                    ability.cooldown > 0 ? ability.cooldownTimer / ability.cooldown : 0;

                if (abilityBackground[i] != null)
                    abilityBackground[i].enabled = true;

                wasOnCooldown[i] = ability.cooldownTimer > 0;
            }
            else
            {
                if (abilityIconImages[i] != null) abilityIconImages[i].enabled = false;
                if (abilityFade[i] != null) abilityFade[i].enabled = false;
                if (abilityBackground[i] != null) abilityBackground[i].enabled = false;
            }
        }
    }

    private void UpdateCooldownVisuals()
    {
        if (playerAbilities == null || playerAbilities.abilities == null) return;

        for (int i = 0; i < 4; i++)
        {
            if (i >= playerAbilities.abilities.Length) continue;

            AbilitySO ability = playerAbilities.abilities[i].ability;
            if (ability == null || abilityIconImages[i] == null || abilityFade[i] == null)
                continue;

            float ratio = ability.cooldown > 0
                ? Mathf.Clamp01(ability.cooldownTimer / ability.cooldown)
                : 0f;

            abilityFade[i].fillAmount = ratio;
            abilityFade[i].enabled = ratio > 0f;

            bool onCooldown = ratio > 0f;

            Vector3 targetPos = originalPositions[i] + (onCooldown ? Vector3.down * cooldownDownOffset : Vector3.zero);
            Vector3 smoothedPos = Vector3.SmoothDamp(
                abilityIconImages[i].rectTransform.anchoredPosition,
                targetPos,
                ref positionVelocity[i],
                cooldownMoveSmooth
            );

            abilityIconImages[i].rectTransform.anchoredPosition = smoothedPos;
            abilityFade[i].rectTransform.anchoredPosition = smoothedPos;

            float scaleFactor = Mathf.Lerp(1f, cooldownScaleMultiplier, ratio);
            Vector3 targetScale = originalScales[i] * scaleFactor;
            Vector3 smoothedScale = Vector3.SmoothDamp(
                abilityIconImages[i].rectTransform.localScale,
                targetScale,
                ref scaleVelocity[i],
                cooldownScaleSmooth
            );

            abilityIconImages[i].rectTransform.localScale = smoothedScale;
            abilityFade[i].rectTransform.localScale = smoothedScale;

            if (abilityBackground[i] != null)
            {
                Transform bgTransform = abilityBackground[i].transform;

                Vector3 targetWorldPos = abilityIconImages[i].transform.position + Vector3.down * bgCooldownDownOffset;
                Vector3 smoothedWorldPos = Vector3.SmoothDamp(
                    bgTransform.position,
                    targetWorldPos,
                    ref bgPositionVelocity[i],
                    bgCooldownMoveSmooth
                );
                bgTransform.position = smoothedWorldPos;

                Vector3 targetBgScale = bgOriginalScales[i] * (onCooldown ? bgCooldownScaleMultiplier : 1f);
                Vector3 smoothedBgScale = Vector3.SmoothDamp(
                    bgTransform.localScale,
                    targetBgScale,
                    ref bgScaleVelocity[i],
                    bgCooldownScaleSmooth
                );
                bgTransform.localScale = smoothedBgScale;
            }

            if (!wasOnCooldown[i] && onCooldown)
                StartCoroutine(ShakeIcon(i));

            if (wasOnCooldown[i] && !onCooldown)
                StartCoroutine(PopAndFlashIcon(i));

            wasOnCooldown[i] = onCooldown;
        }
    }

    private IEnumerator ShakeIcon(int index)
    {
        RectTransform icon = abilityIconImages[index].rectTransform;
        RectTransform fade = abilityFade[index].rectTransform;
        Transform bg = abilityBackground[index]?.transform;

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
            if (bg != null)
                bg.position = icon.transform.position + offset;

            yield return null;
        }
    }

    private IEnumerator PopAndFlashIcon(int index)
    {
        PlayClip(readySounds[index], Vector3.zero, readySoundVolume, readyPitchVariation);

        RectTransform icon = abilityIconImages[index].rectTransform;
        Image img = abilityIconImages[index];

        Vector3 baseScale = originalScales[index];
        Quaternion baseRot = originalRotations[index];
        Color baseColor = img.color;
        Vector3 bgBaseScale = bgOriginalScales[index];

        float elapsed = 0f;
        float randomRot = Random.Range(-popRotationAngle, popRotationAngle);

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;

            float scale = Mathf.Lerp(1f, popScale, Mathf.Sin(t * Mathf.PI));
            icon.localScale = baseScale * scale;
            abilityFade[index].rectTransform.localScale = baseScale * scale;
            if (abilityBackground[index] != null)
                abilityBackground[index].rectTransform.localScale = bgBaseScale * scale;

            float rot = Mathf.Lerp(0f, randomRot, Mathf.Sin(t * Mathf.PI));
            icon.localRotation = baseRot * Quaternion.Euler(0, 0, rot);

            float flash = 1f - Mathf.Abs(t * 2f - 1f);
            img.color = Color.Lerp(baseColor, Color.white, flash);

            yield return null;
        }

        icon.localScale = baseScale;
        icon.localRotation = baseRot;
        img.color = baseColor;

        abilityFade[index].rectTransform.localScale = baseScale;
        if (abilityBackground[index] != null)
            abilityBackground[index].rectTransform.localScale = bgBaseScale;
    }

    private void PlayClip(AudioClip clip, Vector3 pos, float vol, float pitchVar)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("TempReadyAudio");
        AudioSource src = temp.AddComponent<AudioSource>();

        src.clip = clip;
        src.volume = vol;
        src.pitch = 1f + Random.Range(-pitchVar, pitchVar);
        src.spatialBlend = 0f;
        src.Play();

        Destroy(temp, clip.length / Mathf.Abs(src.pitch));
    }

    public void OnDialogueStarted()
    {
        if (playerAbilities != null)
            playerAbilities.enabled = false;

        SetHUDActive(false);
    }

    public void OnDialogueEnded()
    {
        if (playerAbilities != null)
            playerAbilities.enabled = true;

        SetHUDActive(true);
    }

    public void SetHUDActive(bool active)
    {
        foreach (var img in abilityIconImages)
            if (img != null) img.enabled = active;

        foreach (var fade in abilityFade)
            if (fade != null) fade.enabled = active;

        foreach (var bg in abilityBackground)
            if (bg != null) bg.enabled = active;
    }
}
