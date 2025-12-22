using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Teleport Ability", menuName = "Abilities/Teleport Beacon")]
public class teleportBeacon : AbilitySO
{
    public GameObject beaconPrefab;
    public float throwForce = 8f;

    public float teleportDelay = 0.2f;
    public float inputWindow = 0.25f;

    private GameObject activeBeacon;
    private bool inputBlocked = false;

    protected override void ExecuteAbility(GameObject user, Vector2 aimDir)
    {
        if (inputBlocked) return;

        if (activeBeacon != null)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(TeleportAfterDelay(user));
            inputBlocked = true;
            user.GetComponent<MonoBehaviour>().StartCoroutine(InputWindowCooldown());
            return;
        }

        activeBeacon = Instantiate(beaconPrefab, user.transform.position, Quaternion.identity);

        Rigidbody2D rb = activeBeacon.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = aimDir.normalized * throwForce;


        inputBlocked = true;
        user.GetComponent<MonoBehaviour>().StartCoroutine(InputWindowCooldown());
    }

    private IEnumerator TeleportAfterDelay(GameObject user)
    {
        yield return new WaitForSeconds(teleportDelay);

        if (activeBeacon != null)
        {
            user.transform.position = activeBeacon.transform.position;
            GameObject.Destroy(activeBeacon);
            activeBeacon = null;

            TriggerCooldown();
        }
    }



    private IEnumerator InputWindowCooldown()
    {
        yield return new WaitForSeconds(inputWindow);
        inputBlocked = false;
    }
}
