using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private Animator _sceneTransitionAnimator;
    [SerializeField] private float[] _sceneCoordinates = new float[2];
    [SerializeField] private float[] _sceneDirection = new float[2];
    private bool _canInteract = true;

    public void Interact()
    {
        if (!_canInteract)
        {
            return;
        }

        _canInteract = false;

        GameData data = GameManager.instance.gameData;

        GameManager.instance.SaveGame();

        data.changingScenes = true;
        for (int i = 0; i < 2; i++)
        {
            data.playerPosition[i] = _sceneCoordinates[i];
            data.playerRotation[i] = _sceneDirection[i];
        }

        StartCoroutine(TransitionScene());
    }

    private IEnumerator TransitionScene()
    {
        _sceneTransitionAnimator.SetTrigger("EndScene");

        yield return new WaitForSeconds(1.4f);

        SceneManager.LoadScene(_sceneToLoad);
    }
}
