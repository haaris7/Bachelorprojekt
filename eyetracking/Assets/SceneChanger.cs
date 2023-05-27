using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int currentSceneIndex = 0;

    void Start()
    {
        LoadNextScene();
    }

    void Update()
    {
        if(ShouldChangeScene())
        {
            LoadNextScene();
        }
    }

    public bool ShouldChangeScene()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Task1":
                GameObject courseObject = GameObject.Find("Course");
                if (courseObject != null)
                {
                    GameObject xrRigObject = courseObject.transform.Find("XRRig").gameObject;
                    if (xrRigObject != null)
                    {
                        NewDataLogger logger = xrRigObject.GetComponent<NewDataLogger>();
                        if (logger != null)
                        {
                            UnityEngine.Debug.Log("switch scene");
                            return !logger.IsLogging && logger.activeregion == 3;
                        }
                    }
                }
                break;

            default:
                return false;
        }

        return false;
    }

    public void LoadNextScene()
    {
        if(currentSceneIndex >= 4)
        {
            Application.Quit(); // Close the application when we've gone through all scenes
            return;
        }

        currentSceneIndex++;

        SceneManager.LoadScene("Task" + currentSceneIndex.ToString());


    }
}