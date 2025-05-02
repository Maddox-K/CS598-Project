using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector2 _teleportLocation;
    [SerializeField] private Vector2 _lookDirectionOnTeleport;
    [SerializeField] private Animator _transitionAnimator;

    // audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _doorOpenSound;

    [SerializeField] private UnityEvent musicChange;

    void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Interact()
    {
        if (_audioSource != null && _doorOpenSound != null)
        {
            _audioSource.PlayOneShot(_doorOpenSound);
        }

        if (_transitionAnimator != null)
        {
            _transitionAnimator.SetTrigger("Start");
            musicChange.Invoke();
        }
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
