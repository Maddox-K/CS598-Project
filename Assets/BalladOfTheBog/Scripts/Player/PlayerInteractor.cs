using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputInitializer))]
public class PlayerInteractor : MonoBehaviour
{
    private string _currentScene;

    // Interaction
    private GameObject _interactSprite;
    private List<Collider2D> _thingsInRange = new List<Collider2D>();
    private GameObject _closestObject;

    // Input
    private PlayerInputInitializer _inputInitializer;
    private InputAction _interact;

    // Lifecycle Methods
    private void Awake()
    {
        _inputInitializer = GetComponent<PlayerInputInitializer>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        _interact = _inputInitializer.Interact;
    }

    void Update()
    {
        if (_thingsInRange.Count > 0)
        {
            _closestObject = GetClosestObject();
        }
        else
        {
            _closestObject = null;
            if (_interactSprite != null)
            {
                _interactSprite.SetActive(false);
                _interactSprite = null;
            }
        }

        if (_closestObject != null && _interact.WasPressedThisFrame())
        {
            switch (_closestObject.tag)
            {
                case "SNPC":
                    _closestObject.GetComponent<StandardNPC>().Interact();
                    break;
                case "ShopNPC":
                    _closestObject.GetComponent<ShopNPC>().Interact();
                    break;
                case "Enemy":
                    _closestObject.GetComponent<Enemy>().Interact();
                    break;
                case "Door":
                    _closestObject.GetComponent<Door>().Interact();
                    break;
                case "SceneChange":
                    _closestObject.GetComponent<SceneChange>().Interact();
                    break;
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Methods
    private void OnTriggerEnter2D(Collider2D collider)
    {
        _thingsInRange.Add(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        _thingsInRange.Remove(collider);

        if (_thingsInRange.Count == 0)
        {
            _closestObject = null;
        }
    }

    GameObject GetClosestObject()
    {
        if (_currentScene == "BattleTest")
        {
            return null;
        }

        Collider2D closest = null;
        float closestDistanceSqr = float.MaxValue;
        Vector3 playerPosition = transform.position;

        foreach (Collider2D obj in _thingsInRange)
        {
            float sqrDistance = (obj.transform.position - playerPosition).sqrMagnitude;
            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closest = obj;
            }
        }

        GameObject closestObj = closest.gameObject;

        if (closestObj.CompareTag("SNPC") || closestObj.CompareTag("Enemy") || closestObj.CompareTag("Door") || closestObj.CompareTag("SceneChange") || closestObj.CompareTag("ShopNPC"))
        {
            if (closestObj.transform.childCount > 0)
            {
                GameObject thisInteract = closestObj.transform.GetChild(0).gameObject;
                if (_interactSprite == null)
                {
                    _interactSprite = thisInteract;
                    _interactSprite.SetActive(true);
                }
                else if (_interactSprite != thisInteract)
                {
                    _interactSprite.SetActive(false);
                    _interactSprite = thisInteract;
                    _interactSprite.SetActive(true);
                }
            }
        }
        else if (_interactSprite != null)
        {
            _interactSprite.SetActive(false);
            _interactSprite = null;
        }

        return closestObj;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;
    }
}
