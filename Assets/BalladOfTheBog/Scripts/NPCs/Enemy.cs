using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : NPC, I_DataPersistence
{
    [SerializeField] private string id;
    [SerializeField] private Dialogue predialogue;
    [SerializeField] private Dialogue postdialogue;
    [SerializeField] public EnemyAttacks enemyAttacks;
    [SerializeField] private EncounterManager encounterManager;
    [SerializeField] private DialogueManager dialogueManager;
    private bool encounterHappened;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public override void Interact()
    {
        if (encounterHappened == true && postdialogue != null)
        {
            dialogueManager.DisplayNext(postdialogue);
        }
        else if (predialogue == null)
        {
            encounterHappened = true;
            encounterManager.EncounterInit(this);
        }
        else if (dialogueManager.paragraphs.Count > 0 && dialogueManager.paragraphs.Peek() == "[encounter]")
        {
            encounterHappened = true;
            encounterManager.EncounterInit(this);
        }
        else
        {
            dialogueManager.DisplayNext(predialogue);
        }
    }

    public void LoadData(GameData data)
    {
        data.enemiesEncountered.TryGetValue(id, out encounterHappened);
        if (encounterHappened == true && data.lastEnemyEncountered == id && postdialogue != null)
        {
            Interact();
        }
        //this.encounterHappened = data.encounterHappened;
    }

    public void SaveData(GameData data)
    {
        if (data.enemiesEncountered.ContainsKey(id))
        {
            data.enemiesEncountered.Remove(id);
        }
        data.enemiesEncountered.Add(id, encounterHappened);
        data.lastEnemyEncountered = id;
        //data.encounterHappened = this.encounterHappened;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}