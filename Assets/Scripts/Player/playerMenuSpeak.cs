using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomTypewriterText : MonoBehaviour
{
    public TMP_Text textMesh;

    public GameObject chatBoxUI;

    [TextArea]
    public List<string> textList = new List<string>();

    public float typeSpeed = 0.05f;
    public float displayDuration = 2f;
    public float cooldown = 3f;
    public float chatBoxDelay = 0.5f;

    public AudioSource audioSource;
    public AudioClip typeSound;
    public bool skipSpaces = true;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private void Start()
    {
        textMesh.text = "";
        chatBoxUI.SetActive(false);

        StartCoroutine(TextLoop());
    }

    IEnumerator TextLoop()
    {
        // Initial cooldown
        yield return new WaitForSeconds(cooldown);

        while (true)
        {
            // Show chat box
            chatBoxUI.SetActive(true);

            // Delay before typing
            yield return new WaitForSeconds(chatBoxDelay);

            // Pick random text
            string selectedText = textList[Random.Range(0, textList.Count)];

            // Typewriter effect
            yield return StartCoroutine(TypeText(selectedText));

            // Keep text visible
            yield return new WaitForSeconds(displayDuration);

            // Clear and hide
            textMesh.text = "";
            chatBoxUI.SetActive(false);

            // Cooldown before next message
            yield return new WaitForSeconds(cooldown);
        }
    }

    IEnumerator TypeText(string text)
    {
        textMesh.text = "";

        foreach (char c in text)
        {
            textMesh.text += c;

            // Play typing sound
            if (!skipSpaces || !char.IsWhiteSpace(c))
            {
                audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
                audioSource.PlayOneShot(typeSound);
            }

            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
