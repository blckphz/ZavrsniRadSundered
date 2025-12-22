using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Turret", menuName = "Abilities/Turret")]
public class turretSO : offensiveCompanion
{
    public GameObject turretPrefab;
    public float turretLifetime = 10f;

    public float inputWindow = 0.25f;

    private bool inputBlocked = false;

    protected override void ExecuteAbility(GameObject user, Vector2 aimDir)
    {
        if (!IsReady() || inputBlocked) return;

        Vector2 spawnPos = user.transform.position;

        GameObject turretObj = Instantiate(turretPrefab, spawnPos, Quaternion.identity);

        turretBehav behav = turretObj.GetComponent<turretBehav>();

        behav.Setup(turretLifetime);

        TriggerCooldown();

        user.GetComponent<MonoBehaviour>().StartCoroutine(InputWindowCooldown());
    }

    private IEnumerator InputWindowCooldown()
    {
        inputBlocked = true;
        yield return new WaitForSeconds(inputWindow);
        inputBlocked = false;
    }
}
