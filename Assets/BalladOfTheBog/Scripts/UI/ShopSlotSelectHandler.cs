using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopSlotSelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ShopMenuController _shopMenu;
    private TextMeshProUGUI _description;
    private TextMeshProUGUI _price;

    [SerializeField] private int _slotNumber;

    void Awake()
    {
        _shopMenu = gameObject.transform.parent.gameObject.transform.parent.gameObject.GetComponent<ShopMenuController>();
        if (_shopMenu != null)
        {
            _description = _shopMenu.GetDescriptionBox();
            _price = _shopMenu.GetPriceBox();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_shopMenu.GetDescriptionBox() != null)
        {
            SetDescriptionAndPrice();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ClearDescription();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetDescriptionAndPrice();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClearDescription();
    }

    private void SetDescriptionAndPrice()
    {
        _description.text = _shopMenu.currentShopDescriptions[_slotNumber];
        _price.text = "x" + _shopMenu.currentShopPrices[_slotNumber];
    }

    private void ClearDescription()
    {
        _description.text = "";
        _price.text = "x";
    }
}
