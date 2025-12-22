using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform slotContainer;

    List<GameObject> spawnedSlots = new List<GameObject>();

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var slot in spawnedSlots)
            Destroy(slot);

        spawnedSlots.Clear();

        foreach (var item in inventory.items)
        {
            GameObject slot = Instantiate(slotPrefab, slotContainer);

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            TMP_Text amountText = slot.transform.Find("Amount").GetComponent<TMP_Text>();

            icon.sprite = item.data.icon;
            amountText.text = item.amount.ToString();

            spawnedSlots.Add(slot);
        }
    }
}
