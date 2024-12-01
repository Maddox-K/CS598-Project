
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button savesButton;
    [SerializeField] private Button settigsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settingsMenu;
    [SerializeField] private Canvas savesMenu;

    private UnityAction OpenSettingsMenu;

    private void Awake()
    {
        
    }
    void Start()
    {
        startButton.onClick.AddListener(LoadGameScene);
        settigsButton.onClick.AddListener(SwitchToSettingsMenu);
        savesButton.onClick.AddListener(SwitchToSavesMenu);
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Scene1");
        //SceneManager.UnloadSceneAsync("Main Menu");
    }

    bool showSettingsMenu;
    private void SwitchToSettingsMenu()
    {
        if(!showSettingsMenu)
        {
            settingsMenu.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(false);
        }
    }

    bool showSavesMenu;
    private void SwitchToSavesMenu()
    {
        if (!showSavesMenu)
        {
            savesMenu.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(false);
        }
    }

}
