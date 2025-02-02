using UnityEngine;

public class Enemy : NPC, IDataPersistence
{
    [SerializeField] private string _id;
    [SerializeField] private Dialogue _preEncounterDialogue;
    [SerializeField] private Dialogue _postEncounterDialogue;
    [SerializeField] public EnemyAttacks enemyAttacks;
    private bool _encounterHappened;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        _id = System.Guid.NewGuid().ToString();
    }

    public string Id
    {
        get { return _id; }
    }

    public override void Interact()
    {
        if (_encounterHappened && _postEncounterDialogue != null)
        {
            dialogueManager.DisplayNext(_postEncounterDialogue);
        }
        else if ((_preEncounterDialogue == null) || (dialogueManager.paragraphs.Count > 0 && dialogueManager.paragraphs.Peek() == "[encounter]"))
        {
            EncounterManager.instance.EncounterInit(this);
        }
        else
        {
            dialogueManager.DisplayNext(_preEncounterDialogue);
        }
    }

    public void LoadData(GameData data)
    {
        data.enemiesEncountered.TryGetValue(_id, out _encounterHappened);
        if (_encounterHappened == true && data.lastEnemyEncountered == _id)
        {
            data.lastEnemyEncountered = null;
            if (_postEncounterDialogue != null)
            {
                Interact();
            }
        }

    }

    public void SaveData(GameData data)
    {
        if (data.enemiesEncountered.ContainsKey(_id))
        {
            data.enemiesEncountered.Remove(_id);
        }
        data.enemiesEncountered.Add(_id, _encounterHappened);
    }
}
