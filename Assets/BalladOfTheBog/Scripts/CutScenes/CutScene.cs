using UnityEngine;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    [SerializeField] private string _activationConditionId;
    [SerializeField] private bool _initiatesInteration;
    [SerializeField] private GameObject _interactionSubject;
    private PlayableDirector _director;

    void Awake()
    {
        _director = gameObject.GetComponent<PlayableDirector>();
    }

    void OnEnable()
    {
        CutSceneEvents.OnLocationEntered += OnLocationEntered;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_director != null)
        {
            _director.stopped += OnCutSceneFinished;
        }
    }

    void OnDisable()
    {
        CutSceneEvents.OnLocationEntered -= OnLocationEntered;
        if (_director != null)
        {
            _director.stopped -= OnCutSceneFinished;
        }
    }

    private void OnLocationEntered(string locationID)
    {
        if (locationID == _activationConditionId)
        {
            PlayerEvents.InvokeDeactivate(2);
            PauseEvents.InvokeDisablePopup(2);

            _director.Play();
        }
    }

    private void OnCutSceneFinished(PlayableDirector director)
    {
        if (_initiatesInteration)
        {
            PlayerEvents.InvokeActivate(0);
            _interactionSubject.GetComponent<IInteractable>().Interact();
        }
        else
        {
            PlayerEvents.InvokeActivate(2);
        }

        if (_director != null)
        {
            _director.stopped -= OnCutSceneFinished;
        }
    }
}
