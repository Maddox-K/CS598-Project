using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] public bool[] isFulll;
    [SerializeField] public GameObject[] slots;
    [SerializeField] private GameObject InventoryUI;

    private List<string> inventoryItems = new List<string>();

    public void LoadData(GameData data)
    {
        inventoryItems = new List<string>(data.currentInventory);

        for (int i = 0; i < inventoryItems.Count && i < slots.Length; i++)
        {
            if (!string.IsNullOrEmpty(inventoryItems[i]))
            {
                GameObject itemButton = Resources.Load<GameObject>("InventoryItems/" + inventoryItems[i]);
                if (itemButton != null)
                {
                    Instantiate(itemButton, slots[i].transform);
                    isFulll[i] = true;
                }
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.currentInventory = new List<string>(inventoryItems);
    }

    public void AddItemToInventory(string itemName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!isFulll[i])
            {
                inventoryItems.Add(itemName);
                GameObject itemButton = Resources.Load<GameObject>("InventoryItems/" + itemName);
                if (itemButton != null)
                {
                    Instantiate(itemButton, slots[i].transform);
                    isFulll[i] = true;
                }
                break;
            }
        }
    }

    public bool CheckIfFull()
    {
        foreach (bool full in isFulll)
        {
            if (!full)
            {
                return false;
            }
        }

        return true;
    }
}
