using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavesMenuController : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas savesMenu;

    [SerializeField] private Button returnToMainMenu;

    // Start is called before the first frame update
    void Start()
    {
        returnToMainMenu.onClick.AddListener(SwitchToMainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool showMainMenu;
    private void SwitchToMainMenu()
    {
        if (!showMainMenu)
        {
            savesMenu.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
        }
    }
}
