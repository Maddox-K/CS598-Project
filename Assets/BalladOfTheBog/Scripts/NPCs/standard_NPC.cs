using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class standard_NPC : NPC, I_Talkable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private DialogueManager dialogueManager;

    public override void Interact()
    {
        Talk(dialogue);
    }

    public void Talk(Dialogue dialogue)
    {
        Debug.Log("Interact");
        dialogueManager.DisplayNext(dialogue);
    }
}
