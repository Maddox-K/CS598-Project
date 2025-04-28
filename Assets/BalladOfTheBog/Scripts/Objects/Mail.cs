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
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionClip;
    private InputSystemUIInputModule _inputModule;
    private InputAction _navigateAction;
    private InputAction _pointAction;
    [SerializeField] private Dialogue _letterDialogue;
    private int _shownText = 0;
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
            //Cursor.lockState = CursorLockMode.Locked;
    
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
            _mailButton.Select();
        }

        if (_shownText == _letterDialogue.paragraphs.Length - 1)
        {
            _mailButton.onClick.RemoveListener(Interact);
            _navigateAction.Enable();
            _pointAction.Enable();
            //Cursor.lockState = CursorLockMode.None;
            PauseEvents.InvokeEnablePopup(2);
            dialogueManager.paragraphs.Clear();
            dialogueManager.gameObject.SetActive(false);

            StartCoroutine(SelfDestruct(_timeToSelfDestruct));
        }
        else
        {
            _shownText += 1;
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

        Destroy(gameObject); // Destroy the tomato object
    }
}