using UnityEngine;

public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] protected DialogueManager dialogueManager;
    public abstract void Interact();
}
