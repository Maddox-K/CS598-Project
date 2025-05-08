using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneChanger : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _sceneWipeAnimator;
    [SerializeField] private float _animationDelay;
    [SerializeField] private AudioSource _audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DelaySceneChange());
        StartCoroutine(DelayAnimation());
    }

    private IEnumerator DelaySceneChange()
    {
        StartCoroutine(FadeOutMusic(_audioSource, time));

        yield return new WaitForSeconds(time);

        _sceneWipeAnimator.SetTrigger("EndScene");
        yield return new WaitForSeconds(1.3f);

        SceneManager.LoadScene("Home");
    }

    private IEnumerator DelayAnimation()
    {
        yield return new WaitForSeconds(_animationDelay);

        _animator.SetTrigger("Trigger");
    }

    private IEnumerator FadeOutMusic(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Reset volume for next use
    }
}
