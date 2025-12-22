using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ContainerUI : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform contentParent;
    public Inventory playerInventory;
    public GameObject uiCanvas;
    public float uidelay;

    public Container currentContainer;

    public List<GameObject> uiClones = new List<GameObject>();
    public List<Animator> uiANims = new List<Animator>();

    public AudioSource audioSource;     
    public AudioClip pickupSound;     
    public AudioClip openUISound;       

    private void Start()
    {
        uiCanvas.SetActive(false);
        RefreshUI();
    }

    public void OpenContainer(Container container)
    {
        currentContainer = container;

        RefreshUI();

        uiCanvas.SetActive(true);

        audioSource.PlayOneShot(openUISound);

        PlayPickupAnimations(uidelay);
    }

    public void CloseContainer()
    {

        foreach (var clone in uiClones)
        {
            Destroy(clone);
        }
        //uiClones.Clear();
        uiANims.Clear();

        uiCanvas.SetActive(false);
    }

    public void AutoLoot(Container container)
    {

        var itemsCopy = new List<Container.ContainerItem>(container.items);

        foreach (var cItem in itemsCopy)
        {
            playerInventory.AddItem(cItem.itemData, cItem.amount);
            container.items.Remove(cItem);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var clone in uiClones)
        {
            Destroy(clone);
        }
        uiClones.Clear();
        uiANims.Clear();

        if (currentContainer == null) return;

        foreach (var cItem in currentContainer.items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, contentParent);

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = cItem.itemData.icon;

            uiClones.Add(slot);

            Animator anim = slot.GetComponentInChildren<Animator>();
            uiANims.Add(anim);
        }

    }

    public void PlayPickupAnimations(float uidelay)
    {
        StartCoroutine(PlayPickupCoroutine(uidelay));
    }
    private IEnumerator PlayPickupCoroutine(float delay)
    {
        foreach (var anim in uiANims)
        {
            anim.Play("Pickup");
            audioSource.PlayOneShot(pickupSound);
            yield return new WaitForSeconds(delay);
        }
    }
}
