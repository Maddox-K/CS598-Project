using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tomato : MonoBehaviour
{
    // Button
    [SerializeField] private Button tomatoButton;

    // Inventory
    InventoryController inventoryController;

    // Player
    private GameObject player;
    private PlayerController playerController;

    // Tomato Buff Variables
    private float duration = 7f;
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
        inventoryController.slotButtons[slotIndex].interactable = true;
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
}
