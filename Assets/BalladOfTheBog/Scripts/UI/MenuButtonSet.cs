using UnityEngine;
using UnityEngine.UI;

public class MenuButtonSet : MonoBehaviour
{
    [SerializeField] private Button _firstSelected;
    void OnEnable()
    {
        _firstSelected.Select();
    }
}
