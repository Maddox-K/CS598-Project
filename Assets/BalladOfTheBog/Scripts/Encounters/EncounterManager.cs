using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager instance { get; private set; }
    private const int spawnDelay = 1;

    // Audio
    private AudioSource _audioSource;

    // Game Over
    private GameObject _gameOverPrefab;
    private CanvasGroup _gameOverCanvasGroup;
    private Button[] _gameOverButtons = new Button[2];
    private const float fadeDuration = 0.7f;

    // Player Stuff
    private GameObject _player;
    private PlayerController _playercontroller;
    private PlayerData _playerData;

    // Scene Management
    public string prevScene;
    private bool _encounterInProgress;
    private GameObject _transitionParent;
    private Animator _transition;

    // Enemy
    private EnemyAttacks _enemyAttacks;
    private List<GameObject> _projectiles = new List<GameObject>();
    private List<Vector2> _velocities = new List<Vector2>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
        {
            _playercontroller = _player.GetComponent<PlayerController>();
            _playerData = _player.GetComponent<PlayerData>();
        }

        _transitionParent = GameObject.FindGameObjectWithTag("TransitionHolder");
        if (_transitionParent != null)
        {
            _transition = _transitionParent.transform.GetChild(0).GetComponent<Animator>();
        }

        if (scene.name == "BattleTest" && _enemyAttacks != null)
        {
            _audioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

            _gameOverPrefab = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject;
            if (_gameOverPrefab != null)
            {
                _gameOverCanvasGroup = _gameOverPrefab.GetComponent<CanvasGroup>();
            }

            _gameOverButtons[0] = _gameOverPrefab.transform.GetChild(1).gameObject.GetComponent<Button>();
            _gameOverButtons[1] = _gameOverPrefab.transform.GetChild(2).gameObject.GetComponent<Button>();
            _gameOverButtons[0].onClick.AddListener(() => StartEncounter());
            _gameOverButtons[1].onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));

            StartEncounter();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EncounterInit(Enemy enemy)
    {
        _playercontroller.interact.Disable();

        GameManager.instance.gameData.lastEnemyEncountered = enemy.Id;
        _enemyAttacks = enemy.enemyAttacks;
        prevScene = enemy.gameObject.scene.name;
        if (PlayerPrefs.GetInt("AutoSave") == 1)
        {
            GameManager.instance.SaveGame(false);
        }
        else
        {
            GameManager.instance.SaveGame();
        }

        StartCoroutine(EncounterTransition());
    }

    private IEnumerator EncounterTransition()
    {
        if (_transition != null)
        {
            _transition.SetTrigger("Start");
        }

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("BattleTest");
    }

    private void StartEncounter()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        _gameOverCanvasGroup.alpha = 0;
        _playerData.SetHealth();
        _playercontroller.gameObject.transform.position = new Vector3(0, 0, 0);
        _playercontroller.lookDirection = new Vector2(0, -1);
        _playercontroller.dash.Enable();

        _encounterInProgress = true;
        if (_gameOverPrefab.activeSelf == true)
        {
            _gameOverPrefab.SetActive(false);
        }
        if (_playercontroller.move.enabled == false)
        {
            _playercontroller.move.Enable();
        }

        StartCoroutine(DelaySpawns());
        StartCoroutine(EndBattle(_enemyAttacks));
    }

    IEnumerator EndBattle(EnemyAttacks enemyAttacks)
    {
        yield return new WaitForSeconds(enemyAttacks.endTime);

        GameData data = GameManager.instance.gameData;
        if (data.enemiesEncountered.ContainsKey(data.lastEnemyEncountered))
        {
            data.enemiesEncountered[data.lastEnemyEncountered] = true;
        }
        SetBattleEnd(true);
    }

    public void SetBattleEnd(bool reachedEnd)
    {
        _encounterInProgress = false;
        _projectiles.Clear();
        _velocities.Clear();
        _playercontroller.dash.Disable();

        if (reachedEnd)
        {
            SceneManager.LoadScene(prevScene);
        }
        else
        {
            _playerData.canTakeDamage = false;
        }
    }

    IEnumerator DelaySpawns()
    {
        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnProjectiles(_enemyAttacks));
    }

    IEnumerator SpawnProjectiles(EnemyAttacks enemyAttacks)
    {
        Projectile proj = enemyAttacks.projectile;
        Vector2[] positions = enemyAttacks.spawnPositions;
        Vector2[] directions = enemyAttacks.launchDirections;
        float[] times = enemyAttacks.launchTimes;
        float[] speeds = enemyAttacks.projectileSpeeds;

        for (int i = 0; i < positions.Length; i++)
        {
            yield return new WaitForSeconds(times[i]);

            GameObject projectile = Instantiate(proj.gameObject, (Vector3)positions[i], Quaternion.identity);
            _projectiles.Add(projectile);
            _velocities.Add(directions[i].normalized * speeds[i]);
        }
    }

    public void GameOver()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        _encounterInProgress = false;
        StopAllCoroutines();
        for (int i = 0; i < _projectiles.Count; i++)
        {
            Destroy(_projectiles[i]);
        }
        _projectiles.Clear();
        _velocities.Clear();
        _gameOverPrefab.SetActive(true);

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1.1f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _gameOverCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        _gameOverCanvasGroup.alpha = 1;

        _playerData.canTakeDamage = true;
    }

    private void FixedUpdate()
    {
        if (!_encounterInProgress || _enemyAttacks.affectedByGravity)
        {
            return;
        }
        if (_projectiles.Count > 0)
        {
            for (int i = 0; i < _projectiles.Count; i++)
            {
                _projectiles[i].GetComponent<Rigidbody2D>().velocity = _velocities[i];
            }
        }
    }
}
