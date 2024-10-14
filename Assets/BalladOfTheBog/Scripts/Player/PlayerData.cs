using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour, I_DataPersistence
{
    // coin collection
    public int currency_count;
    [SerializeField] public TextMeshProUGUI currencyGUI;

    // Start is called before the first frame update
    void Start()
    {
        currencyGUI.text = currency_count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncrementCurrency()
    {
        currency_count++;
        currencyGUI.text = currency_count.ToString();
    }

    public void LoadData(GameData data)
    {
        currency_count = data.coinCount;
    }

    public void SaveData(GameData data)
    {
        data.coinCount = currency_count;
    }
}
