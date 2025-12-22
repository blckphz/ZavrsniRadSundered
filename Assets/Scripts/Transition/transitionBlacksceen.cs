using System.Collections;
using UnityEngine;

public class transitionBlacksceen : MonoBehaviour
{
    public Animator animator;
    public SaveScript saveManager;

    void OnEnable()
    {
        TransitionEvents.OnTransitionBegin += PlayBlackTransition;
    }

    void OnDisable()
    {
        TransitionEvents.OnTransitionBegin -= PlayBlackTransition;
    }

    public IEnumerator PlayBlackTransitionCoroutine()
    {
        animator.Play("transitionState");

        yield return null;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);
    }

    public void ContinueExistingGame()
    {
        saveManager.LoadGame();
    }

    public void ContinueNewGame()
    {
        saveManager.LoadGame();
    }

    void PlayBlackTransition()
    {
        StartCoroutine(PlayBlackTransitionCoroutine());
    }
}
