using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Container : MonoBehaviour
{
    [System.Serializable]
    public class ContainerItem
    {
        public Item itemData;
        public int amount = 1;
    }

    public Sprite EmptySprite;
    private Sprite defaultSprite;
    public SpriteRenderer spriteRenderer;

    public List<ContainerItem> items = new List<ContainerItem>();
    public bool isOpen = false;
    public bool isLooted = false; 

    public ContainerUI containerUI;
    public Inventory playerInventory;

    private void Awake()
    {
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        playerInventory = other.GetComponent<Inventory>();
        isOpen = true;
            containerUI.playerInventory = playerInventory;
            containerUI.OpenContainer(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Inventory inv = other.GetComponent<Inventory>();
        if (inv == null) return;

        if (isOpen && containerUI != null)
        {
            containerUI.CloseContainer();
            isOpen = false;
        }
    }

    public void AutoLoot(Inventory playerInventory)
    {
        foreach (var containerItem in new List<ContainerItem>(items))
        {
            playerInventory.AddItem(containerItem.itemData, containerItem.amount);
            items.Remove(containerItem);
            isLooted = true;
            spriteRenderer.sprite = EmptySprite;
        }
    }

}