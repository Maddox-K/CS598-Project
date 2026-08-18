using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerData : MonoBehaviour, IDataPersistence
{
    private PlayerController _playerController;

    // Audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _damageSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _eatingSound;

    // Animation
    private Animator _animator;

    // Rendering
    private SpriteRenderer _playerRenderer;
    private Color _playerColor;
    [SerializeField] private Sprite _hurtFrogFace;

    // Currency
    private int _currencyCount;
    private GameObject _currencyGUI;
    private TextMeshProUGUI _currencyGUIText;
    private const float CooldownTime = 1.25f;

    // Health & taking damage
    private bool _canTakeDamage = true;
    private int _maxHealth = 3;
    private int _currentHealth;
    private GameObject _healthBar;
    private GameObject[] _hearts = new GameObject[4];
    private Image[] _heartRenderers = new Image[4];

    // Properties
    public int CurrencyCount
    {
        get => _currencyCount;
        private set => _currencyCount = value;
    }
    public bool CanTakeDamage
    {
        get => _canTakeDamage;
        set => _canTakeDamage = value;
    }

    // Lifecycle Methods
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        
        _audioSource = GetComponent<AudioSource>();

        _animator = GetComponent<Animator>();

        _playerRenderer = GetComponent<SpriteRenderer>();
        _playerColor = _playerRenderer.color;

        _currentHealth = _maxHealth;

        if (SceneManager.GetActiveScene().name != "BattleTest")
        {
            _currencyGUI = GameObject.FindGameObjectWithTag("CurrencyGUI");
            if (_currencyGUI != null)
            {
                _currencyGUIText = _currencyGUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                _currencyGUIText.text = _currencyCount.ToString();
            }
        }
        else
        {
            _healthBar = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(1).gameObject;
            if (_healthBar != null)
            {
                for (int i = 0; i < _hearts.Length; i++)
                {
                _hearts[i] = _healthBar.transform.GetChild(i).gameObject;
                _heartRenderers[i] = _hearts[i].GetComponent<Image>();
                }
            }
        }
    }

    void OnEnable()
    {
        QuestEvents.RewardCoins += RewardCoins;

        PlayerEvents.OnHealActivated += HealPlayer;
        PlayerEvents.OnObjectEaten += ActivateEatSound;
    }

    void OnDisable()
    {
        QuestEvents.RewardCoins -= RewardCoins;

        PlayerEvents.OnHealActivated -= HealPlayer;
        PlayerEvents.OnObjectEaten -= ActivateEatSound;
    }

    // Methods
    private void ActivateEatSound()
    {
        if (_audioSource != null && _eatingSound != null)
        {
            _audioSource.PlayOneShot(_eatingSound);
        }
    }

    private void HealPlayer(int amount)
    {
        if (_currentHealth >= _maxHealth)
        {
            return;
        }

        int amountHealed = 0;

        for (int i = _currentHealth; i < _maxHealth; i++)
        {
            if (amountHealed < amount)
            {
                _hearts[i].SetActive(true);
                amountHealed++;
            }
            else
            {
                break;
            }
        }

        if (_currentHealth + amount > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        else
        {
            _currentHealth += amount;
        }
    }

    public void SetHealth()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            _hearts[i].SetActive(true);
        }
    }

    private void RewardCoins(int amount)
    {
        IncrementCurrency(amount);
    }

    public void IncrementCurrency(int amount)
    {
        _currencyCount += amount;
        _currencyGUIText.text = _currencyCount.ToString();
    }

    public void DecreaseCurrency(int amount)
    {
        _currencyCount -= amount;
        _currencyGUIText.text = _currencyCount.ToString();
    }

    public void TakeDamage(Projectile projectile)
    {
        if (!_canTakeDamage || _playerController.IsDashing)
        {
            return;
        }

        int healthBeforeDamage = _currentHealth;
        _canTakeDamage = false;

        if (_currentHealth <= projectile.damage)
        {
            _currentHealth = 0;
            _audioSource.PlayOneShot(_deathSound);
        }
        else
        {
            _currentHealth -= projectile.damage;
            _audioSource.PlayOneShot(_damageSound);
        }
        Debug.Log(_currentHealth);

        while (healthBeforeDamage > _currentHealth)
        {
            StartCoroutine(FlashHeart(_heartRenderers[healthBeforeDamage - 1], 0.5f, 1));
            healthBeforeDamage--;
        }

        StartCoroutine(FlashEffect(CooldownTime, 7));

        if (_currentHealth == 0)
        {
            _canTakeDamage = false;
            _currentHealth = _maxHealth;
            PlayerEvents.InvokeDeactivate(1);
            PlayerEvents.InvokeDeactivate(3);
            _animator.SetTrigger("DeathTrigger");
            EncounterManager.instance.GameOver();
        }
        else
        {
            StartCoroutine(DamageCoolDown());
        }
    }

    IEnumerator DamageCoolDown()
    {
        yield return new WaitForSeconds(CooldownTime);
        _canTakeDamage = true;
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
            yield return new WaitForSeconds(duration * .75f);
            sourceImage.sprite = originalSprite;
            yield return new WaitForSeconds(duration * 0.25f);
        }
        sourceImage.gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            return;
        }

        _currencyCount = data.coinCount;

        if (_currencyGUI != null)
        {
            _currencyGUIText = _currencyGUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            _currencyGUIText.text = _currencyCount.ToString();
        }
    }

    public void SaveData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            return;
        }

        data.coinCount = _currencyCount;
    }
}
