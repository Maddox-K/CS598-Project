using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    private InventoryController inventoryController;
    public GameObject itemButton;
    public string itemName;

    private void Start()
    {
        inventoryController = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < inventoryController.slots.Length; i++)
            {
                if (inventoryController.isFulll[i] == false)
                {
                    if (itemName == "Mail")
                    {
                        GameManager.instance.gameData._pickedUpLetter = true;
                    }

                    inventoryController.isFulll[i] = true;
                    Instantiate(itemButton, inventoryController.slots[i].transform);

                    inventoryController.slotButtons[i].interactable = false;

                    QuestEvents.OnItemCollected?.Invoke(itemName);
                    
                    gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}
