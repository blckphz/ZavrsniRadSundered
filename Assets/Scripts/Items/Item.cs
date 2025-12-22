using UnityEngine;

public enum ItemType
{
    Consumable,
    Junk
}

public class Item : ScriptableObject
{
    public string itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public int maxStack = 1;
}
