using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectQuest", menuName = "Quests/Collect Quest")]
public class CollectQuestSO : QuestSO
{
    public string requiredItemID;

    public int requiredAmount = 1;

    public string turnInNPC_ID;
    public string itemName;


    [HideInInspector]
    public int currentAmount = 0;

    private Inventory playerInventory;

    public override void Initialize()
    {
        base.Initialize();
        currentAmount = 0;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
        }

        if (playerInventory != null)
        {
            currentAmount = playerInventory.GetItemAmount(requiredItemID);
        }
    }

    public override void ResetQuest()
    {
        base.ResetQuest();
        currentAmount = 0;
    }

    public void ItemCollected(string itemID, int amount)
    {
        if (itemID.ToLower() == requiredItemID.ToLower())
        {
            currentAmount += amount;
            currentAmount = Mathf.Min(currentAmount, requiredAmount);
        }
    }

    public override bool CheckCompletion()
    {
        return currentAmount >= requiredAmount;
    }
}
