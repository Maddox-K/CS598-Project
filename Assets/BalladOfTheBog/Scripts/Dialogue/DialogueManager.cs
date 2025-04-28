using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // General dialogue variables
    public Queue<string> paragraphs = new Queue<string>();
    public bool conversationEnded;
    private bool _isTyping;
    private string _currentParagraph;
    private Coroutine typeDialogueCoroutine;
    private const float maxTypeTime = 0.1f;
    private const float typeSpeed = 10.0f;

    // references to other gameobjects necessary for dialogue to function
    [SerializeField] private TextMeshProUGUI _NPCNameText;
    [SerializeField] private TextMeshProUGUI _NPCDialogueText;

    // choice dialogue
    [SerializeField] private Button[] choiceButtons;
    private GameObject[] _choiceIndicators;
    private bool _waitingForInput = false;

    // audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _dialogueTypeSound;
    [SerializeField] private bool _stopAudioSource;
    [SerializeField] private int _frequencyLevel;
    [Range(-3, 3)]
    [SerializeField] private float _minPitch = 0.8f;
    [Range(-3, 3)]
    [SerializeField] private float _maxPitch = 1.2f;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();

        _choiceIndicators = new GameObject[choiceButtons.Length];

        for (int i = 0; i < _choiceIndicators.Length; i++)
        {
            _choiceIndicators[i] = choiceButtons[i].transform.GetChild(0).gameObject;
        }
    }

    public void DisplayNext(Dialogue dialogue)
    {
        // if nothing in the queue
        if (_waitingForInput)
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
            else if (conversationEnded && !_isTyping)
            {
                // end conversation
                EndConversation();
                return;
            }
        }

        // if something in the queue
        if (!_isTyping)
        {
            _currentParagraph = paragraphs.Dequeue();
            if (_currentParagraph == "[choose]")
            {
                MakeDialogueChoice(dialogue);
            }
            else
            {
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(_currentParagraph));
            }
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0 && !_waitingForInput)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(Dialogue dialogue)
    {
        PlayerEvents.InvokeDeactivate(1);
        PauseEvents.InvokeDisablePopup(2);
        
        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _NPCNameText.text = dialogue.speaker_name;

        for (int i = 0; i < dialogue.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogue.paragraphs[i]);
        }
    }

    public void EndConversation()
    {
        PlayerEvents.InvokeActivate(1);
        PauseEvents.InvokeEnablePopup(2);
        
        paragraphs.Clear();

        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeDialogueText(string p)
    {
        PlayerEvents.InvokeActivate(0);

        _isTyping = true;

        int maxVisibleChars = 0;

        _NPCDialogueText.text = p;
        _NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

        foreach(char c in p.ToCharArray())
        {
            PlayDialogueSound(_NPCDialogueText.maxVisibleCharacters);

            maxVisibleChars++;
            _NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSeconds(maxTypeTime / typeSpeed);
        }

        _isTyping = false;
    }

    private void PlayDialogueSound(int currentDisplayedCharCount)
    {
        if (currentDisplayedCharCount % _frequencyLevel == 0)
        {
            if (_stopAudioSource)
            {
                _audioSource.Stop();
            }
            _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
            _audioSource.PlayOneShot(_dialogueTypeSound);
        }
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);

        _NPCDialogueText.maxVisibleCharacters = _currentParagraph.Length;

        _isTyping = false;
    }

    private void MakeDialogueChoice(Dialogue dialogue)
    {
        PlayerEvents.InvokeDeactivate(0);

        if (dialogue.choices != null && dialogue.choices.Length > 0)
        {
            _waitingForInput = true;

            for (int i = 0; i < dialogue.choices.Length; i++)
            {
                _choiceIndicators[i].SetActive(false);

                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = dialogue.choices[i].choiceText;

                choiceButtons[i].onClick.RemoveAllListeners();

                int choiceIndex = i;
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(dialogue, choiceIndex));
            }
        }

        choiceButtons[0].Select();
        _choiceIndicators[0].SetActive(true);
    }

    private void OnChoiceSelected(Dialogue dialogue, int choiceIndex)
    {
        Dialogue nextDialogue = dialogue.choices[choiceIndex].nextDialogue;
        paragraphs.Clear();

        foreach (Button button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }

        _waitingForInput = false;

        DisplayNext(nextDialogue);
    }
}
