using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneToLoad;

    /* void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
 */
    public void Interact()
    {
        if (GameManager.instance.gameData.autoSave)
        {
            GameManager.instance.SaveGame(false);
        }
        else
        {
            GameManager.instance.SaveGame();
        }

        SceneManager.LoadScene(_sceneToLoad);
    }
}
