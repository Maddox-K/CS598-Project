using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector2 _teleportLocation;
    [SerializeField] private Animator _transitionAnimator;

    // audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _doorOpenSound;

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
        }
    }

    public Vector2 GetTeleport()
    {
        return _teleportLocation;
    }
}
