using UnityEngine;
using UnityEngine.Tilemaps;

public class houseTrans : MonoBehaviour
{
    public float fadedAlpha = 0.3f;
    public float fadeSpeed = 5f;

    public Renderer specialDetailRenderer;

    private Tilemap tilemap;
    private float targetAlpha;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        targetAlpha = tilemap.color.a;
    }

    void Update()
    {
        Color c = tilemap.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        tilemap.color = c;

        if (specialDetailRenderer != null)
        {
            Color detailColor = specialDetailRenderer.material.color;
            detailColor.a = c.a;
            specialDetailRenderer.material.color = detailColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetAlpha = fadedAlpha;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetAlpha = 1f;
        }
    }
}
