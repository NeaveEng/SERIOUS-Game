using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.visible = false;
            Debug.Log("Application is focused");
        }
        else
        {
            Debug.Log("Application lost focus");
        }
    }
}