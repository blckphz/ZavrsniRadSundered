using UnityEngine;

public class uiInfoSender : MonoBehaviour
{
    public Container citem;


    public void isOPened()
    {
        citem.AutoLoot(citem.playerInventory);
    }
}
