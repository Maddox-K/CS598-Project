using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour, I_DataPersistence
{
    // currency
    public int currency_count;
    private bool displayCurrencyGUI = true;
    private TextMeshProUGUI currencyGUI;

    // health
    public int health = 3;

    // Start is called before the first frame update
    void Start()
    {
        if (displayCurrencyGUI)
        {
            currencyGUI = GameObject.FindGameObjectWithTag("CurrencyGUI").transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            if (currencyGUI != null)
            {
                currencyGUI.text = currency_count.ToString();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleTest")
        {
            displayCurrencyGUI = false;
        }
        else
        {
            displayCurrencyGUI = true;
        }
    }

    public void IncrementCurrency()
    {
        currency_count++;
        currencyGUI.text = currency_count.ToString();
    }

    public void TakeDamage(Projectile projectile)
    {
        health -= projectile.damage;
        Debug.Log(health);
        if (health == 0)
        {
            health = 3;
            EncounterManager.instance.GameOver();
        }
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
