using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour, I_DataPersistence
{
    private PlayerController pcontroller;

    // animation
    public Animator animator;

    // rendering
    private SpriteRenderer _playerRenderer;
    private Color _playerColor;
    [SerializeField] private Sprite _hurtFrogFace;

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
    private Image[] _heartRenderers = new Image[4];

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
                _heartRenderers[i] = _hearts[i].GetComponent<Image>();
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
        pcontroller = GetComponent<PlayerController>();
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

        int healthBeforeDamage = _currentHealth;
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

        /* if (_hearts[_currentHealth].activeSelf)
        {
            _hearts[_currentHealth].SetActive(false);
            if (projectile.damage > 1)
            {
                for (int i = 1; i < projectile.damage; i++)
                {
                    _hearts[_currentHealth + i].SetActive(false);
                }
            }
        } */
        while (healthBeforeDamage > _currentHealth)
        {
            StartCoroutine(FlashHeart(_heartRenderers[healthBeforeDamage - 1], 0.5f, 1));
            //_hearts[healthBeforeDamage - 1].SetActive(false);
            healthBeforeDamage--;
        }

        StartCoroutine(DamageCoolDown());
        StartCoroutine(FlashEffect(cooldownTime, 7));

        if (_currentHealth == 0)
        {
            _currentHealth = _maxHealth;
            pcontroller.move.Disable();
            pcontroller.dash.Disable();
            StartCoroutine(AnimateDeath());
            //animator.SetTrigger("DeathTrigger");
            //EncounterManager.instance.GameOver();
        }
    }

    IEnumerator AnimateDeath()
    {
        animator.SetTrigger("DeathTrigger");

        yield return new WaitForSeconds(1.7f);

        EncounterManager.instance.GameOver();
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

    private IEnumerator FlashHeart(Image sourceImage, float duration, int flashCount)
    {
        Sprite originalSprite = sourceImage.sprite;

        for (int i = 0; i < flashCount; i++)
        {
            sourceImage.sprite = _hurtFrogFace;
            //yield return new WaitForSeconds(duration / (flashCount * 2));
            yield return new WaitForSeconds(duration * .75f);
            sourceImage.sprite = originalSprite;
            //yield return new WaitForSeconds(duration / (flashCount * 2));
            yield return new WaitForSeconds(duration * 0.25f);
        }
        sourceImage.gameObject.SetActive(false);
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
