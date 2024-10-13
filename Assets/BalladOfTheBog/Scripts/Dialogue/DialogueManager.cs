using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10;
    [SerializeField] private PlayerController pcontroller;
    [SerializeField] private Button[] choiceButtons;

    public Queue<string> paragraphs = new Queue<string>();
    public bool conversationEnded;
    private bool isTyping;
    private bool waitingForInput = false;
    private string p;
    private Coroutine typeDialogueCoroutine;

    private const float MAX_TYPE_TIME = 0.1f;

    public void DisplayNext(Dialogue dialogue)
    {
        //Debug.Log("Starting conversation with " + dialogue.name);
        // if nothing in the queue
        if (waitingForInput == true)
        {
            return;
        }

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
            if (p == "[choose]")
            {
                MakeDialogueChoice(dialogue);
                //p = paragraphs.Dequeue();
            }
            else
            {
                Debug.Log(p);
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
            }
        }

        else
        {
            FinishParagraphEarly();
        }
        //p = paragraphs.Dequeue();

        //update ConversationText
        //NPCDialogueText.text = p;

        if (paragraphs.Count == 0 && waitingForInput == false)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(Dialogue dialogue)
    {
        pcontroller.move.Disable();

        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

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
        if (pcontroller.interact.enabled == false)
        {
            pcontroller.interact.Enable();
        }

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

    private void MakeDialogueChoice(Dialogue dialogue)
    {
        pcontroller.interact.Disable();

        if (dialogue.choices != null && dialogue.choices.Length > 0)
        {
            waitingForInput = true;

            for (int i = 0; i < dialogue.choices.Length; i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = dialogue.choices[i].choiceText;

                choiceButtons[i].onClick.RemoveAllListeners();

                int choiceIndex = i;
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(dialogue, choiceIndex));
            }
        }
    }

    private void OnChoiceSelected(Dialogue dialogue, int choiceIndex)
    {
        Dialogue nextDialogue = dialogue.choices[choiceIndex].nextDialogue;
        paragraphs.Clear();

        /* for (int i = 0; i < nextDialogue.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(nextDialogue.paragraphs[i]);
        } */

        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        waitingForInput = false;

        DisplayNext(nextDialogue);
    }
}
