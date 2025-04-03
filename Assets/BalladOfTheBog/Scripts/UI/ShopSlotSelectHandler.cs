using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopSlotSelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ShopMenuController _shopMenu;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private int _slotNumber;

    void Awake()
    {
        _shopMenu = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<ShopMenuController>();
        if (_shopMenu != null)
        {
            _description = _shopMenu.GetDescription();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_shopMenu.GetDescription() != null)
        {
            SetDescription();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ClearDescription();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClearDescription();
    }

    private void SetDescription()
    {
        _description.text = _shopMenu.currentShopDescriptions[_slotNumber];
    }

    private void ClearDescription()
    {
        _description.text = "";
    }
}
