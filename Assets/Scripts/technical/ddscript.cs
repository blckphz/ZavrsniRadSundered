using UnityEngine;

public class ddscript : MonoBehaviour
{
    public static ddscript Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

    }
}
