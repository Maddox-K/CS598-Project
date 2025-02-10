using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private Animator _sceneTransitionAnimator;

    public void Interact()
    {
        if (PlayerPrefs.GetInt("AutoSave") == 1)
        {
            GameManager.instance.SaveGame(false);
        }
        else
        {
            GameManager.instance.SaveGame();
        }

        StartCoroutine(TransitionScene());
    }

    private IEnumerator TransitionScene()
    {
        _sceneTransitionAnimator.SetTrigger("EndScene");

        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene(_sceneToLoad);
    }
}
