using UnityEngine;
using UnityEngine.UI;

public class MenuButtonSet : MonoBehaviour
{
    [SerializeField] protected Button _firstSelected;
    void OnEnable()
    {
        _firstSelected.Select();
    }

    void Start()
    {
        _firstSelected.transform.GetChild(0).gameObject.SetActive(true);
    }
}
