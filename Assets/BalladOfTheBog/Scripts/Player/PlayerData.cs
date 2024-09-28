using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
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

    public void changeCurrency()
    {
        currency_count++;
        currencyGUI.text = currency_count.ToString();
    }
}
