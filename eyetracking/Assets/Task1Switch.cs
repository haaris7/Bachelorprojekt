using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Task1Switch : MonoBehaviour
{
    public NewDataLogger dataLogger;


    void Update()
    {
        if (!dataLogger.IsLogging && dataLogger.activeregion == 3)
        {
            SceneManager.LoadScene("Task2");
        }
    }
}
