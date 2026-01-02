using UnityEngine;
using Unity.Cinemachine;

public class PlayerAim : MonoBehaviour
{
    public Transform player;
    public QuestLogUI questLog;      
    public CinemachineCamera vCam;  

    public float maxDistance = 3f;
    public float followSpeed = 5f;

    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("player(Clone)").transform;
        }

        if (questLog != null && questLog.IsOpen())
        {
            vCam.Follow = player;
        }
        else
        {
            vCam.Follow = this.transform;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = mousePos - player.position;

        if (direction.magnitude > maxDistance)
        {
            direction = direction.normalized * maxDistance;
        }

        Vector3 targetPos = player.position + direction;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    public Vector2 GetAimDir()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (Vector2)mousePos - (Vector2)player.position;
        return dir.normalized;
    }
}