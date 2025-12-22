using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class NPC_DialogueTrigger : MonoBehaviour
{
    [System.Serializable]
    public class DialogueEntry
    {
        public string inkKnotName;
    }

    public string npcID = "Unnamed_NPC";

    public DialogueEntry[] dialoguePhases;

    public int currentDialogueIndex;
    public bool isNewGameAutoStarter = false;
    public bool ignorePlayerPrefsForTesting = false;

    [SerializeField] private TMP_Text dialoguePromptText;
    [SerializeField] private Image speechbubble;
    public string playerTag = "Player";

    [SerializeField] private MonoBehaviour aggroCheckComponent;
    private IAggroCheck aggroCheck;

    private QuestManager questManager;
    private DialogueManager dialogueManager;

    private bool playerIsInRange = false;

    private string autostart = "NPC_AutoStart_";
    private string index = "NPC_DialogueIndex_";


    void Awake()
    {
        if (aggroCheckComponent == null)
            aggroCheckComponent = GetComponent<MonoBehaviour>();

        aggroCheck = aggroCheckComponent as IAggroCheck;
        if (aggroCheck == null)
            aggroCheck = GetComponent<IAggroCheck>();
    }

    void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        questManager = FindFirstObjectByType<QuestManager>();

        if (ignorePlayerPrefsForTesting)
            ResetMemory();
        else
            LoadDialogueState();

        CheckAutoStart();
    }

    void OnEnable()
    {
        QuestManager.OnQuestCompleted += OnQuestFinished;
        DialogueManager.OnDialogueEnded += OnDialogueEnded;
    }

    void OnDisable()
    {
        QuestManager.OnQuestCompleted -= OnQuestFinished;
        DialogueManager.OnDialogueEnded -= OnDialogueEnded;
    }

    void Update()
    {
        if (!playerIsInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (aggroCheck.IsAggroed)
                return;

            if (!dialogueManager.IsDialogueRunning())
            {
                StartNPCDialogue(false);
            }
        }
    }


    private void StartNPCDialogue(bool isAutoStart)
    {
        questManager.NotifySpokenToNPC(npcID);
        questManager.TurnInCompletedQuestsAtNPC(npcID);


        if (dialoguePhases == null || dialoguePhases.Length == 0) return;
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialoguePhases.Length)
        {
            return;
        }

        string knotToRun = dialoguePhases[currentDialogueIndex].inkKnotName;

        speechbubble.gameObject.SetActive(true);
        dialoguePromptText.gameObject.SetActive(true);

        dialogueManager.dialogueText = dialoguePromptText;

        dialogueManager.StartDialogue(knotToRun);
    }

    private void OnDialogueEnded()
    {
        StartCoroutine(ResetDialogueJustEndedFlag());

        questManager.NotifySpokenToNPC(npcID);

        dialoguePromptText.text = "";
        speechbubble.gameObject.SetActive(false);

        SetPromptVisibility(false);

        if (playerIsInRange && !aggroCheck.IsAggroed)
        {
            speechbubble.gameObject.SetActive(true);
            SetPromptVisibility(true);
        }
    }

    private IEnumerator ResetDialogueJustEndedFlag()
    {
        yield return null;
    }


    private void OnQuestFinished(string completedQuestID, string relatedNPC_ID, int nextDialogueIndex)
    {
        if (relatedNPC_ID != npcID) return;

        currentDialogueIndex = nextDialogueIndex;
        SaveDialogueState();

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (aggroCheck.IsAggroed)
            return;

        playerIsInRange = true;

        speechbubble.gameObject.SetActive(true);

        if (!dialogueManager.IsDialogueRunning())
            SetPromptVisibility(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerIsInRange = false;
        SetPromptVisibility(false);

        speechbubble.gameObject.SetActive(false);

        if (dialogueManager.IsDialogueRunning())
            dialogueManager.EndDialogue();
    }

    private void SetPromptVisibility(bool show)
    {
        dialoguePromptText.gameObject.SetActive(show);
    }

    private void LoadDialogueState()
    {
        string key = index + npcID;
        currentDialogueIndex = PlayerPrefs.GetInt(key, currentDialogueIndex);
    }

    private void SaveDialogueState()
    {
        string key = index + npcID;
        PlayerPrefs.SetInt(key, currentDialogueIndex);
        PlayerPrefs.Save();
    }

    private void ResetMemory()
    {
        PlayerPrefs.DeleteKey(index + npcID);
        PlayerPrefs.DeleteKey(autostart + npcID);
        PlayerPrefs.Save();
        currentDialogueIndex = 0;
    }

    private void CheckAutoStart()
    {
        if (!isNewGameAutoStarter) return;
        if (currentDialogueIndex > 0) return;

        string key = autostart + npcID;
        bool alreadyRun = PlayerPrefs.GetInt(key, 0) == 1;

        if (!alreadyRun || ignorePlayerPrefsForTesting)
        {
            StartNPCDialogue(true);

            if (!ignorePlayerPrefsForTesting)
            {
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefs.Save();
            }
        }
    }
}