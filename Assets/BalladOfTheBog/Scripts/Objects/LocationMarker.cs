using UnityEngine;

public class LocationMarker : MonoBehaviour
{
    [SerializeField] private string _locationName;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            QuestEvents.OnLocationVisited?.Invoke(_locationName);
        }
    }
}
