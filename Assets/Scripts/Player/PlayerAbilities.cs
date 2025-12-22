using UnityEngine;
using UnityEngine.InputSystem;
using System;

[System.Serializable]
public struct AbilityInput
{
    public AbilitySO ability;
    public KeyCode key;
}

public class PlayerAbilities : MonoBehaviour
{
    public AbilityInput[] abilities;

    public PlayerInput playerInput;
    public GameObject player;


    void LateUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }


        foreach (var ab in abilities)
        {
            if (ab.ability == null) continue;

            ab.ability.TickCooldown(Time.deltaTime);

            if (Input.GetKey(ab.key))
            {
                ab.ability.UseAbility(player);
            }
        }
    }

}
