using System;
using System.IO;
using UnityEngine;

public class NewDataLogger : MonoBehaviour
{
    private char sep = ';';
    private string header = "Timestamp;Region;Target;TextureCoordX;TextureCoordY";
    private string filenameBase = "GazeData";
    public string pathPrefix = "Assets/Scripts/Data/";
    private string path;
    public int activeregion = 0;
    public NewEyeTracker eyeTracker;
    public string currentFileName = "";
    private Vector2 prevTextureCoord;

    void Start()
    {
        path = genFileName();
        FileStream fs = File.Create(path);
        fs.Close();
        using (StreamWriter writer = File.AppendText(path))
        {
            writer.WriteLine(header);
        }
        prevTextureCoord = eyeTracker.GetTextureCoord(new Ray(eyeTracker.transform.position, eyeTracker.transform.forward));
    }

    void Update()
    {
        Vector2 curTextureCoord = eyeTracker.GetTextureCoord(new Ray(eyeTracker.transform.position, eyeTracker.transform.forward));
        if (curTextureCoord != prevTextureCoord)
        {
            Log(curTextureCoord);
            prevTextureCoord = curTextureCoord;
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

    public void Log(Vector2 textureCoord)
    {
        string line = Time.time.ToString() + sep;
        line += activeregion + sep;
        line += DetermineTarget() + sep;
        line += textureCoord.x + sep + textureCoord.y;
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(line);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Start")
        {
            activeregion++;
        }
    }
}
