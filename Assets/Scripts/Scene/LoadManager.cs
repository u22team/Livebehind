using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
