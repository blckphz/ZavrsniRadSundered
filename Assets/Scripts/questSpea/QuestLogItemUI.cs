using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestLogItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questTitleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;

    public void Setup(QuestSO quest)
    {
        questTitleText.text = quest.questName;
        descriptionText.text = quest.description;

        string progressInfo = "In Progress...";

        if (quest is CollectQuestSO collectQuest)
        {
            progressInfo = "Collect: " + collectQuest.currentAmount + "/" + collectQuest.requiredAmount + " " + collectQuest.itemName + "s";
        }
        else if (quest is KillQuestSO killQuest)
        {
            progressInfo = "Kill: " + killQuest.currentKills + "/" + killQuest.requiredKills + " " + killQuest.EnemyTextName + "s";
        }
        else if (quest is SpeakWithQuestSO speakQuest)
        {
            if (speakQuest.hasSpokenToTarget)
                progressInfo = "Target spoken to! (Complete)";
            else
                progressInfo = "Speak with " + speakQuest.targetNPC_ID;
        }

        progressText.text = progressInfo;
    }
}
