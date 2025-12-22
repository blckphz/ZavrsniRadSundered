using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionScript : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string transitionID;

    private Collider2D doorCollider;

    private void Start()
    {
        doorCollider = GetComponent<Collider2D>();

        if (TransitionManager.LastUsedTransition == transitionID)
        {
            DisableTriggerOnSpawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && doorCollider.enabled)
        {

            TransitionManager.LastUsedTransition = transitionID;

            doorCollider.enabled = false;

            TransitionEvents.OnTransitionBegin?.Invoke();

            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
    }

    public void DisableTriggerOnSpawn()
    {
        doorCollider.enabled = false;
        StartCoroutine(ReEnableAfterDelay());
    }

    private IEnumerator ReEnableAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        doorCollider.enabled = true;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        SaveScript.IsLoadingGame = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
