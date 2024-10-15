using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager instance { get; private set; }
    [SerializeField] private GameObject gameOverPrefab;
    public string prevScene;
    private bool encounterInProgress;
    public Enemy currentEnemy;
    private EnemyAttacks eAttacks;
    private const int spawnDelay = 1;
    private List<Rigidbody2D> projectiles = new List<Rigidbody2D>();
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
        encounterInProgress = true;
        currentEnemy = enemy;
        eAttacks = enemy.enemyAttacks;
        prevScene = enemy.gameObject.scene.name;
        GameManager.instance.SaveGame();
        SceneManager.LoadScene("BattleTest");
    }

    private void StartEncounter(Enemy enemy)
    {
        // code here to handle aspects of encounter initialization
        StartCoroutine(DelaySpawns(enemy));
        StartCoroutine(EndBattle(enemy.enemyAttacks));
    }

    IEnumerator EndBattle(EnemyAttacks enemyAttacks)
    {
        yield return new WaitForSeconds(enemyAttacks.endTime);

        encounterInProgress = false;
        SceneManager.LoadScene(prevScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleTest")
        {
            StartEncounter(currentEnemy);
        }
        /* else if (scene.name == prevScene)
        {
            GameManager.instance.LoadGame();
        } */
    }

    IEnumerator DelaySpawns(Enemy enemy)
    {
        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnProjectiles(eAttacks));
    }

    IEnumerator SpawnProjectiles(EnemyAttacks enemyAttacks)
    {
        GameObject proj = enemyAttacks.projectile;
        Vector2[] positions = enemyAttacks.spawnPositions;
        Vector2[] directions = enemyAttacks.launchDirections;
        float[] times = enemyAttacks.launchTimes;
        float[] speeds = enemyAttacks.projectileSpeeds;

        for (int i = 0; i < positions.Length; i++)
        {
            yield return new WaitForSeconds(times[i]);

            GameObject projectile = Instantiate(proj, (Vector3)positions[i], Quaternion.identity);
            projectiles.Add(projectile.GetComponent<Rigidbody2D>());
            velocities.Add(directions[i].normalized * speeds[i]);
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();

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
                projectiles[i].velocity = velocities[i];
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
