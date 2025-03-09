using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private GameObject _highlighter;

    void Awake()
    {
        _highlighter = transform.GetChild(0).gameObject;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _highlighter.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _highlighter.SetActive(false);
    }
}
