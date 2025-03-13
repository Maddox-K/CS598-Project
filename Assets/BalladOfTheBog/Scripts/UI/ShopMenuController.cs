using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField] public GameObject[] slots;
    public GameObject[] slotContents;
    private bool _isStocked = false;
    private ShopNPC _lastOpenShop = null;
    private Canvas _shopCanvas;

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

        GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;

        _playerData = player.GetComponent<PlayerData>();

        _inventoryController = player.GetComponent<InventoryController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool GetIsStocked()
    {
        return _isStocked;
    }

    public ShopNPC GetLastOpenShop()
    {
        return _lastOpenShop;
    }

    public void Shop(ShopNPC shopNPC)
    {
        _lastOpenShop = shopNPC;

        _shopCanvas.enabled = true;
    }
}
