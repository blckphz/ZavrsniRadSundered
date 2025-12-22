using UnityEngine;
using TMPro;
using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static event Action<string> OnQuestTagFound;
    public static event Action<string> OnSpawnQuestObject;
    public static event Action OnDialogueEnded;

    public static event Action<string> OnAggroTagFound;

    public TextAsset globalInkFile;

    public TMP_Text dialogueText;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private TMP_Text[] choiceButtons;

    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private AudioSource typewriterSFX;

    [SerializeField] private float minTypeSoundInterval = 0.1f;
    [SerializeField] private float maxTypeSoundInterval = 0.3f;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    private float nextTypeSoundTime;

    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private PlayerMovement playerMove;
    [SerializeField] private attackhudcontroller hudController;
    [SerializeField] private GameObject dialogueBackgroundUI;

    [SerializeField] private GameObject disableDuringDialogue;

    private static Story story;
    public static bool dialogueIsRunning = false;
    private string currentLineText = "";
    private Coroutine typingCoroutine;

    private static DialogueManager Instance;

    void Awake()
    {
        Instance = this;

        if (story == null && globalInkFile != null)
        {
            story = new Story(globalInkFile.text);
        }
    }

    void Start()
    {
        dialogueText.text = "";
        dialogueText.gameObject.SetActive(false);
        if (choicesContainer != null) choicesContainer.SetActive(false);

        if (playerAbilities == null)
            playerAbilities = FindAnyObjectByType<PlayerAbilities>();

        if (hudController == null)
            hudController = FindAnyObjectByType<attackhudcontroller>();

        if (dialogueBackgroundUI == null)
            dialogueBackgroundUI = GameObject.Find("DialogueBackground");

        if (playerMove == null)
            playerMove = FindAnyObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (dialogueIsRunning && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            if (IsAwaitingChoice()) return;
            DisplayNextLine();
        }

        if (playerAbilities == null)
            playerAbilities = FindAnyObjectByType<PlayerAbilities>();
        if (hudController == null)
            hudController = FindAnyObjectByType<attackhudcontroller>();
        if (playerMove == null)
            playerMove = FindAnyObjectByType<PlayerMovement>();
    }

    public void StartDialogue(string knotName)
    {
        if (story == null) return;

        try { story.ChoosePathString(knotName); }
        catch (Exception e)
        {
            Debug.LogError("knot not found: " + knotName);
            return;
        }

        dialogueIsRunning = true;
        dialogueText.gameObject.SetActive(true);
        choicesContainer.SetActive(false);
        currentLineText = "";

        playerAbilities.enabled = false;
        hudController.OnDialogueStarted();
        dialogueBackgroundUI.SetActive(true);
        playerMove.enabled = false;

        disableDuringDialogue.SetActive(false);

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (story == null || !dialogueIsRunning) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = currentLineText;
            return;
        }

        if (!story.canContinue && story.currentChoices.Count > 0)
        {
            HandleChoices();
            return;
        }

        if (story.canContinue)
        {
            currentLineText = story.Continue().Trim();
            HandleTags(story.currentTags);
            typingCoroutine = StartCoroutine(TypewriterEffect(currentLineText));
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypewriterEffect(string line)
    {
        dialogueText.text = "";
        nextTypeSoundTime = Time.time + UnityEngine.Random.Range(minTypeSoundInterval, maxTypeSoundInterval);

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];

            if (typewriterSFX != null && Time.time >= nextTypeSoundTime)
            {
                typewriterSFX.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                typewriterSFX.Play();
                nextTypeSoundTime = Time.time + UnityEngine.Random.Range(minTypeSoundInterval, maxTypeSoundInterval);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;

        if (!story.canContinue && story.currentChoices.Count > 0)
            HandleChoices();
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = "";
        dialogueText.gameObject.SetActive(false);
        choicesContainer.SetActive(false);
        dialogueIsRunning = false;
        currentLineText = "";

        playerAbilities.enabled = true;
        hudController.OnDialogueEnded();
        dialogueBackgroundUI.SetActive(false);
        playerMove.enabled = true;

        disableDuringDialogue.SetActive(true);

        OnDialogueEnded?.Invoke();
    }

    public void ResetStoryState()
    {
        if (story != null && globalInkFile != null)
        {
            story = new Story(globalInkFile.text);
        }
    }

    public bool IsAwaitingChoice()
    {
        return dialogueIsRunning &&
               story != null &&
               story.currentChoices.Count > 0 &&
               choicesContainer.activeInHierarchy;
    }

    private void HandleChoices()
    {
        choicesContainer.SetActive(true);

        foreach (var btn in choiceButtons)
            btn.transform.gameObject.SetActive(false);

        int safeCount = Mathf.Min(story.currentChoices.Count, choiceButtons.Length);

        for (int i = 0; i < safeCount; i++)
        {
            choiceButtons[i].text = story.currentChoices[i].text;
            choiceButtons[i].transform.gameObject.SetActive(true);
        }
    }

    public void ChooseChoice(int choiceIndex)
    {
        if (story == null || choiceIndex >= story.currentChoices.Count) return;

        story.ChooseChoiceIndex(choiceIndex);
        choicesContainer.SetActive(false);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        currentLineText = "";

        if (story.canContinue)
            story.Continue();

        DisplayNextLine();
    }

    private void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            string tagLower = tag.ToLower();
            if (tagLower.StartsWith("startquest"))
            {
                string questId = tag.Split(':')[1].Trim();
                OnQuestTagFound?.Invoke(questId);
            }
            else if (tagLower.StartsWith("spawnquestobject"))
            {
                string objectId = tag.Split(':')[1].Trim();
                OnSpawnQuestObject?.Invoke(objectId);
            }
            else if (tagLower == "aggro")
            {
                OnAggroTagFound?.Invoke("aggro");
            }
        }
    }

    public bool IsDialogueRunning()
    {
        return dialogueIsRunning;
    }

    public static string GetStoryState()
    {
        if (story != null)
            return story.state.ToJson();
        return "";
    }

    public static void SetStoryState(string jsonState)
    {
        if (story != null && jsonState != null && jsonState != "")
        {
            try
            {
                story.state.LoadJson(jsonState);
                story.Continue();
            }
            catch (Exception e)
            {
                Debug.LogError("ink state load failed");
                if (Instance != null && Instance.globalInkFile != null)
                {
                    story.state.LoadJson(
                        new Story(Instance.globalInkFile.text).state.ToJson()
                    );
                }
            }
        }
    }
}
