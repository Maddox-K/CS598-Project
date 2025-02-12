using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerSet : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            _animator.SetBool("StartNow", false);
        }
        else
        {
            _animator.SetBool("StartNow", true);
        }
    }
}
