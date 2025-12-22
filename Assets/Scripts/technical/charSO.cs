using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/Character Stats")]
public class CharSO : ScriptableObject
{
    public int maxHealth = 100;

    public float walkSpeed = 5f;

    public AbilitySO[] Abilities;

    [HideInInspector]
    public int currentHealth;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }
}
