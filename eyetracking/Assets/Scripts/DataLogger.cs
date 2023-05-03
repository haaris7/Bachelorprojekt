using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class DataLogger : MonoBehaviour
{
    public float lth = 0.5f;
    private Vector3 prevpos;
    private char sep = ';';
    private string header = "Timestamp;Region;Target;PosX;PosY;PosZ";
    private string filenameBase = "GazeData";
    private string pathPrefix = "Assets/Scripts/Data/";
    private string path;
    public int activeregion = 0;
    public bool IsLogging = false;
    public EyeTracker2 eyeTracker;
    public float previousTime;
    public float period = 0.1f;
    public int width = 1920;
    public int height = 1080;
    public string currentFileName = "";
    public Camera camera;
    


    

    void Start()
    {
        path = genFileName();
        FileStream fs = File.Create(path);
        fs.Close();
        prevpos = eyeTracker.gazePoint;
        using(StreamWriter writer = File.AppendText(path))
        {
            writer.WriteLine(header);
        }
    }


    void Update()
    {
        if(IsLogging)
        {
            Vector3 curpos = eyeTracker.gazePoint;
            float distance = Vector3.Distance(curpos,prevpos);
            if(distance > lth)
            {
                Log(curpos);
                prevpos = eyeTracker.gazePoint;
            }
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
        currentFileName = filenameBase+"("+counter+")_";
        return ret;
    }
    public string genImageName()
    {
        string pth = "Screenshot_"+currentFileName+"Region"+"("+activeregion+")"+ ".png";
        return pth;
    }
    // void Update()
    // {
    //     if(IsLogging)
    //     {
    //         // float time = Time.time-previousTime;
    //         // UnityEngine.Debug.Log(time);
    //         Vector3 curpos = eyeTracker.gazePoint;
    //         if(Time.time-previousTime > period)
    //         {
    //             Log(curpos);
    //             previousTime = Time.time;
    //         }
    //     }
    // }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("HH-mm-ss");
    }

    public string DetermineTarget()
    {
        if(eyeTracker.TargetName != "")
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
    public string[] Vector3ToStringArray(Vector3 pos)
    {
        string[] arr = new string[3];
        arr[0] = Convert.ToString(pos.x);
        arr[1] = Convert.ToString(pos.y);
        arr[2] = Convert.ToString(pos.z);
        return arr;
    }

    public string ItemsMerged(char seperator, string[] items)
    {
        string txt = "";
        for(int i = 0; i < items.Length; i++)
        {
            if(i == items.Length-1) txt = txt+items[i];
            else txt = txt+items[i]+seperator;
        }
        return txt;
    }

    // public void Log(Vector3 pos)
    // {
    //     string line = Time.time +";";
    //     line += activeregion+";";
    //     line += DetermineTarget()+";";
    //     line += ItemsMerged(sep, Vector3ToStringArray(pos));
    //     using(StreamWriter writer = new StreamWriter(path,true))
    //     {
    //         writer.WriteLine(line);
    //     }

    // }

    public void Log(Vector3 pos)
    {
        // Assuming you have a reference to your camera
        Vector2 pos2D = camera.WorldToViewportPoint(pos);

        string line = Time.time +";";
        line += activeregion+";";
        line += DetermineTarget()+";";
        line += pos2D.x + ";" + pos2D.y;
        using(StreamWriter writer = new StreamWriter(path,true))
        {
            writer.WriteLine(line);
        }
    }
    void TakeScreenshot(Camera cam)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Path.Combine(@"Assets\Scripts\Data\Screenshots",genImageName());
        File.WriteAllBytes(filename, bytes);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Start")
        {
            camera = other.gameObject.GetComponent<Camera>();
            TakeScreenshot(camera);
            IsLogging = true;
            // UnityEngine.Debug.Log("Enter");

            activeregion++;
            previousTime = Time.time;
        }
        else if(other.tag == "Stop")
        {
            // UnityEngine.Debug.Log("Exit");
            IsLogging = false;
        }
    }
}

