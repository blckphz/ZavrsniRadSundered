using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public SaveScript saveManager;
    private string defaultSlotName = "playerSave";

    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject buttonToHide;

    public string firstSceneName = "IntroScene";

    private void Start()
    {
        SaveScript.OnSaveFileDetected += HideButton;
        saveManager.CurrentSaveName = defaultSlotName;
    }

    public void OnClick_ContinueGame()
    {
        if (saveManager.CheckIfSaveExists(defaultSlotName))
        {
            TransitionEvents.OnTransitionBegin?.Invoke();
        }
        else
        {
            OnClick_NewGame();
        }
    }

    public void OnClick_NewGame()
    {
        saveManager.DeleteSave(defaultSlotName);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        saveManager.CurrentSaveName = defaultSlotName;
        TransitionEvents.OnTransitionBegin?.Invoke();
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;
    }

    public void ToggleOptions()
    {
        if (optionsMenuPanel.activeSelf)
        {
            mainMenuPanel.SetActive(true);
            optionsMenuPanel.SetActive(false);
        }
        else
        {
            optionsMenuPanel.SetActive(true);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }

    private void HideButton()
    {
       buttonToHide.SetActive(false);
    }

}
