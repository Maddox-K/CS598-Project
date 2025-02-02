using UnityEngine;

public class StandardNPC : NPC
{
    [SerializeField] private Dialogue _dialogue;

    public override void Interact()
    {
        Talk(_dialogue);
    }

    public void Talk(Dialogue dialogue)
    {
        dialogueManager.DisplayNext(dialogue);
    }
}
