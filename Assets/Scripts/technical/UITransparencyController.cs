using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITransparencyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Camera mainCamera;

    [SerializeField, Range(0f, 1f)] private float fadedAlpha = 0.3f;
    [SerializeField] private float fadeSpeed = 5f;

    private RectTransform rectTransform;
    private bool playerOverlapping;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null || !mainCamera.isActiveAndEnabled)
            mainCamera = Camera.main;

        if (player == null || mainCamera == null)
            return;

        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(player.position);

        playerOverlapping = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, playerScreenPos);

        float targetAlpha = playerOverlapping ? fadedAlpha : 1f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }
}
