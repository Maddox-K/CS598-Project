using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopMenuController : MonoBehaviour
{
    // General Menu Components
    [SerializeField] private GameObject[] _slots;
    private Button[] _slotButtons = new Button[6];
    private Canvas _shopCanvas;
    [SerializeField] private TextMeshProUGUI _descriptionBox;
    [SerializeField] private TextMeshProUGUI _priceBox;
    [SerializeField] private GameObject _brokePopUp;
    [SerializeField] private Button _brokeConfirm;
    private Vector2 targetSpriteSize = new Vector2(8, 8);
    [SerializeField] private Button _returnButton;

    // Current Shop Information
    private GameObject[] _slotContents = new GameObject[6];
    private TextMeshProUGUI[] _slotItemNames = new TextMeshProUGUI[6];
    public string[] currentShopDescriptions = new string[6];
    public string[] currentShopPrices = new string[6];
    private bool _isStocked = false;
    private ShopNPC _lastOpenShop = null;
    private int _lastAttemptedPurchase;
    
    // Player
    private PlayerData _playerData;

    // Inventory
    private InventoryController _inventoryController;

    void Awake()
    {
        _shopCanvas = GetComponent<Canvas>();

        for (int i = 0; i < _slots.Length; i++)
        {
            _slotButtons[i] = _slots[i].GetComponent<Button>();
            _slotItemNames[i] = _slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }


        GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;

        _playerData = player.GetComponent<PlayerData>();

        _inventoryController = player.GetComponent<InventoryController>();
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

        _brokeConfirm.onClick.AddListener(ClosePopup);
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
        ShopInventory inventory = shopNPC.inventory;

        // if shop menu is not initialized with this npc's shop items
        if (!_isStocked || _lastOpenShop != shopNPC)
        {
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

        if (_playerData.currency_count < price)
        {
            _lastAttemptedPurchase = slotNumber;

            for (int i = 0; i < _slotButtons.Length; i++)
            {
                _slotButtons[i].interactable = false;
            }
            _returnButton.interactable = false;

            _brokePopUp.SetActive(true);
            _brokeConfirm.Select();
        }
        else
        {
            Debug.Log("Buy");
        }
    }

    private void ClosePopup()
    {
        for (int i = 0; i < _slotButtons.Length; i++)
        {
            _slotButtons[i].interactable = true;
        }
        _returnButton.interactable = true;

        _brokePopUp.SetActive(false);

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
