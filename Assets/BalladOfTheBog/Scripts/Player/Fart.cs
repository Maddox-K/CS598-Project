using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fart : MonoBehaviour
{

    [SerializeField] GameObject Character;
    UnityEvent FartBig;

    private void Awake()
    {
        
    }

    // Start is called before the first frame updat

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Character.activeInHierarchy)
        {
            FartBig.Invoke();
        }
    }
}
