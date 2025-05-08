using UnityEngine;
using UnityEngine.Events;

public class UnlockableDoor : Door, IDataPersistence
{
    private InventoryController _inventoryController;

    // Locking Fields
    [SerializeField] private string _doorId;
    private bool _isUnlocked;
    [SerializeField] private string _itemNeededToUnlock;

    //Sound
    [SerializeField] private AudioClip _lockedClip;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        _doorId = System.Guid.NewGuid().ToString();
    }

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();

        _inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
    }

    void OnEnable()
    {
        QuestEvents.OnItemCollected += OnItemCollected;
    }

    void OnDisable()
    {
        QuestEvents.OnItemCollected -= OnItemCollected;
    }

    private void OnItemCollected(string id)
    {
        if (id == _itemNeededToUnlock)
        {
            _isUnlocked = true;
        }
    }

    public override void Interact()
    {
        if (!_canOpen)
        {
            return;
        }

        if (_isUnlocked)
        {
            if (_audioSource != null && _doorOpenSound != null)
            {
                _audioSource.volume = 0.09f;
                _audioSource.PlayOneShot(_doorOpenSound);
            }

            if (_transitionAnimator != null)
            {
                _transitionAnimator.SetTrigger("Start");
            }

            musicChange.Invoke();

            StartCoroutine(SetCanOpen());

            PlayerEvents.InvokeDoorOpen(this);
        }
        else if (_inventoryController.CheckItemInInventory(_itemNeededToUnlock))
        {
            _isUnlocked = true;
            Interact();
        }
        else
        {
            if (_audioSource != null && _lockedClip != null)
            {
                _audioSource.volume = 1.0f;
                _audioSource.PlayOneShot(_lockedClip);
            }
            PlayerEvents.InvokeFailedLockedDoor();
        }
    }

    public void LoadData(GameData data)
    {
        data.doorsUnlocked.TryGetValue(_doorId, out _isUnlocked);
    }

    public void SaveData(GameData data)
    {
        if (data.doorsUnlocked.ContainsKey(_doorId))
        {
            data.doorsUnlocked.Remove(_doorId);
        }
        data.doorsUnlocked.Add(_doorId, _isUnlocked);
    }
}
