using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    private InventoryController inventoryController;

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
                    //Item can be added to Inventory
                    inventoryController.isFulll[i] = true;
                    break;
                }
            }
        }
    }
}
