using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayClip(AudioClip clip, float pitchVariation = 0.1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("null clip");
            return;
        }

        GameObject temp = new GameObject("TempSFX_" + clip.name);
        AudioSource src = temp.AddComponent<AudioSource>();

        src.clip = clip;

        src.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        src.spatialBlend = 0f; 

        src.Play();

        Destroy(temp, clip.length + 0.1f);
    }
}