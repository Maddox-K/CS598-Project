using UnityEngine;

[CreateAssetMenu(fileName = "ShopInventory", menuName = "Shop/ShopInventory")]
public class ShopInventory : ScriptableObject
{
    public GameObject[] shopSlots;
    public int[] prices;
    public string[] names;
    public string[] descriptions;
}
