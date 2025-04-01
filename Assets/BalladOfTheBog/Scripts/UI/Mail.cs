using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mail : MonoBehaviour
{
    [SerializeField] private Button mailButton;
    [SerializeField] private GameObject mailGameObject;
    [SerializeField] private GameObject mailText;
    [SerializeField] private Animator openMailAnimation;

    private void Start()
    {

        //mailButton.onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mailText.SetActive(true);
    }
}