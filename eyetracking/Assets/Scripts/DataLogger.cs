using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLogger : MonoBehaviour
{
    public float lth = 0.5f;
    private Vector3 prevpos;
    private char sep = ';';
    private string header = "Timestamp;Target;PosX;PosY;PosZ";
    public string filenameBase = "GazeData";
    public string pathPrefix = "Assets/Scripts/Data/";
    private string path;
    public EyeTracker2 eyeTracker;
    

    void Start()
    {
        //path = Application.persistentDataPath + "/" +filenameBase +".csv";
        path = pathPrefix + filenameBase + System.DateTime.Now.ToString("ddmmyyyy-HHmmss")+".csv";
        FileStream fs = File.Create(path);
        fs.Close();
        prevpos = eyeTracker.gazePoint;
        if(HasHeader() == false)
        {
            using(StreamWriter writer = File.AppendText(path))
            {
                writer.WriteLine(header);
            }
        }
    }
    public bool HasHeader()
    {
        using(StreamReader reader = new StreamReader(path))
        {
            if(reader.ReadLine() == header)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        //Quaternion currot = eyeTracker.transform.rotation; 
        Vector3 curpos = eyeTracker.gazePoint;
        float distance = Vector3.Distance(curpos,prevpos);
        if(distance > lth)
        {
            Log(curpos);
            //Log(curpos, currot);
            prevpos = eyeTracker.gazePoint;
        }
        
    }
    public void WriteToFile(string txt)
    {
        using(StreamWriter writer = new StreamWriter(path,true))
        {
            writer.WriteLine(txt);
        }
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

    // public string[] QuaternionToStringArray(Quaternion rot)
    // {
    // string[] arr = new string[3];
    // arr[0] = Convert.ToString(rot.eulerAngles.x);
    // arr[1] = Convert.ToString(rot.eulerAngles.y);
    // arr[2] = Convert.ToString(rot.eulerAngles.z);
    // return arr;
    // }

    public void Log(Vector3 pos)
    {
        string line = GetTimestamp(DateTime.Now)+";";
        line += DetermineTarget()+";";
        line += ItemsMerged(sep, Vector3ToStringArray(pos));
        // line += ItemsMerged(sep, QuaternionToStringArray(rot));
        WriteToFile(line);

    }
}

