using UnityEngine;
using Unity.Cinemachine;

public class playerAim : MonoBehaviour
{
    public Transform player;
    public Transform followAnchor;
    public QuestLogUI questLogUI;
    public CinemachineCamera vCam;

    public float maxOffset = 3f;
    public float followSpeed = 5f;

    private Camera cam;

    public static playerAim Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("player(Clone)").transform;
        }

        if (vCam != null && questLogUI != null)
        {
            vCam.Follow = questLogUI.IsOpen() ? player : followAnchor;
        }

        Transform target = player;
        Vector3 mousePos = GetMouseWorldPosition();
        Vector3 direction = mousePos - target.position;
        Vector3 clampedOffset = Vector3.ClampMagnitude(direction, maxOffset);
        Vector3 targetPos = target.position + clampedOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

    public Vector2 GetAimDirection()
    {
        if (player == null) return Vector2.right;
        return ((Vector2)GetMouseWorldPosition() - (Vector2)player.position).normalized;
    }

    public static Vector2 GetAimDir()
    {
        if (Instance != null)
            return Instance.GetAimDirection();
        return Vector2.right;
    }
}
