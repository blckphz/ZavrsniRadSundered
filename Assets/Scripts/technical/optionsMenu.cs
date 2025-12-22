using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class optionsMenu : MonoBehaviour
{
    [System.Serializable]
    public class VolumeChannel
    {
        public string parameterName;
        public Slider slider;
        public float defaultValue = 0.75f;
    }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private VolumeChannel[] channels;

    void Start()
    {
        foreach (var channel in channels)
        {
            if (channel.slider == null) continue;

            float savedValue = PlayerPrefs.GetFloat(channel.parameterName, channel.defaultValue);
            channel.slider.value = savedValue;
            SetVolume(channel.parameterName, savedValue);

            string param = channel.parameterName;
            channel.slider.onValueChanged.AddListener(value => SetVolume(param, value));
        }
    }

    private void SetVolume(string parameter, float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, db);
        PlayerPrefs.SetFloat(parameter, value);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}