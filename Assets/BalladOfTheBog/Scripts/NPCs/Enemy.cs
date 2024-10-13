using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : NPC, I_DataPersistence
{
    [SerializeField] private Dialogue predialogue;
    [SerializeField] private Dialogue postdialogue;
    [SerializeField] public EnemyAttacks enemyAttacks;
    [SerializeField] private EncounterManager encounterManager;
    [SerializeField] private DialogueManager dialogueManager;
    private bool encounterHappened;

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
        //encounterHappened = data.
    }

    public void SaveData(GameData data)
    {

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