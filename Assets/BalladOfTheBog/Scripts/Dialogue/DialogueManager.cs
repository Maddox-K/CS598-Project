using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10;
    [SerializeField] private PlayerController pcontroller;

    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private bool isTyping;
    private string p;
    private Coroutine typeDialogueCoroutine;

    private const float MAX_TYPE_TIME = 0.1f;

    public void DisplayNext(Dialogue dialogue)
    {
        //Debug.Log("Starting conversation with " + dialogue.name);
        // if nothing in the queue
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                // start conversation
                StartConversation(dialogue);
            }
            else if (conversationEnded && !isTyping)
            {
                // end conversation
                EndConversation();
                return;
            }
        }

        // if something in the queue
        if (!isTyping)
        {
            p = paragraphs.Dequeue();

            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }

        else
        {
            FinishParagraphEarly();
        }
        //p = paragraphs.Dequeue();

        //update ConversationText
        //NPCDialogueText.text = p;

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(Dialogue dialogue)
    {
        pcontroller.move.Disable();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        NPCNameText.text = dialogue.speaker_name;

        for (int i = 0; i < dialogue.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogue.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        pcontroller.move.Enable();

        paragraphs.Clear();

        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        int maxVisibleChars = 0;

        NPCDialogueText.text = p;
        NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

        foreach(char c in p.ToCharArray())
        {
            maxVisibleChars++;

            NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);

        NPCDialogueText.maxVisibleCharacters = p.Length;

        isTyping = false;
    }
}
