using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public Item data;
    public int amount;

    public ItemInstance(Item data, int amount = 1)
    {
        this.data = data;
        this.amount = amount;
    }

    public void Use()
    {
        if (data.itemType == ItemType.Consumable)
        {
            Debug.Log(data.itemName + " used!");
        }
        else
        {
            Debug.Log(data.itemName + " cannot be used.");
        }
    }
}
