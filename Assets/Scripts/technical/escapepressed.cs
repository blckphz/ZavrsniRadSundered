using UnityEngine;

public class escapepressed : MonoBehaviour
{
    public GameObject targetObject;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleObject();
        }
    }

    void ToggleObject()
    {
        if (targetObject == null) return;

        bool isActive = !targetObject.activeSelf;
        targetObject.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;
    }
}
