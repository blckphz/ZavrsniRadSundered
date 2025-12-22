using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    public string ObjectID;

    private void Awake()
    {
        if (SaveScript.Instance == null) return;

        if (SaveScript.Instance.IsObjectDestroyed(ObjectID))
        {
            Destroy(gameObject);
        }
    }

    public void DestroyObject()
    {
        if (SaveScript.Instance != null)
        {
            SaveScript.Instance.RegisterDestroyedObject(ObjectID);
        }

        Destroy(gameObject);
    }
}
