using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour, IDataPersistence
{
    private PlayerController pcontroller;

    // audio
    private AudioSource _audioSource;
    public AudioClip DamageSound;
    public AudioClip DeathSound;

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
    private const float cooldownTime = 1.25f;

    // health and taking damage
    public bool canTakeDamage = true;
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

        if (SceneManager.GetActiveScene().name != "BattleTest")
        {
            currencyGUI = GameObject.FindGameObjectWithTag("CurrencyGUI");
            if (currencyGUI != null)
            {
                _currencyGUIText = currencyGUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                _currencyGUIText.text = currency_count.ToString();
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

    void Start()
    {
        pcontroller = GetComponent<PlayerController>();
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetHealth()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            _hearts[i].SetActive(true);
        }
    }

    public void IncrementCurrency()
    {
        currency_count++;
        _currencyGUIText.text = currency_count.ToString();
    }

    public void DecreaseCurrency(int amount)
    {
        
    }

    public void TakeDamage(Projectile projectile)
    {
        if (!canTakeDamage || pcontroller.isDashing)
        {
            return;
        }

        int healthBeforeDamage = _currentHealth;
        canTakeDamage = false;

        if (_currentHealth <= projectile.damage)
        {
            _currentHealth = 0;
            _audioSource.PlayOneShot(DeathSound);
        }
        else
        {
            _currentHealth -= projectile.damage;
            _audioSource.PlayOneShot(DamageSound);
        }
        Debug.Log(_currentHealth);

        while (healthBeforeDamage > _currentHealth)
        {
            StartCoroutine(FlashHeart(_heartRenderers[healthBeforeDamage - 1], 0.5f, 1));
            healthBeforeDamage--;
        }

        StartCoroutine(FlashEffect(cooldownTime, 7));

        if (_currentHealth == 0)
        {
            canTakeDamage = false;
            _currentHealth = _maxHealth;
            pcontroller.move.Disable();
            pcontroller.dash.Disable();
            animator.SetTrigger("DeathTrigger");
            EncounterManager.instance.GameOver();
        }
        else
        {
            StartCoroutine(DamageCoolDown());
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
        currency_count = data.coinCount;

        if (currencyGUI != null)
        {
            _currencyGUIText = currencyGUI.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            _currencyGUIText.text = currency_count.ToString();
        }
    }

    public void SaveData(GameData data)
    {
        data.coinCount = currency_count;
    }
}
