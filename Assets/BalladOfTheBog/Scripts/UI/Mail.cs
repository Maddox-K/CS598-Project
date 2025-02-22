using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mail : MonoBehaviour
{
    [SerializeField] private GameObject MailText;
    [SerializeField] private Button MailButton;

    private void Start()
    {
        MailButton.onClick.AddListener(OnButtonPress);
    }

    private void OnButtonPress()
    {

        StartCoroutine(DisplayText());
    }

    private IEnumerator DisplayText()
    {
        yield return MailText;
    }

}
