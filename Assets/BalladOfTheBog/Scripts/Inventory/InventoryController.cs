using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour, IDataPersistence
{
    [SerializeField] public bool[] isFulll;
    [SerializeField] public GameObject[] slots;
    [SerializeField] private GameObject InventoryUI;

    public List<string> inventoryItems = new List<string>();
    public void LoadData(GameData data)
    {
        inventoryItems.Clear(); // clear first
        for (int i = 0; i < slots.Length; i++)
        {
            foreach (Transform child in slots[i].transform)
            {
                Destroy(child.gameObject); // clean existing UI
            }

            isFulll[i] = false; // reset slots
        }

        foreach (var kvp in data.inventorySlotData)
        {
            int slotIndex = kvp.Key;
            string itemName = kvp.Value;

            GameObject itemButton = Resources.Load<GameObject>("InventoryItems/" + itemName);
            if (itemButton != null && slotIndex >= 0 && slotIndex < slots.Length)
            {
                Instantiate(itemButton, slots[slotIndex].transform);
                isFulll[slotIndex] = true; // Mark slot as full
                inventoryItems.Add(itemName);
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.inventorySlotData.Clear();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                string itemName = slots[i].transform.GetChild(0).name.Replace("(Clone)", "");
                data.inventorySlotData[i] = itemName;
            }
            if (slots[i].transform.childCount == 0)
            {
                isFulll[i] = false;
            }

        }

    }


    public void AddItem(string itemName, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
            return;

        inventoryItems.Add(itemName);
        isFulll[slotIndex] = true;
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

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
            return;

        Transform slot = slots[slotIndex].transform;
        var itemUI = slot.GetComponentInChildren<Stackable>();

        if (itemUI != null)
        {
            itemUI.RemoveOne();

            if (itemUI.quantity <= 0)
            {
                isFulll[slotIndex] = false;
                inventoryItems.Remove(itemUI.itemName); // optional
            }
        }
    }


    //public void RemoveItem(int slotIndex)
    //{
    //    if (slotIndex < 0 || slotIndex >= slots.Length)
    //        return;

    //    // Destroy the item in the slot
    //    foreach (Transform child in slots[slotIndex].transform)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    // Mark slot as empty
    //    isFulll[slotIndex] = false;

    //    // Optionally remove the item from inventoryItems list
    //    if (inventoryItems.Count > slotIndex)
    //    {
    //        inventoryItems.RemoveAt(slotIndex);
    //    }
    //}


}
