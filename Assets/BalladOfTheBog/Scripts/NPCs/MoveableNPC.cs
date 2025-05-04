using UnityEngine;

public class MoveableNPC : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string _moveNPCID;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        _moveNPCID = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data)
    {
        if (data.npcPositions.ContainsKey(_moveNPCID))
        {
            Transform trans = GetComponent<Transform>();

            float[] coords = data.npcPositions[_moveNPCID];
            Vector3 temp = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                temp[i] = coords[i];
            }
            trans.position = temp;
        }
    }

    public void SaveData(GameData data)
    {
        Vector3 currentPos = GetComponent<Transform>().position;
        float[] coords = new float[3];
        coords[0] = currentPos.x;
        coords[1] = currentPos.y;
        coords[2] = 0.0f;

        if (data.npcPositions.ContainsKey(_moveNPCID))
        {
            data.npcPositions.Remove(_moveNPCID);
        }
        data.npcPositions.Add(_moveNPCID, coords);
    }
}
