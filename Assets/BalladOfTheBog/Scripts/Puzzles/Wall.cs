using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] PuzzleZone _puzzleZone;

    private Transform _transform;
    private Vector3 _position;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _position = _transform.position;

        _puzzleZone.puzzleContents.Add(Vector2Int.RoundToInt(_position), (_transform, false));
    }
}
