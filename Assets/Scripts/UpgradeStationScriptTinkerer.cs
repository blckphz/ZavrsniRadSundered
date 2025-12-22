using UnityEngine;

public class UpgradeStationScript : MonoBehaviour
{
    public chainLightningAbil Thunderwave;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Thunderwave.damageFalloff = 2f;
        }
    }
}
