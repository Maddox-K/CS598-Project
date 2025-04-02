using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopNPC : NPC
{
    // Menu
    private ShopMenuController _shopMenu;
    private Vector2 targetSize = new Vector2(8, 8);

    // NPC-specific Shop and Interaction Info
    [SerializeField] private ShopInventory _inventory;
    [SerializeField] private Dialogue _preShopDialogue;
    [SerializeField] private Dialogue _postShopDialogue;
    private TextMeshProUGUI[] _slotNames = new TextMeshProUGUI[6];

    void Awake()
    {
        _shopMenu = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<ShopMenuController>();

        for (int i = 0; i < _inventory.shopSlots.Length; i++)
        {
            _slotNames[i] = _shopMenu.slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }

    public override void Interact()
    {
        if (_preShopDialogue == null || (dialogueManager.paragraphs.Count > 0 && dialogueManager.paragraphs.Peek() == "[shop]"))
        {
            // if shop menu is not initialized with this npc's shop items
            if (_shopMenu != null && (!_shopMenu.GetIsStocked() || _shopMenu.GetLastOpenShop() != this))
            {
                // initialize shop with items that this npc has to offer
                for (int i = 0; i < _inventory.shopSlots.Length; i++)
                {
                    GameObject item = Instantiate(_inventory.shopSlots[i], _shopMenu.slots[i].transform);
                    _slotNames[i].text = _inventory.names[i];
                    FormatAndScale(item);
                    _shopMenu.slotContents[i] = _inventory.shopSlots[i];
                }
            }
            // initiate shopping
            dialogueManager.gameObject.SetActive(false);
            _shopMenu.Shop(this);
        }
        else
        {
            dialogueManager.DisplayNext(_preShopDialogue);
        }
    }

    // function that takes in a boolean (yes or no bought something) and ends interaction
    private void EndInteraction()
    {

    }

    void FormatAndScale(GameObject item)
    {
        item.GetComponent<Button>().interactable = false;

        Image image = item.GetComponent<Image>();
        if (image == null || image.sprite == null) return;

        // Get the sprite's original size in pixels
        Vector2 spriteSize = image.sprite.rect.size;
        Debug.Log(spriteSize);

        // Calculate scale factors for width and height
        float scaleX = targetSize.x / spriteSize.x;
        float scaleY = targetSize.y / spriteSize.y;
        Debug.Log(targetSize.x);
        Debug.Log(scaleX);
        Debug.Log(scaleY);

        // Use the smaller scale factor to maintain aspect ratio
        float uniformScale = Mathf.Min(scaleX, scaleY);

        // Apply scale
        item.transform.localScale = new Vector3(uniformScale, uniformScale, 1);
    }
}
