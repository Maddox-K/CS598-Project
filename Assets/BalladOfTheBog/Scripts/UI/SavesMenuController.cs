using UnityEngine;
using UnityEngine.UI;

public class SavesMenuController : MonoBehaviour
{
    // Other Menus
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas savesMenu;

    [SerializeField] private Button returnToMainMenu;

    // Start is called before the first frame update
    void Start()
    {
        returnToMainMenu.onClick.AddListener(SwitchToMainMenu);
    }

    private void SwitchToMainMenu()
    {
        savesMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }
}
