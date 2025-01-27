using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // data handling, save and load
    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    private FileDataHandler _dataHandler;
    private List<I_DataPersistence> dataPersistenceObjects;
    public GameData gameData;

    // singleton class pattern
    public static GameManager instance { get; private set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleTest")
        {
            return;
        }

        this._dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (scene.name == "Main Menu")
        {
            LoadGame(false);
        }
        else
        {
            LoadGame();
        }
    }

    private void Awake()
    {
        if (instance != null) {
            Debug.Log("Found more than one GameManager in the scene");
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (gameData == null)
        {
            gameData = new GameData();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        //LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    // optional bool determines if the loading is "temporary" or not
    // temp load means that the game is loading data from the current state of gameData as opposed to gameData recorded in disk
    // used for things like scene transitions when autosave is turned off
    public void LoadGame(bool isTemp = true)
    {
        Debug.Log("loading data");
        if (isTemp == false)
        {
            this.gameData = _dataHandler.Load();
        }

        if (gameData == null)
        {
            Debug.Log("Initializing data to defaults");
            NewGame();
        }

        // push loaded data to scripts in current scene that need it
        foreach (I_DataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            if (dataPersistenceObj != null)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
    }

    // isTemp performs the same function as in LoadGame()
    // temp save is one that overwrites gameData object but does not overwrite gameData save file stored on disk
    public void SaveGame(bool isTemp = true)
    {
        Debug.Log("saving data");

        gameData.lastScene = SceneManager.GetActiveScene().name;

        foreach (I_DataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        if (isTemp == false)
        {
            _dataHandler.Save(gameData);
        }
    }

    /* private void OnApplicationQuit()
    {
        SaveGame();
    } */

    private List<I_DataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<I_DataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<I_DataPersistence>();

        return new List<I_DataPersistence>(dataPersistenceObjects);
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }
}
