using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private Animator _sceneTransitionAnimator;

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

        StartCoroutine(TransitionScene());

        //SceneManager.LoadScene(_sceneToLoad);
    }

    private IEnumerator TransitionScene()
    {
        _sceneTransitionAnimator.SetTrigger("EndScene");

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(_sceneToLoad);
    }
}
