using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour, IDataPersistence
{
    // core fields
    [SerializeField] private string _id;
    [SerializeField] private PlayerData _playerData;
    public bool collected;

    // audio
    private AudioSource _audioSource;

    // animation
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private const float fadeDuration = 0.5f;
    private const float moveDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _animator = gameObject.GetComponent<Animator>();

        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        _id = System.Guid.NewGuid().ToString();
    }

    public void Collect()
    {
        if (collected)
        {
            return;
        }

        collected = true;
        
        _playerData.IncrementCurrency();

        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        StartCoroutine(CollectAnimate());
    }

    IEnumerator CollectAnimate()
    {
        if (_animator != null)
        {
            _animator.speed = 3f;
        }

        float elapsed = 0f;
        Vector2 startPosition = transform.position;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Move the coin up
            float moveProgress = elapsed / fadeDuration;
            transform.position = startPosition + new Vector2(0, moveProgress * moveDistance);

            // Fade the coin out
            if (_spriteRenderer != null)
            {
                Color color = _spriteRenderer.color;
                color.a = 1f - moveProgress;
                _spriteRenderer.color = color;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        data.coinsCollected.TryGetValue(_id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.coinsCollected.ContainsKey(_id))
        {
            data.coinsCollected.Remove(_id);
        }
        data.coinsCollected.Add(_id, collected);
    }
}
