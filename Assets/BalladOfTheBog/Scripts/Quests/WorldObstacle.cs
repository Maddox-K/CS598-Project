using UnityEngine;

public class WorldObstacle : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string _id;
    [SerializeField] private string _targetID;
    private bool _isCleared;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        _id = System.Guid.NewGuid().ToString();
    }

    void OnEnable()
    {
        QuestEvents.ClearObstacle += ClearObstacle;
    }

    void OnDisable()
    {
        QuestEvents.ClearObstacle -= ClearObstacle;
    }

    private void ClearObstacle(string questID)
    {
        if (questID == _targetID)
        {
            gameObject.SetActive(false);
        }
    }

    public void LoadData(GameData data)
    {
        data.clearedObstacles.TryGetValue(_id, out _isCleared);

        if (_isCleared)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.clearedObstacles.ContainsKey(_id))
        {
            data.clearedObstacles.Remove(_id);
        }
        data.clearedObstacles.Add(_id, _isCleared);
    }
}
