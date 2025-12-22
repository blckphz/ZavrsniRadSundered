using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<ItemInstance> items = new List<ItemInstance>();

    public int maxSlots = 10;

    public void AddItem(Item itemData, int amount = 1)
    {
        string itemID = itemData.itemID;

        foreach (var item in items)
        {
            if (item.data == itemData && item.amount < item.data.maxStack)
            {
                int spaceLeft = item.data.maxStack - item.amount;
                int adding = Mathf.Min(spaceLeft, amount);
                item.amount += adding;
                amount -= adding;

                ItemEvents.OnItemCollected?.Invoke(itemID, adding);

                if (amount <= 0) return;
            }
        }

        while (amount > 0 && items.Count < maxSlots)
        {
            int adding = Mathf.Min(amount, itemData.maxStack);

            items.Add(new ItemInstance(itemData, adding));
            amount -= adding;

            ItemEvents.OnItemCollected?.Invoke(itemID, adding);
        }

        if (amount > 0)
            Debug.Log("Inventory full! Could not add all items.");
    }

    public void RemoveItem(ItemInstance item, int quantity = 1)
    {
        if (!items.Contains(item)) return;

        item.amount -= quantity;

        if (item.amount <= 0)
        {
            items.Remove(item);
            Debug.Log("Removed " + item.data.itemName + " completely");
        }
        else
        {
            Debug.Log("Removed " + quantity + " " + item.data.itemName);
        }
    }

    public int GetItemAmount(string itemID)
    {
        int totalAmount = 0;
        foreach (var item in items)
        {
            if (item.data.itemID.ToLower() == itemID.ToLower())
            {
                totalAmount += item.amount;
            }
        }
        return totalAmount;
    }

    public void PrintInventory()
    {
        Debug.Log("=== Inventory ===");
        if (items.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return;
        }

        foreach (var item in items)
            Debug.Log("- " + item.data.itemName + " x" + item.amount + " [" + item.data.itemType + "]");
    }
}
