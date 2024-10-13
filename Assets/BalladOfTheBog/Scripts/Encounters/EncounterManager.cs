using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager instance { get; private set; }
    private Enemy currentEnemy;
    private const int spawnDelay = 2;
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
        currentEnemy = enemy;
        GameManager.instance.SaveGame();
        SceneManager.LoadScene("BattleTest");
    }

    private void StartEncounter(Enemy enemy)
    {
        // code here to handle aspects of encounter initialization
        StartCoroutine(DelaySpawns(enemy));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleTest")
        {
            StartEncounter(currentEnemy);
        }
    }

    IEnumerator DelaySpawns(Enemy enemy)
    {
        yield return new WaitForSeconds(spawnDelay);

        StartCoroutine(SpawnProjectiles(enemy.enemyAttacks));
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

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = directions[i].normalized * speeds[i];
            }
        }
    }

    /* public void StartDialogue(Enemy enemy)
    {
        if (dialogueStarted && dialogueManager.paragraphs.Count == 0)
        {
            StartEncounter(enemy);
        }
        if (dialogueManager.paragraphs.Count == 0 && !dialogueStarted)
        {
            dialogueStarted = true;
            dialogueManager.DisplayNext(enemy.predialogue);
        }
        
        GameManager.instance.SaveGame();
        SceneManager.LoadScene("BattleTest");
    } */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
