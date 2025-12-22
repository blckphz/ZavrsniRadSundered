using UnityEngine;
using Unity.Cinemachine;

public class BoundariesManager : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner;
    [SerializeField] private PolygonCollider2D polygonCollider;

    void Start()
    {
        if (cinemachineConfiner == null)
            cinemachineConfiner = FindAnyObjectByType<CinemachineConfiner2D>();

        if (polygonCollider == null)
            polygonCollider = FindAnyObjectByType<PolygonCollider2D>();

        if (cinemachineConfiner != null && polygonCollider != null)
            cinemachineConfiner.BoundingShape2D = polygonCollider;
    }

    public void SetBoundary(PolygonCollider2D newBoundary)
    {
        polygonCollider = newBoundary;
        if (cinemachineConfiner != null)
            cinemachineConfiner.BoundingShape2D = polygonCollider;
    }
}
