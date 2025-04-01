using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        GetComponent<Button>().Select();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _highlighter.SetActive(false);

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
