using System;
using System.IO;
using UnityEngine;

public class NewDataLogger : MonoBehaviour
{
    private char sep = ';';
    private string header = "GazeTime;Region;Target;PosX;PosY;PosZ;PlayerPosX;PlayerPosY;PlayerPosZ;Eccentricity;Proximity";
    private string filenameBase = "GazeData";
    public string pathPrefix = "Assets/Scripts/Data/";
    private string path;
    public int activeregion = 0;
    public NewEyeTracker eyeTracker;
    public string currentFileName = "";
    private Vector2 prevTextureCoord;
    public bool IsLogging = false;
    public float eccentricity;
    public float proximity;

    void Start()
    {
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
        int counter = 1;
        string ret = pathPrefix + filenameBase + counter + ".csv";
        string[] dir = Directory.GetFiles(pathPrefix);
        if (dir.Length != 0)
        {
            foreach (string item in dir)
            {
                ret = pathPrefix + filenameBase + counter + ".csv";
                if (item == pathPrefix + filenameBase + counter + ".csv")
                {
                    counter++;
                }
            }
        }
        currentFileName = filenameBase + "(" + counter + ")_";
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

        // Calculate the eccentricity and proximity and log them.
        Vector3 eyePosition = eyeTracker.GetEyePosition();
        Vector3 objectDirection = (pos - eyePosition).normalized;
        Vector3 gazeDirection = (eyeTracker.gazePoint - eyePosition).normalized;
        eccentricity = Vector3.Angle(gazeDirection, objectDirection);
        proximity = Vector3.Distance(eyePosition, pos);
        line += eccentricity.ToString() + sep + proximity.ToString();

        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(line);
        }
        // UnityEngine.Debug.Log(line);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Start")
        {
            IsLogging = true;
            // UnityEngine.Debug.Log("Enter");

            activeregion++;
        }
        else if (other.tag == "Stop")
        {
            // UnityEngine.Debug.Log("Exit");
            IsLogging = false;
        }
    }
}
