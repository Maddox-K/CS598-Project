using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string _dataDirectoryPath = "";
    private string _dataFileName = "";

    public FileDataHandler(string dataDirectoryPath, string dataFileName)
    {
        this._dataDirectoryPath = dataDirectoryPath;
        this._dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(_dataDirectoryPath, _dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // de-serialize the data from json back into GameData object
                loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to load the data from file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(_dataDirectoryPath, _dataFileName);
        try
        {
            // create the directory the file will be written to if it does not exist already
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize the game data object into json
            //string dataToStore = JsonUtility.ToJson(data, true);
            string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);

            //write the file to filesystem
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
