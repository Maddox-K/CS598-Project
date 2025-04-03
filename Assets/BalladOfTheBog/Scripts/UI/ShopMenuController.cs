using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] _slots;
    private GameObject[] _slotContents = new GameObject[6];
    private TextMeshProUGUI[] _slotItemNames = new TextMeshProUGUI[6];
    public string[] currentShopDescriptions = new string[6];
    private bool _isStocked = false;
    private ShopNPC _lastOpenShop = null;
    private Canvas _shopCanvas;
    [SerializeField] private TextMeshProUGUI _description;
    private Vector2 targetSize = new Vector2(8, 8);

    // Buttons
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _confirmPurchase;
    [SerializeField] private Button _cancelPurchase;
    [SerializeField] private Button _okayButton;

    // Player
    private PlayerData _playerData;

    // Inventory
    private InventoryController _inventoryController;

    void Awake()
    {
        _shopCanvas = GetComponent<Canvas>();

        for (int i = 0; i < _slots.Length; i++)
        {
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
    }

    public TextMeshProUGUI GetDescription()
    {
        return _description;
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
            }

            _isStocked = true;
            _lastOpenShop = shopNPC;
        }


        _slots[0].GetComponent<Button>().Select();
        _description.text = currentShopDescriptions[0];

        gameObject.SetActive(true);
    }

    void FormatAndScale(GameObject item)
    {
        item.GetComponent<Button>().interactable = false;

        Image image = item.GetComponent<Image>();

        image.raycastTarget = false;

        // Get the sprite's original size in pixels
        Vector2 spriteSize = image.sprite.rect.size;

        // Calculate scale factors for width and height
        float scaleX = targetSize.x / spriteSize.x;
        float scaleY = targetSize.y / spriteSize.y;

        // Use the smaller scale factor to maintain aspect ratio
        float uniformScale = Mathf.Min(scaleX, scaleY);

        // Apply scale
        item.transform.localScale = new Vector3(uniformScale, uniformScale, 1);
    }
}
