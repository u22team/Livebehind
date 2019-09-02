using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAnyKey : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
            SceneManager.LoadScene("StageSelect");

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                SceneManager.LoadScene("StageSelect");
            }
        }
    }
}
