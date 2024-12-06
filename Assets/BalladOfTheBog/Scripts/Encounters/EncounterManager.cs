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
    private GameObject gameOverPrefab;
    private CanvasGroup _gameOverCanvasGroup;
    private Button[] gameOverButtons = new Button[2];
    private const float fadeDuration = 0.7f;

    // Player Stuff
    private GameObject _player;
    private PlayerController pcontroller;
    private PlayerData _playerData;

    // Scene Management
    public string prevScene;
    private bool encounterInProgress;
    private GameObject _transitionParent;
    private Animator _transition;

    // Enemy
    private EnemyAttacks eAttacks;
    private List<GameObject> projectiles = new List<GameObject>();
    private List<Vector2> velocities = new List<Vector2>();

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
        Debug.Log("finding player");
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
        {
            pcontroller = _player.GetComponent<PlayerController>();
            _playerData = _player.GetComponent<PlayerData>();
        }

        _transitionParent = GameObject.FindGameObjectWithTag("Transition");
        if (_transitionParent != null)
        {
            _transition = _transitionParent.GetComponent<Animator>();
        }

        if (scene.name == "BattleTest" && eAttacks != null)
        {
            _audioSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

            gameOverPrefab = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject;
            if (gameOverPrefab != null)
            {
                _gameOverCanvasGroup = gameOverPrefab.GetComponent<CanvasGroup>();
            }

            gameOverButtons[0] = gameOverPrefab.transform.GetChild(1).gameObject.GetComponent<Button>();
            gameOverButtons[1] = gameOverPrefab.transform.GetChild(2).gameObject.GetComponent<Button>();
            gameOverButtons[0].onClick.AddListener(() => StartEncounter());
            gameOverButtons[1].onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));

            StartEncounter();
        }
    }

    void Start()
    {
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EncounterInit(Enemy enemy)
    {
        pcontroller.interact.Disable();

        GameManager.instance.gameData.lastEnemyEncountered = enemy.Id;
        eAttacks = enemy.enemyAttacks;
        prevScene = enemy.gameObject.scene.name;
        GameManager.instance.SaveGame();

        StartCoroutine(EncounterTransition());
        
        //SceneManager.LoadScene("BattleTest");
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
        pcontroller.gameObject.transform.position = new Vector3(0, 0, 0);
        pcontroller.lookDirection = new Vector2(0, -1);
        pcontroller.dash.Enable();

        encounterInProgress = true;
        if (gameOverPrefab.activeSelf == true)
        {
            gameOverPrefab.SetActive(false);
        }
        if (pcontroller.move.enabled == false)
        {
            pcontroller.move.Enable();
        }

        StartCoroutine(DelaySpawns());
        StartCoroutine(EndBattle(eAttacks));
    }

    IEnumerator EndBattle(EnemyAttacks enemyAttacks)
    {
        yield return new WaitForSeconds(enemyAttacks.endTime);

        GameData data = GameManager.instance.gameData;
        if (data.enemiesEncountered.ContainsKey(data.lastEnemyEncountered))
        {
            data.enemiesEncountered[data.lastEnemyEncountered] = true;
        }
        SetBattleEnd();
    }

    public void SetBattleEnd()
    {
        encounterInProgress = false;
        projectiles.Clear();
        velocities.Clear();
        pcontroller.dash.Disable();
        SceneManager.LoadScene(prevScene);
    }

    IEnumerator DelaySpawns()
    {
        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnProjectiles(eAttacks));
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
            projectiles.Add(projectile);
            velocities.Add(directions[i].normalized * speeds[i]);
        }
    }

    public void GameOver()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        encounterInProgress = false;
        StopAllCoroutines();
        for (int i = 0; i < projectiles.Count; i++)
        {
            Destroy(projectiles[i]);
        }
        projectiles.Clear();
        velocities.Clear();
        gameOverPrefab.SetActive(true);

        //_playerData.canTakeDamage = true;

        StartCoroutine(FadeIn());
        
        //pcontroller.move.Disable();
        //pcontroller.dash.Disable();
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
        if (!encounterInProgress || eAttacks.affectedByGravity)
        {
            return;
        }
        if (projectiles.Count > 0)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].GetComponent<Rigidbody2D>().velocity = velocities[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
