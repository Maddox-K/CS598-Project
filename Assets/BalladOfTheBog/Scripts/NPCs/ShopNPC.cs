using UnityEngine;

public class ShopNPC : NPC
{
    // Menu
    private ShopMenuController _shopMenu;

    // NPC-specific Shop and Interaction Info
    [SerializeField] public ShopInventory inventory;
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
    public void EndInteraction(bool purchased)
    {
        dialogueManager.EndConversation(_preShopDialogue);

        if (_postShopDialogue != null)
        {
            if (!purchased)
            {
                dialogueManager.DisplayNext(_postShopDialogue.choices[0].nextDialogue);
            }
            else
            {
                dialogueManager.DisplayNext(_postShopDialogue.choices[1].nextDialogue);
            }
        }
    }

    public Dialogue GetPostDialogue()
    {
        return _postShopDialogue;
    }
}
