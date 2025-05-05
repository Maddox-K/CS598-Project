using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    // Door Transport
    [SerializeField] protected Vector2 _teleportLocation;
    [SerializeField] protected Vector2 _lookDirectionOnTeleport;
    [SerializeField] protected Animator _transitionAnimator;

    // audio
    protected AudioSource _audioSource;
    [SerializeField] protected AudioClip _doorOpenSound;
    [SerializeField] protected UnityEvent musicChange;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public virtual void Interact()
    {
        if (_audioSource != null && _doorOpenSound != null)
        {
            _audioSource.volume = 0.09f;
            _audioSource.PlayOneShot(_doorOpenSound);
        }

        if (_transitionAnimator != null)
        {
            _transitionAnimator.SetTrigger("Start");
        }

        musicChange.Invoke();

        PlayerEvents.InvokeDoorOpen(this);
    }

    public Vector2 GetTeleport()
    {
        return _teleportLocation;
    }

    public Vector2 GetDirection()
    {
        return _lookDirectionOnTeleport;
    }
}
