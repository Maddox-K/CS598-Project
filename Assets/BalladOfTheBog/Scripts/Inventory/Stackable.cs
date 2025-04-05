using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stackable : MonoBehaviour
{
    public string itemName;
    [SerializeField] public int quantity = 1;
    [SerializeField] public int maxStack = 5;

    [SerializeField] private TextMeshProUGUI quantityText;

    private void Start()
    {
        UpdateQuantityText();
    }

    public void AddOne()
    {
        quantity++;
        UpdateQuantityText();
    }

    public void RemoveOne()
    {
        quantity--;
        UpdateQuantityText();

        if (quantity <= 0)
        {
            Destroy(gameObject); // removes UI item
        }
    }

    public bool CanStack()
    {
        return quantity < maxStack;
    }

    private void UpdateQuantityText()
    {
        if (quantityText != null)
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }
}
