using UnityEngine;
using UnityEngine.UI;

public class MenuButtonSet : MonoBehaviour
{
    [SerializeField] protected Button _firstSelected;
    void OnEnable()
    {
        _firstSelected.Select();
    }
}
