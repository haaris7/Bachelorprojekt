using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeSwitcher : MonoBehaviour
{
    public float time = 0f;
    public string taskname = "";

    //private starttime;
    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > time)
        {
            if (taskname == "Exit")
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }

            else SceneManager.LoadScene(taskname);


        }
    }
}