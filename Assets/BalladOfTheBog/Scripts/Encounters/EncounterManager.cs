using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager instance { get; private set; }
    private GameObject gameOverPrefab;
    private Button[] gameOverButtons = new Button[2];
    private PlayerController pcontroller;
    public string prevScene;
    private bool encounterInProgress;
    public Enemy currentEnemy;
    private EnemyAttacks eAttacks;
    private const int spawnDelay = 1;
    private List<GameObject> projectiles = new List<GameObject>();
    private List<Vector2> velocities = new List<Vector2>();
    //[SerializeField] private DialogueManager dialogueManager;
    //private bool dialogueStarted;

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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EncounterInit(Enemy enemy)
    {
        //encounterInProgress = true;
        eAttacks = enemy.enemyAttacks;
        prevScene = enemy.gameObject.scene.name;
        GameManager.instance.SaveGame();
        Debug.Log("Enemy before loading scene: " + currentEnemy);
        SceneManager.LoadScene("BattleTest");
    }

    private void StartEncounter()
    {
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

        encounterInProgress = false;
        SceneManager.LoadScene(prevScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleTest" && eAttacks != null)
        {
            gameOverPrefab = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject;
            gameOverButtons[0] = gameOverPrefab.transform.GetChild(1).gameObject.GetComponent<Button>();
            gameOverButtons[1] = gameOverPrefab.transform.GetChild(2).gameObject.GetComponent<Button>();
            gameOverButtons[0].onClick.AddListener(() => StartEncounter());

            pcontroller = GameObject.FindFirstObjectByType<PlayerController>();

            StartEncounter();
        }
        /* else if (scene.name == prevScene)
        {
            GameManager.instance.LoadGame();
        } */
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
        encounterInProgress = false;
        StopAllCoroutines();
        for (int i = 0; i < projectiles.Count; i++)
        {
            Destroy(projectiles[i]);
        }
        projectiles.Clear();
        velocities.Clear();
        gameOverPrefab.SetActive(true);
        
        pcontroller.move.Disable();
    }

    private void FixedUpdate()
    {
        if (!encounterInProgress)
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
