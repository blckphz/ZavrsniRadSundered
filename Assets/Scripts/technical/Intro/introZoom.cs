using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroZoom : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float finalTargetFOV = 80f;

    public float triggerFOV_1 = 65f;
    public float jumpToFOV_1 = 75f;
    private bool jump1Done = false;

    public float triggerFOV_2 = 78f;
    public float jumpToFOV_2 = 90f;
    private bool jump2Done = false;

    public screenShaker shaker;
    public CinemachineCamera cineCam;

    public AudioSource audioSource;
    public AudioClip jump1AudioClip;

    public GameObject gameUI;
    public CanvasGroup gameCanvasGroup;
    public float uiFadeDuration = 1.0f;

    public float sceneSwitchFOV = 114f;
    public string nextSceneName = "LorrinsScene";
    private bool sceneSwitched = false;

    private void Awake()
    {
        if (cineCam == null)
            cineCam = GetComponent<CinemachineCamera>();

        // Find the UI GameObject by name
        if (gameUI == null)
        {
                gameUI = GameObject.Find("gameCanvas");
        }

        // Get the CanvasGroup and ensure its initial state is hidden (alpha = 0)
        if (gameUI != null)
        {
            gameCanvasGroup = gameUI.GetComponent<CanvasGroup>();
        }
    }

    private void Update()
    {

        // Find the UI GameObject by name
        if (gameUI == null)
        {
                gameUI = GameObject.Find("gameCanvas");
        }

        if (cineCam == null)
            return;

        // Check for Skip Input
        if (Input.GetKeyDown(KeyCode.Space) && !sceneSwitched)
        {
            SkipIntro();
            return;
        }

        // Your attempt to find gameCanvas in Update() is redundant and can be removed, 
        // as the UI object is found in Awake().

        var lens = cineCam.Lens;

        // Smooth zoom
        lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, finalTargetFOV, Time.deltaTime * zoomSpeed);
        cineCam.Lens = lens;

        // ---- TRIGGER JUMP #1 ----
        if (!jump1Done && lens.FieldOfView >= triggerFOV_1)
        {
            DoJump(jumpToFOV_1, 1);
            jump1Done = true;

            // Play audio
            if (audioSource != null && jump1AudioClip != null)
            {
                audioSource.clip = jump1AudioClip;
                audioSource.Play();
            }
        }

        // ---- TRIGGER JUMP #2 ----
        if (!jump2Done && lens.FieldOfView >= triggerFOV_2)
        {
            DoJump(jumpToFOV_2, 2);
            jump2Done = true;
        }

        // ---- SWITCH SCENE WHEN FOV REACHES TARGET ----
        if (!sceneSwitched && lens.FieldOfView >= sceneSwitchFOV)
        {
            EndIntroAndSwitchScene();
        }
    }

    private void DoJump(float jumpToFOV, int jumpIndex)
    {
        var lens = cineCam.Lens;
        lens.FieldOfView = jumpToFOV;
        cineCam.Lens = lens;

        Debug.Log("Jump " + jumpIndex + " triggered");

        if (shaker != null)
            shaker.ShakeCamera(0.1f, 0.1f);
    }

    // NEW CENTRALIZED METHOD TO END THE INTRO AND START SCENE LOAD
    private void EndIntroAndSwitchScene()
    {
        sceneSwitched = true;

        // 1. Fade In and Enable the Game UI
        if (gameCanvasGroup != null)
        {
            gameCanvasGroup.alpha = 1f;
            //StartCoroutine(FadeCanvasGroup(gameCanvasGroup, 0.01f, uiFadeDuration));
            Debug.Log("UI fade started");
        }
        else if (gameUI != null)
        {
            // Fallback if canvas group is somehow missed
           // gameUI.SetActive(true);
            Debug.Log("UI activated");
        }

        // 2. Start the scene load
        StartCoroutine(LoadSceneAsync(nextSceneName));
    }

    private void SkipIntro()
    {
        if (sceneSwitched) return;

        // Stop any ongoing audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        Debug.Log("Intro skipped! Loading scene: " + nextSceneName);

        gameUI.SetActive(true);

        // Use the centralized method to ensure UI is activated
        EndIntroAndSwitchScene();
    }


    private IEnumerator LoadSceneAsync(string sceneName)
    {
       
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }
    }
}