using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour, I_DataPersistence
{
    // rendering
    private SpriteRenderer _playerRenderer;
    private Color _playerColor;

    // currency
    public int currency_count;
    private GameObject currencyGUI;
    private TextMeshProUGUI _currencyGUIText;
    public bool canTakeDamage = true;
    private const float cooldownTime = 1.25f;

    // health
    private int _maxHealth = 3;
    private int _currentHealth;
    private GameObject _healthBar;
    private GameObject[] _hearts = new GameObject[4];

    private void Awake()
    {
        _playerRenderer = GetComponent<SpriteRenderer>();
        _playerColor = _playerRenderer.color;

        _currentHealth = _maxHealth;
        currencyGUI = GameObject.FindGameObjectWithTag("CurrencyGUI");
        if (currencyGUI != null)
        {
            _currencyGUIText = currencyGUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            _currencyGUIText.text = currency_count.ToString();
        }
        else
        {
            //Debug.Log("trying to find health bar");
            _healthBar = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(1).gameObject;
            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i] = _healthBar.transform.GetChild(i).gameObject;
                //Debug.Log("trying to get heart");
            }
        }
    }

    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        
    }

    public void SetHealth()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            _hearts[i].SetActive(true);
        }
    }

    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void IncrementCurrency()
    {
        currency_count++;
        _currencyGUIText.text = currency_count.ToString();
    }

    public void TakeDamage(Projectile projectile)
    {
        if (!canTakeDamage)
        {
            return;
        }

        canTakeDamage = false;

        if (_currentHealth < projectile.damage)
        {
            _currentHealth = 0;
        }
        else
        {
            _currentHealth -= projectile.damage;
        }
        Debug.Log(_currentHealth);

        if (_hearts[_currentHealth].activeSelf)
        {
            _hearts[_currentHealth].SetActive(false);
            if (projectile.damage > 1)
            {
                for (int i = 1; i < projectile.damage; i++)
                {
                    _hearts[_currentHealth + i].SetActive(false);
                }
            }
        }

        StartCoroutine(DamageCoolDown());
        StartCoroutine(FlashEffect(cooldownTime, 7));

        if (_currentHealth == 0)
        {
            _currentHealth = _maxHealth;
            EncounterManager.instance.GameOver();
        }
    }

    IEnumerator DamageCoolDown()
    {
        yield return new WaitForSeconds(cooldownTime);
        canTakeDamage = true;
    }

    private IEnumerator FlashEffect(float duration, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            _playerRenderer.color = new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.1f);
            yield return new WaitForSeconds(duration / (flashCount * 2));
            _playerRenderer.color = _playerColor;
            yield return new WaitForSeconds(duration / (flashCount * 2));
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
