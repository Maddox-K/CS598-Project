using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour, I_DataPersistence
{
    // core fields
    [SerializeField] private string id;
    [SerializeField] private PlayerData playerData;
    public bool collected;

    // animation
    private Animator animator;
    private SpriteRenderer _spriteRenderer;
    private const float _fadeDuration = 0.5f;
    private const float _moveDistance = 1f;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void Collect()
    {
        collected = true;
        //gameObject.SetActive(false);
        playerData.IncrementCurrency();

        StartCoroutine(CollectAnimate());
    }

    IEnumerator CollectAnimate()
    {
        if (animator != null)
        {
            animator.speed = 3f;
        }

        float elapsed = 0f;
        Vector2 startPosition = transform.position;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;

            // Move the coin up
            float moveProgress = elapsed / _fadeDuration;
            transform.position = startPosition + new Vector2(0, moveProgress * _moveDistance);

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
        data.coinsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.coinsCollected.ContainsKey(id))
        {
            data.coinsCollected.Remove(id);
        }
        data.coinsCollected.Add(id, collected);
    }

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
