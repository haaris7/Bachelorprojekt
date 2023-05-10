using System;
using System.IO;
using UnityEngine;

public class DataLogger2 : MonoBehaviour
{
    public EyeTracker2 eyeTracker;
    public float logTimeInterval = 0.15f;
    private float previoustime;
    private char sep = ';';
    private string header = "Timestamp;PosX;PosY";
    private string filenameBase = "GazeData";
    public string pathPrefix = "Assets/Scripts/Data/";
    private string path;

    void Start()
    {
        previoustime = Time.time;
        path = GenerateFileName();
        FileStream fs = File.Create(path);
        fs.Close();

        using (StreamWriter writer = File.AppendText(path))
        {
            writer.WriteLine(header);
        }
    }

    void Update()
    {
        if (eyeTracker.ShouldCheck())
        {
            Vector2 textureCoord = eyeTracker.GetTextureCoord(new Ray(transform.position, transform.forward));
            if (textureCoord != Vector2.zero)
            {
                Log(textureCoord);
            }
        }
    }

    public string GenerateFileName()
    {
        int counter = 1;
        string ret = pathPrefix + filenameBase + counter + ".csv";
        while (File.Exists(ret))
        {
            counter++;
            ret = pathPrefix + filenameBase + counter + ".csv";
        }
        return ret;
    }

    public void Log(Vector2 pos)
    {
        string line = Time.time.ToString() + sep;
        line += pos.x + sep;
        line += pos.y;

        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine(line);
        }
    }
}
