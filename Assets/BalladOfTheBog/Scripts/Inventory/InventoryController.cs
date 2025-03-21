using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] public bool[] isFulll;
    [SerializeField] public GameObject[] slots;

    [SerializeField] private GameObject InventoryUI;

    private void Update()
    {

    }

    public void LoadData(GameData data)
    {
        //data.currentInventory.TryGetValue();
        //if ()
        //{
        //    gameObject.SetActive(false);
        //}
    }


    //public void SaveData(GameData data)
    //{
    //    if (data.currentInventory.ContainsKey(slots.ToString()))
    //    {
    //        data.currentInventory.Remove(slots.ToString());
    //    }
    //}
}