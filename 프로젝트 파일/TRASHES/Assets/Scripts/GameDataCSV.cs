using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.InteropServices;
using System;
using System.Threading;
//using GoogleSheetsToUnity;
//using GoogleSheetsToUnity.ThirdPary;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;

public class GameDataCSV : MonoBehaviour
{
    private List<string[]> studentData = new List<string[]>();
    void Write(){
        string[] tempStudentData = new string[3];
        tempStudentData[0] = "Name";
        tempStudentData[1] = "Age";
        tempStudentData[2] = "ID";
        studentData.Add(tempStudentData);
        for(int i = 0; i < 10; i++)
        {
            tempStudentData = new string[3];
            tempStudentData[0] = "Micheal"+i;// Name            
            tempStudentData[1] = (i+20).ToString(); // Age            
            tempStudentData[2] = i.ToString();// ID            
            studentData.Add(tempStudentData);
        }
        string[][] output = new string[studentData.Count][];
        for(int i = 0; i < output.Length; i++)
        {
            output[i] = studentData[i];
        }
        int length = output.GetLength(0);
        string delimiter = ",";
        StringBuilder sb = new StringBuilder();
        for(int index = 0; index < length; index++)
        {
            sb.AppendLine(string.Join(delimiter, output[index]));
        }
        string filePath = getPath();
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb); 
        outStream.Close();
    }
    
    public void Start(){
        Write();
    }



    private string getPath(){
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" +"/PlayerData.csv";
#elif UNITY_WINDOWSEDITOR
        return Application.dataPath + "/CSV/" + "/PlayerData.csv";
#else
        return Application.dataPath +"/"+"DataFile.csv";
#endif
    }
}
