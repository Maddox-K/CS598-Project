using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : NPC
{
    // Menu
    private ShopMenuController _shopMenu;

    // NPC-specific Shop and Interaction Info
    [SerializeField] private ShopInventory _inventory;
    [SerializeField] private Dialogue _preShopDialogue;
    [SerializeField] private Dialogue _postShopDialogue;

    void Awake()
    {
        _shopMenu = GameObject.FindGameObjectWithTag("ShopMenu").GetComponent<ShopMenuController>();
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
                    Instantiate(_inventory.shopSlots[i], _shopMenu.slots[i].transform);
                    _shopMenu.slotContents[i] = _inventory.shopSlots[i];
                }
            }
            // initiate shopping
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
}
