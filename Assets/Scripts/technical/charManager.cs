using UnityEngine;
using System;



public class charManager : MonoBehaviour
{
    public CharSO currentchat;
    public PlayerAbilities playerAbilities;
    public attackhudcontroller attackHUD;

    public static Action<PlayerAbilities> OnPlayerAbilitiesReady;

    void Awake()
    {
        OnPlayerAbilitiesReady += SetPlayerAbilitiesReference;
    }

    void OnDestroy()
    {
        OnPlayerAbilitiesReady -= SetPlayerAbilitiesReference;
    }

    void Update()
    {
        playerAbilities = FindAnyObjectByType<PlayerAbilities>();
        assignSkills();
    }

    public void assignSkills()
    {
        attackHUD = FindAnyObjectByType<attackhudcontroller>();
        CheckAndRunFullSetup();
    }

    public void SetPlayerAbilitiesReference(PlayerAbilities abilities)
    {
        playerAbilities = abilities;
        CheckAndRunFullSetup();
    }

    private void CheckAndRunFullSetup()
    {
        SetupCharacterAbilities();
    }

    public void SetupCharacterAbilities()
    {

        int charAbilityCount = currentchat.Abilities.Length;

        if (playerAbilities.abilities == null || playerAbilities.abilities.Length == 0)
        {
            playerAbilities.abilities = new AbilityInput[charAbilityCount];

            for (int i = 0; i < charAbilityCount; i++)
            {
                playerAbilities.abilities[i] = new AbilityInput
                {
                    ability = currentchat.Abilities[i],
                    key = KeyCode.Alpha1 + i
                };
            }
        }
        else
        {
            for (int i = 0; i < Mathf.Min(playerAbilities.abilities.Length, charAbilityCount); i++)
            {
                playerAbilities.abilities[i].ability = currentchat.Abilities[i];
            }
        }

        attackHUD.RefreshIcons();
    }

    public void ChangeCharacter(CharSO newChar)
    {
        currentchat = newChar;
        CheckAndRunFullSetup();
    }
}
