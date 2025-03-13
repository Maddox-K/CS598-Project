using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // debugging
    [Header("Debugging")]
    [SerializeField] public bool savingLoadingEnabled = true;

    // data handling, save and load
    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    public FileDataHandler _dataHandler;
    private List<IDataPersistence> dataPersistenceObjects;
    public GameData gameData;
    public string _selectedProfileId = "";

    // singleton class pattern
    public static GameManager instance { get; private set; }


    // instance member functions
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

        _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Do not perform loading actions when going into an encounter
        if (scene.name == "BattleTest")
        {
            return;
        }

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (scene.name == "Main Menu")
        {
            _selectedProfileId = _dataHandler.GetMostRecentlyUpdatedProfileId();

            // case that save data was found
            if (_selectedProfileId != null)
            {
                LoadGame(false);
            }
        }
        else if (gameData.changingScenes || gameData.lastEnemyEncountered != null)
        {
            LoadGame();

            gameData.changingScenes = false;
            if (PlayerPrefs.GetInt("AutoSave") == 1)
            {
                SaveGame(false);
            }
            else
            {
                SaveGame();
            }
        }
        else
        {
            LoadGame();
        }
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        _selectedProfileId = newProfileId;

        LoadGame(false);
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    // optional bool determines if the loading is "temporary" or not
    // temp load means that the game is loading data from the current state of gameData as opposed to gameData recorded in disk
    // used for things like scene transitions when autosave is turned off
    public void LoadGame(bool isTemp = true)
    {
        if (!savingLoadingEnabled)
        {
            return;
        }

        Debug.Log("loading data");
        if (!isTemp)
        {
            gameData = _dataHandler.Load(_selectedProfileId);
        }

        if (gameData == null)
        {
            Debug.Log("Initializing data to defaults");
            NewGame();
        }

        // push loaded data to scripts in current scene that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
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
        if (!savingLoadingEnabled)
        {
            return;
        }

        Debug.Log("saving data");
        gameData.lastScene = SceneManager.GetActiveScene().name;

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        if (!isTemp)
        {
            gameData.lastUpdated = System.DateTime.Now.ToBinary();
            _dataHandler.Save(gameData, _selectedProfileId);
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return _dataHandler.LoadAllProfiles();
    }

    public bool HasGameData()
    {
        return gameData != null;
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
