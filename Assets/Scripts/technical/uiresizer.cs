using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class uiresizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private float targetScaleFactor = 1.2f;

    [SerializeField]
    private float resizeSpeed = 10f;

    [SerializeField]
    private TextMeshProUGUI targetText;

    [SerializeField]
    private Color hoverColor = Color.white;

    [SerializeField]
    private Color nonHoverColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    [SerializeField]
    private UnityEvent onClick;

    [SerializeField]
    private AudioClip hoverSound;

    [SerializeField]
    private AudioSource audioSource;

    private Vector3 originalScale;
    private Vector3 enlargedScale;
    private RectTransform rectTransform;
    private Coroutine resizeCoroutine;
    private Coroutine colorCoroutine;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (targetText != null)
        {
            targetText.color = nonHoverColor;
        }

        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
            enlargedScale = new Vector3(
                originalScale.x * targetScaleFactor,
                originalScale.y * targetScaleFactor,
                originalScale.z
            );
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        resizeCoroutine = StartCoroutine(ResizeAnimation(enlargedScale));

        if (targetText != null)
        {
            colorCoroutine = StartCoroutine(ColorAnimation(hoverColor));
        }

        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        resizeCoroutine = StartCoroutine(ResizeAnimation(originalScale));

        if (targetText != null)
        {
            colorCoroutine = StartCoroutine(ColorAnimation(nonHoverColor));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick.Invoke();
        }
    }

    private IEnumerator ResizeAnimation(Vector3 target)
    {
        while (Vector3.Distance(rectTransform.localScale, target) > 0.001f)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                target,
                Time.deltaTime * resizeSpeed
            );
            yield return null;
        }

        rectTransform.localScale = target;
        resizeCoroutine = null;
    }

    private IEnumerator ColorAnimation(Color target)
    {
        while (targetText.color != target)
        {
            targetText.color = Color.Lerp(
                targetText.color,
                target,
                Time.deltaTime * resizeSpeed
            );
            yield return null;
        }

        targetText.color = target;
        colorCoroutine = null;
    }
}
