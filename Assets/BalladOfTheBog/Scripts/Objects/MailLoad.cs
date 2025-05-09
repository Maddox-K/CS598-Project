using UnityEngine;

public class MailLoad : MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData data)
    {
        if (data._pickedUpLetter)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        Debug.Log("idk");
    }
}
