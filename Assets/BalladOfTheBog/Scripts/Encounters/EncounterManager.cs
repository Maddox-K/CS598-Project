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
    private const float fadeDuration = 0.6f;

    // Player Stuff
    private GameObject _player;
    private PlayerController _playercontroller;
    private PlayerData _playerData;

    // Scene Management
    public string prevScene;
    private bool _encounterInProgress;

    // Transitioning
    private GameObject _transitionParent;
    private GameObject _transitionObject;
    private Animator _transition;
    private GameObject _leftFrogPanel;
    private GameObject _rightFrogPanel;
    private Animator _quitTransition;

    // Enemy
    private EnemyAttacks _enemyAttacks;
    private List<Rigidbody2D> _projectileBodies = new List<Rigidbody2D>();
    private List<Vector2> _velocities = new List<Vector2>();
    private List<GameObject> _specialProjectiles = new List<GameObject>();
    private GameObject _grid;

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

    void Start()
    {
        
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
            _transitionObject = _transitionParent.transform.GetChild(0).gameObject;
            _quitTransition = _transitionParent.transform.GetChild(1).GetComponent<Animator>();
            _transition = _transitionObject.GetComponent<Animator>();
            
            _leftFrogPanel = _transitionObject.transform.GetChild(0).transform.GetChild(0).gameObject;
            _rightFrogPanel = _transitionObject.transform.GetChild(0).transform.GetChild(1).gameObject;
        }

        if (scene.name == "BattleTest" && _enemyAttacks != null)
        {
            _transition.SetTrigger("End");
            _quitTransition.enabled = false;

            _audioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

            _grid = GameObject.FindGameObjectWithTag("Grid");

            _gameOverPrefab = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject;
            if (_gameOverPrefab != null)
            {
                _gameOverCanvasGroup = _gameOverPrefab.GetComponent<CanvasGroup>();
            }

            _gameOverButtons[0] = _gameOverPrefab.transform.GetChild(1).gameObject.GetComponent<Button>();
            _gameOverButtons[1] = _gameOverPrefab.transform.GetChild(2).gameObject.GetComponent<Button>();

            StartEncounter();
        }
    }

    private void TryAgain()
    {
        foreach (Button button in _gameOverButtons)
        {
            button.interactable = false;
        }

        StartEncounter();
    }

    private IEnumerator GiveUp()
    {

        Debug.Log("give up called");
        if (_quitTransition != null)
        {
            _quitTransition.gameObject.SetActive(true);
            _quitTransition.enabled = true;
            _quitTransition.SetTrigger("EndScene");
        }

        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene("Main Menu");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EncounterInit(Enemy enemy)
    {
        _playercontroller.interact.Disable();

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

        GameManager.instance.gameData.lastEnemyEncountered = enemy.Id;

        StartCoroutine(EncounterTransition());
    }

    private IEnumerator EncounterTransition()
    {
        if (_transition != null)
        {
            _transition.SetTrigger("Start");
        }

        yield return new WaitForSeconds(1f);

        _leftFrogPanel.transform.position = new Vector3(-225, 540, 0);
        _rightFrogPanel.transform.position = new Vector3(225, 540, 0);

        SceneManager.LoadScene("BattleTest");
    }

    private void StartEncounter()
    {
        if (_audioSource != null)
        {
            _audioSource.Play();
        }

        switch (_enemyAttacks.encounterScenery)
        {
            case 0:
                _grid.transform.GetChild(0).gameObject.SetActive(true);
                break;
            case 1:
                _grid.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }

        _gameOverCanvasGroup.alpha = 0;
        if (!_gameOverButtons[0].interactable)
        {
            foreach (Button button in _gameOverButtons)
            {
                button.interactable = true;
            }
        }
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

        for (int i = 0; i < _projectileBodies.Count; i++)
        {
            Destroy(_projectileBodies[i].gameObject);
        }
        for (int i = 0; i < _specialProjectiles.Count; i++)
        {
            Destroy(_specialProjectiles[i]);
        }
        _projectileBodies.Clear();
        _specialProjectiles.Clear();
        _velocities.Clear();
        
        _playercontroller.dash.Disable();

        if (reachedEnd)
        {
            StartCoroutine(TransitionBackToGame());
        }
        else
        {
            _playerData.canTakeDamage = false;
        }
    }

    private IEnumerator TransitionBackToGame()
    {
        Debug.Log("transition called");
        _transition.SetTrigger("Start");
        _transition.Play("FrogTransition");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(prevScene);
    }

    IEnumerator DelaySpawns()
    {
        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnProjectiles(_enemyAttacks));
    }

    IEnumerator SpawnProjectiles(EnemyAttacks enemyAttacks)
    {
        Projectile[] projectiles = enemyAttacks.projectiles;
        int[] selector = enemyAttacks.projectileSelector;
        Vector2[] positions = enemyAttacks.spawnPositions;
        Vector2[] directions = enemyAttacks.launchDirections;
        float[] times = enemyAttacks.launchTimes;
        float[] speeds = enemyAttacks.projectileSpeeds;

        for (int i = 0; i < positions.Length; i++)
        {
            yield return new WaitForSeconds(times[i]);

            GameObject projectile = Instantiate(projectiles[selector[i]].gameObject, (Vector3)positions[i], Quaternion.identity);

            if (projectiles[selector[i]].GetType() == typeof(Projectile))
            {
                _projectileBodies.Add(projectile.GetComponent<Rigidbody2D>());
                _velocities.Add(directions[i].normalized * speeds[i]);
            }
            else
            {
                _specialProjectiles.Add(projectile);
            }
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

        for (int i = 0; i < _projectileBodies.Count; i++)
        {
            Destroy(_projectileBodies[i].gameObject);
        }
        for (int i = 0; i < _specialProjectiles.Count; i++)
        {
            Destroy(_specialProjectiles[i]);
        }
        _projectileBodies.Clear();
        _specialProjectiles.Clear();
        _velocities.Clear();

        _gameOverPrefab.SetActive(true);
        for (int i = 0; i < _gameOverButtons.Length; i++)
        {
            _gameOverButtons[i].interactable = false;
        }

        StartCoroutine(FadeInGameOver());
    }

    private IEnumerator FadeInGameOver()
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

        _gameOverButtons[0].onClick.AddListener(() => TryAgain());
        _gameOverButtons[1].onClick.AddListener(() => StartCoroutine(GiveUp()));
        for (int i = 0; i < _gameOverButtons.Length; i++)
        {
            _gameOverButtons[i].interactable = true;
        }
        _gameOverButtons[0].Select();
        
        _playerData.canTakeDamage = true;
    }

    private void FixedUpdate()
    {
        if (!_encounterInProgress)
        {
            return;
        }
        if (_projectileBodies.Count > 0)
        {
            for (int i = 0; i < _projectileBodies.Count; i++)
            {
                _projectileBodies[i].velocity = _velocities[i];
            }
        }
    }
}
