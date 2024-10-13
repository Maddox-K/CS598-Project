using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get; private set; }
    public GameData gameData;
    private List<I_DataPersistence> dataPersistenceObjects;

    private void Awake()
    {
        if (instance != null) {
            Debug.LogError("Found more than one GameManager in the scene");
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
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    /* public void NewGame()
    {
        this.gameData = new GameData();
    } */

    public void LoadGame()
    {
        /* if (gameData == null)
        {
            NewGame();
        } */

        foreach (I_DataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (I_DataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
    }

    private List<I_DataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<I_DataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<I_DataPersistence>();

        return new List<I_DataPersistence>(dataPersistenceObjects);
    }
}
