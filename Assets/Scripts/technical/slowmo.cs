using UnityEngine;
using System.Collections;

public class slowmo : MonoBehaviour
{
    public float slowmoScale = 0.2f;
    public float slowmoDuration = 0.2f;
    public float slowmoTransition = 0.05f;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void HandleWooshHit(wooshScript woosh, GameObject target)
    {
        StartCoroutine(SlowMotionCoroutine());
    }

    public void TriggerSlowMotion()
    {
        StartCoroutine(SlowMotionCoroutine());
    }

    private IEnumerator SlowMotionCoroutine()
    {
        float startScale = Time.timeScale;
        float t = 0f;
        while (t < slowmoTransition)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, slowmoScale, t / slowmoTransition);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = slowmoScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(slowmoDuration);

        t = 0f;
        while (t < slowmoTransition)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowmoScale, 1f, t / slowmoTransition);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
