using UnityEngine;

[CreateAssetMenu(fileName = "NewKillQuest", menuName = "Quests/Kill Quest")]
public class KillQuestSO : QuestSO
{
    public string requiredEnemyID = "Enemy_ID";
    public string EnemyTextName = "Enemy_ID";
    public int requiredKills = 10;

    public int currentKills = 0;

    public string turnInNPC_ID = "";


    public override void Initialize()
    {
        base.Initialize();
        currentKills = 0;
    }


    public override void ResetQuest()
    {
        base.ResetQuest();
        currentKills = 0;
    }

    public override bool CheckCompletion()
    {
        return currentKills >= requiredKills;
    }

    public void EnemyKilled(string enemyID, int amount = 1)
    {
        if (enemyID.ToLower() != requiredEnemyID.ToLower())
        {
            return;
        }

        if (CheckCompletion())
        {
            return;
        }

        currentKills += amount;
        currentKills = Mathf.Min(currentKills, requiredKills);

        if (CheckCompletion())
        {
            MarkComplete();
        }
    }

}
