using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] GameObject uiContainer = null; 

    void Start()
    {
        uiContainer.SetActive(false);
    }
    public void GetUpdate()
    {
        uiContainer.SetActive(!uiContainer.activeSelf);
    }
}
