using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{

    public GameObject joyPublisher;
    

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.J))
        {
            joyPublisher.SetActive(!joyPublisher.activeSelf);
        }

        if(Input.GetKeyUp(KeyCode.R))
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }
}
