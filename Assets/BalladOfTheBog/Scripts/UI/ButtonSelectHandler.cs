using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        _highlighter.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _highlighter.SetActive(false);
    }
}
