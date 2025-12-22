using UnityEngine;

[CreateAssetMenu(fileName = "New Chain Lightning Ability", menuName = "Abilities/Chain Lightning")]
public class chainLightningAbil : rangedOffensive
{
    public int maxBounces;
    public float damageFalloff;
  
    public GameObject thunderPrefab;
    public float rotationOffset;


    protected override void ExecuteAbility(GameObject user, Vector2 aimDir)
    {

        GameObject thunder = Instantiate(thunderPrefab, user.transform.position, Quaternion.identity);
        var chain = thunder.GetComponent<chainLightningBehav>();
        if (chain != null)
        {
            chain.Init(this, user, aimDir, rotationOffset);
            TriggerCooldown();
        }
        else
        {
            Debug.LogError("chain missing");
        }
    }

   
}
