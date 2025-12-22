using UnityEngine;

[System.Serializable]
public class grenadeData
{
    public float fuseTime;
    public float explosionRadius;
    public int damage;
    public GameObject explosionPrefab;
    public float armingTime;
    public KeyCode manualDetonationKey;
}