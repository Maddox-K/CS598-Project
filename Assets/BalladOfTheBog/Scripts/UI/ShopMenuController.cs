using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopMenuController : MonoBehaviour
{
    // General Menu Components
    [SerializeField] private GameObject[] _slots;
    private Button[] _slotButtons = new Button[6];
    private TextMeshProUGUI[] _slotItemNames = new TextMeshProUGUI[6];
    private Canvas _shopCanvas;
    [SerializeField] private TextMeshProUGUI _descriptionBox;
    [SerializeField] private TextMeshProUGUI _priceBox;
    private Vector2 targetSpriteSize = new Vector2(8, 8);
    [SerializeField] private Button _returnButton;

    // Current Shop Information
    private GameObject[] _slotContents = new GameObject[6];
    public string[] currentShopDescriptions = new string[6];
    public string[] currentShopPrices = new string[6];
    private bool _isStocked = false;
    private ShopNPC _lastOpenShop = null;
    private int _lastAttemptedPurchase;
    private bool _purchasedSomething = false;

    // popup menu
    [SerializeField] private GameObject _popUp;
    [SerializeField] private Button _popupConfirm;
    [SerializeField] private TextMeshProUGUI _popupConfirmText;
    private readonly string[] _messages = {"You don't have enough Frog Coins.", "Your inventory is full."};
    
    // Player
    private GameObject _player;
    private PlayerData _playerData;

    // Inventory
    private InventoryController _inventoryController;

    // Audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _purchaseClip;

    void Awake()
    {
        _shopCanvas = GetComponent<Canvas>();

        for (int i = 0; i < _slots.Length; i++)
        {
            _slotButtons[i] = _slots[i].GetComponent<Button>();
            _slotItemNames[i] = _slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }


        _player = GameObject.FindGameObjectWithTag("Player").gameObject;

        _playerData = _player.GetComponent<PlayerData>();

        _inventoryController = _player.GetComponent<InventoryController>();

        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        _shopCanvas.enabled = true;

        for (int i = 0; i < _slots.Length; i++)
        {
            int index = i;
            _slotButtons[i].onClick.AddListener(() => Buy(index));
        }

        _popupConfirm.onClick.AddListener(ClosePopup);

        _returnButton.onClick.AddListener(EndShopping);
    }

    private void EndShopping()
    {
        _lastAttemptedPurchase = 0;

        gameObject.SetActive(false);

        _returnButton.transform.GetChild(0).gameObject.SetActive(false);

        bool purchased = _purchasedSomething;
        _purchasedSomething = false;

        PlayerEvents.InvokeActivate(0);

        _lastOpenShop.EndInteraction(purchased);
    }

    public TextMeshProUGUI GetDescriptionBox()
    {
        return _descriptionBox;
    }

    public TextMeshProUGUI GetPriceBox()
    {
        return _priceBox;
    }

    public void Shop(ShopNPC shopNPC)
    {
        PlayerEvents.InvokeDeactivate(0);

        ShopInventory inventory = shopNPC.inventory;

        // if shop menu is not initialized with this npc's shop items
        if (!_isStocked || _lastOpenShop != shopNPC)
        {
            currentShopDescriptions = new string[6];
            currentShopPrices = new string[6];

            // initialize shop with items that this npc has to offer
            for (int i = 0; i < inventory.shopSlots.Length; i++)
            {
                GameObject item = Instantiate(inventory.shopSlots[i], _slots[i].transform);
                _slotItemNames[i].text = inventory.names[i];
                FormatAndScale(item);
                _slotContents[i] = inventory.shopSlots[i];
                currentShopDescriptions[i] = inventory.descriptions[i];
                currentShopPrices[i] = inventory.prices[i].ToString();
            }

            _isStocked = true;
            _lastOpenShop = shopNPC;
        }


        _slotButtons[0].GetComponent<Button>().Select();
        _descriptionBox.text = currentShopDescriptions[0];
        _priceBox.text = "x" + currentShopPrices[0];

        gameObject.SetActive(true);
    }

    private void Buy(int slotNumber)
    {
        if (_slotContents[slotNumber] == null)
        {
            return;
        }

        int price = _lastOpenShop.inventory.prices[slotNumber];


        if (_playerData.currency_count < price) // can't affort item
        {
            _lastAttemptedPurchase = slotNumber;

            ShowPopupMenu(false);
        }
        else if (_inventoryController.CheckIfFull()) // inventory is full
        {
            _lastAttemptedPurchase = slotNumber;

            ShowPopupMenu(true);
        }
        else
        {
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_purchaseClip);
            }

            _playerData.DecreaseCurrency(price);

            _purchasedSomething = true;

            GameObject item = _lastOpenShop.inventory.shopSlots[slotNumber];

            for (int i = 0; i < _inventoryController.slots.Length; i++)
            {
                if (!_inventoryController.isFulll[i])
                {
                    _inventoryController.isFulll[i] = true;
                    Instantiate(item, _inventoryController.slots[i].transform);
                    break;
                }
            }
        }
    }

    private void ShowPopupMenu(bool messageIndex)
    {
        for (int i = 0; i < _slotButtons.Length; i++)
        {
            _slotButtons[i].interactable = false;
        }
        _returnButton.interactable = false;

        if (!messageIndex)
        {
            _popupConfirmText.text = _messages[0];
        }
        else
        {
            _popupConfirmText.text = _messages[1];
        }

        _popUp.SetActive(true);
        _popupConfirm.Select();
    }

    private void ClosePopup()
    {
        for (int i = 0; i < _slotButtons.Length; i++)
        {
            _slotButtons[i].interactable = true;
        }
        _returnButton.interactable = true;

        _popUp.SetActive(false);

        _slotButtons[_lastAttemptedPurchase].Select();

        _lastAttemptedPurchase = 0;
    }

    void FormatAndScale(GameObject item)
    {
        item.GetComponent<Button>().interactable = false;

        Image image = item.GetComponent<Image>();

        image.raycastTarget = false;

        // Get the sprite's original size in pixels
        Vector2 spriteSize = image.sprite.rect.size;

        // Calculate scale factors for width and height
        float scaleX = targetSpriteSize.x / spriteSize.x;
        float scaleY = targetSpriteSize.y / spriteSize.y;

        // Use the smaller scale factor to maintain aspect ratio
        float uniformScale = Mathf.Min(scaleX, scaleY);

        // Apply scale
        item.transform.localScale = new Vector3(uniformScale, uniformScale, 1);
    }
}
