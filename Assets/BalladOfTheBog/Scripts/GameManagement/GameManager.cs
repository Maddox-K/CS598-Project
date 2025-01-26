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
        this._dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (scene.name != "BattleTest")
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
        //Debug.Log(gameData.playerPosition);
        //Debug.Log(gameData.encounterHappened);
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

    public void LoadGame()
    {
        Debug.Log("loading data");
        this.gameData = _dataHandler.Load();
        if (gameData == null)
        {
            Debug.Log("Initializing data to defaults");
            NewGame();
        }

        // push loaded to data to scripts in current scene that need it
        foreach (I_DataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            if (dataPersistenceObj != null)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }
    }

    public void SaveGame()
    {
        Debug.Log("saving data");

        gameData.lastScene = SceneManager.GetActiveScene().name;

        foreach (I_DataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        _dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

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
