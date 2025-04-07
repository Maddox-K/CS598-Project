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
                    inventoryController.isFulll[i] = true;
                    Instantiate(itemButton, inventoryController.slots[i].transform);
                    QuestEvents.OnItemCollected?.Invoke(itemName);
                    //Change list to ignore objects
                    //Destroy(gameObject);
                    gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}
