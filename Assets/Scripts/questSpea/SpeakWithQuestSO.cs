using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeakQuest", menuName = "Quests/Speak With Quest")]
public class SpeakWithQuestSO : QuestSO
{
    public string targetNPC_ID = "TargetNPC";

    public bool hasSpokenToTarget = false;


    public override bool CheckCompletion()
    {
        return hasSpokenToTarget;
    }

    public void MarkTargetSpoken()
    {
        hasSpokenToTarget = true;
    }
}
