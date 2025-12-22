using UnityEngine;

public class AbilityBarUI : MonoBehaviour
{
    [SerializeField] private AbilitySlotUI[] slots = new AbilitySlotUI[4];

    [SerializeField] private PlayerAbilities playerAbilities;

    private bool _isHUDActive = true;

    public AbilitySlotUI[] Slots { get { return slots; } }
    public int SlotCount { get { return slots.Length; } }

    void Start()
    {
        if (playerAbilities == null)
        {
            playerAbilities = FindAnyObjectByType<PlayerAbilities>();
        }

        if (playerAbilities != null)
        {
            RefreshAllSlots();
        }
    }

    void Update()
    {
    }

    public void RefreshAllSlots()
    {
        if (playerAbilities == null || playerAbilities.abilities == null) return;

        AbilityInput[] abilities = playerAbilities.abilities;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            if (i < abilities.Length && abilities[i].ability != null)
            {
                slots[i].SetAbility(abilities[i].ability);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void SetSlotAbility(int index, AbilitySO ability)
    {
        if (index < 0 || index >= slots.Length || slots[index] == null) return;
        slots[index].SetAbility(ability);
    }

    public void SetHUDActive(bool active)
    {
        _isHUDActive = active;

        foreach (var slot in slots)
        {
            if (slot != null)
                slot.SetVisible(active);
        }
    }

    public void OnDialogueStarted()
    {
        if (playerAbilities != null)
            playerAbilities.enabled = false;

        SetHUDActive(false);
    }

    public void OnDialogueEnded()
    {
        if (playerAbilities != null)
            playerAbilities.enabled = true;

        SetHUDActive(true);
    }

    public AbilitySlotUI GetSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return null;
        return slots[index];
    }
}
