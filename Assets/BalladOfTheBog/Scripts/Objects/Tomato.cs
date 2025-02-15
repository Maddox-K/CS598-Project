using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tomato : MonoBehaviour
{
    [SerializeField] private Button tomatoButton;
    [SerializeField] private GameObject tomatoPrefab;

    private GameObject player;
    private PlayerController playerController;

    private float duration = 5.0f;
    private float currentTime;

    private void Start()
    {
        currentTime = duration;

        tomatoButton.onClick.AddListener(() =>
        {
            player = GameObject.Find("Player");
            playerController = player.GetComponent<PlayerController>();
            playerController.speed = 10.0f;

            Destroy(tomatoPrefab);
            //Destroy()
        });
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0) 
        {
            RemoveEffect();
        }
    }

    private void RemoveEffect()
    {
        playerController = player.GetComponent<PlayerController>();
        playerController.speed = 5.0f;
    }
}
