using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tomato : MonoBehaviour
{
    [SerializeField] private Button tomatoButton;
    [SerializeField] private GameObject tomatoPrefab;

    InventoryController inventoryController;

    private GameObject player;
    private PlayerController playerController;
    private float duration = 10f;
    private bool isBuffActive = false;

    private void Start()
    {
        tomatoButton.onClick.AddListener(ApplySpeedBuff);
        inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
    }

    private void ApplySpeedBuff()
    {
        if (isBuffActive) return;

        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();

        Debug.Log("Speed buff applied!");
        playerController.speed = 10.0f;
        isBuffActive = true;

        // Start the coroutine from the player instead of the tomato
        player.GetComponent<MonoBehaviour>().StartCoroutine(RemoveEffectAfterDelay());

        int slotIndex = transform.parent.GetSiblingIndex();
        inventoryController.RemoveItem(slotIndex);

        Destroy(gameObject); // Destroy the tomato object
    }

    private IEnumerator RemoveEffectAfterDelay()
    {
        yield return new WaitForSeconds(duration);

        if (playerController != null)
        {
            Debug.Log("Speed buff expired. Resetting speed.");
            playerController.speed = 5.0f;
        }

        isBuffActive = false;
    }

    private int GetSlotIndexOfItemInInventory()
    {
        for (int i = 0; i < inventoryController.inventoryItems.Count; i++)
        {
            if (inventoryController.inventoryItems[i] == "TomatoButton") // Or any other item name
            {
                return i;
            }
        }

        return -1; // Return -1 if item is not found
    }

}

