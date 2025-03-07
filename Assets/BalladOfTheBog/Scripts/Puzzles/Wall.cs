using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] PuzzleZone _puzzleZone;

    private Transform _transform;
    private Vector3 _position;
    private Vector3 _initialPosition;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _position = _transform.position;
        _initialPosition = _position;

        _puzzleZone.puzzleContents.Add(Vector2Int.RoundToInt(_position), (_transform, false));
        _puzzleZone.startContents.Add(Vector2Int.RoundToInt(_position), (_transform, false));
    }

    public Vector3 GetInitialPosition()
    {
        return _initialPosition;
    }
}
