using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewDataLogger : MonoBehaviour
{
    private char sep = ';';
    private string header = "GazeTime;Region;Target;PosX;PosY;PosZ;PlayerPosX;PlayerPosY;PlayerPosZ;LightIntensity;Velocity";
    private string filenameBase = "GazeData";
    private string sceneName = "";
    public string pathPrefix;
    private string path;
    public int activeregion = 0;
    public NewEyeTracker eyeTracker;
    private Vector2 prevTextureCoord;
    public bool IsLogging = false;

    private string counterFilePath; //"Assets/Scripts/Data/counter.txt";
    private int counter = 1;

    void Start()
    {
        counterFilePath = Path.Combine(Application.persistentDataPath, "counter.txt");
        pathPrefix = Application.persistentDataPath.ToString();
        sceneName = SceneManager.GetActiveScene().name;
        Directory.CreateDirectory(pathPrefix + sceneName);
        
        // Load the counter from a file.
        if (File.Exists(counterFilePath))
        {
            string counterString = File.ReadAllText(counterFilePath);
            if (!int.TryParse(counterString, out counter))
            {
                Debug.LogError("Could not parse counter file");
                counter = 1;
            }
        }

        path = genFileName();
        FileStream fs = File.Create(path);
        fs.Close();
        using (StreamWriter writer = File.AppendText(path))
        {
            writer.WriteLine(header);
        }
    }

    void Update()
    {
        if (eyeTracker.Check() && IsLogging)
        {
            Log(eyeTracker.gazePoint);
        }
    }

    public string genFileName()
    {
        string ret = "";

        // Loop until we find a file name that doesn't exist.
        while (true)
        {
            ret = pathPrefix + sceneName + "/" + filenameBase + "_" + counter + ".csv";
            if (!File.Exists(ret))
            {
                // If the file doesn't exist, break the loop.
                break;
            }
            counter++;
        }

        // Save the counter back to a file.
        File.WriteAllText(counterFilePath, counter.ToString());

        return ret;
    }

    public string DetermineTarget()
    {
        if (eyeTracker.TargetName != "")
        {
            string txt = eyeTracker.TargetName;
            eyeTracker.TargetName = "";
            return txt;
        }
        else
        {
            return "None";
        }
    }

    public void Log(Vector3 pos)
    {
        string line = eyeTracker.duration.ToString() + sep;
        line += activeregion.ToString() + sep;
        line += DetermineTarget() + sep;
        line += pos.x.ToString() + sep + pos.y.ToString() + sep + pos.z.ToString() + sep;
        line += transform.position.x.ToString() + sep + transform.position.y.ToString() + sep + transform.position.z.ToString() + sep;
        line += eyeTracker.intensity.ToString() + sep;
        line += eyeTracker.velocity.ToString() + sep;

        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(line);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Start")
        {
            IsLogging = true;
            activeregion++;
        }
        else if (other.tag == "Stop")
        {
            IsLogging = false;
        }
    }




    
}
