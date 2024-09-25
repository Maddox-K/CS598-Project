using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private PlayerController pcontroller;

    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private string p;

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
            else
            {
                // end conversation
                EndConversation();
                return;
            }
        }

        // if something in the queue
        p = paragraphs.Dequeue();

        //update ConversationText
        NPCDialogueText.text = p;

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
}
