using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Mail : NPC
{
    // Button
    [SerializeField] private Button _mailButton;

    // Inventory
    private InventoryController _inventoryController;

    // Mail Variables
    [SerializeField] private string _mailName;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionClip;
    private InputSystemUIInputModule _inputModule;
    private InputAction _navigateAction;
    private InputAction _pointAction;
    [SerializeField] private Dialogue _letterDialogue;
    [SerializeField] private float _timeToSelfDestruct;

    void Awake()
    {
        dialogueManager = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject.GetComponent<DialogueManager>();

        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        _mailButton.onClick.AddListener(Interact);
        _inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();

        _inputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        _navigateAction = _inputModule.actionsAsset.FindAction("Navigate");
        _pointAction = _inputModule.actionsAsset.FindAction("Point");
    }

    public override void Interact()
    {
        if (_navigateAction.enabled)
        {
            _navigateAction.Disable();
            _pointAction.Disable();
    
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            _mailButton.Select();
        }

        if (dialogueManager.conversationEnded)
        {
            dialogueManager.DisplayNext(_letterDialogue);
            
            PlayerEvents.InvokeDeactivate(1);
            PauseEvents.InvokeDisablePopup(2);

            _mailButton.onClick.RemoveListener(Interact);

            StartCoroutine(SelfDestruct(_timeToSelfDestruct));
        }
        else
        {
            dialogueManager.DisplayNext(_letterDialogue);
        }
    }

    private IEnumerator SelfDestruct(float time)
    {
        yield return new WaitForSeconds(time);

        int slotIndex = transform.parent.GetSiblingIndex();
        _inventoryController.slotButtons[slotIndex].interactable = true;
        _inventoryController.RemoveItem(slotIndex);

        _audioSource.PlayOneShot(_explosionClip);

        yield return new WaitForSeconds(1f);

        if (dialogueManager.gameObject.activeSelf)
        {
            dialogueManager.paragraphs.Clear();
            dialogueManager.gameObject.SetActive(false);
            dialogueManager.conversationEnded = false;
        }

        PauseEvents.InvokeEnablePopup(2);

        _navigateAction.Enable();
        _pointAction.Enable();

        QuestEvents.OnItemUsed?.Invoke(_mailName);

        Destroy(gameObject);
    }
}