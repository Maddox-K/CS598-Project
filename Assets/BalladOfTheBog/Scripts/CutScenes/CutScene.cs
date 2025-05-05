using UnityEngine;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    [SerializeField] private string _activationConditionId;
    [SerializeField] private bool _initiatesInteration;
    [SerializeField] private GameObject _interactionSubject;
    private Transform _subjectTransform;
    private Transform _subjectParentTransform;
    private Transform _playerTransform;
    [SerializeField] private bool _matchPlayerX;
    [SerializeField] private bool _matchPlayerY;
    private PlayableDirector _director;

    void Awake()
    {
        _director = gameObject.GetComponent<PlayableDirector>();

        if (_matchPlayerX || _matchPlayerY)
        {
            if (_interactionSubject != null)
            {
                _subjectTransform = _interactionSubject.transform;
                _subjectParentTransform = _interactionSubject.transform.parent;
            }
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
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
            if (_matchPlayerX)
            {
                Vector3 playerPos = _playerTransform.position;
                Vector3 currParentPos = _subjectParentTransform.position;
                Vector3 currPos = _subjectTransform.position;
                _subjectParentTransform.position = new Vector3(playerPos.x - currPos.x, currParentPos.y, currParentPos.z);
            }
            else if (_matchPlayerY)
            {
                Vector3 playerPos = _playerTransform.position;
                Vector3 currParentPos = _subjectParentTransform.position;
                Vector3 currPos = _subjectTransform.position;
                _subjectParentTransform.position = new Vector3(currParentPos.x, playerPos.y - currPos.y, currParentPos.z);
            }

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
