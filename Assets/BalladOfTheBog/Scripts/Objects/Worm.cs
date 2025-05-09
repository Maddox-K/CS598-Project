using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Worm : MonoBehaviour
{
    // Worm Stuff
    [SerializeField] private Button _wormButton;
    [SerializeField] private int _healAmount;

    private InventoryController _inventoryController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            _wormButton.onClick.AddListener(() => HealPlayer(_healAmount));
            _inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
        }
    }

    private void HealPlayer(int amount)
    {
        PlayerEvents.InvokeObjectEaten();

        PlayerEvents.InvokeHealActivated(amount);

        int slotIndex = transform.parent.GetSiblingIndex();
        _inventoryController.RemoveItem(slotIndex);
    }
}
