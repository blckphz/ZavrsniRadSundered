using UnityEngine;

public class tentBehav : MonoBehaviour
{
    public float fadedAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private float targetAlpha;
    private float defaultAlpha = 1f; 

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        targetAlpha = defaultAlpha;

        Color c = sr.color;
        c.a = targetAlpha;
        sr.color = c;
    }

    void Update()
    {
        Color c = sr.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        sr.color = c;
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
            targetAlpha = defaultAlpha;
        }
    }
}