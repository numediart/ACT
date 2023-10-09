using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.Serialization;

[Serializable]
public class CsvReader : MonoBehaviour
{
    [Header("Instances")] public TextAsset CsvToRead;
    
    [Header("Options")]
    public string DecimalSeparatorInCSV;
    public int TotNbColumn;
    public bool IgnoreFirstLine;
    public int FrameToRead;
    public int ColumnIdForFrameNb;
    public int ColumnIdForTimestamp;
    public int NbColumnToIgnoreForAu;
    public int NbColumnToIgnoreForPose;
    
    [Header("Data check")]
    public string[] data;
    public FrameList MyFrameList = new FrameList();

    #region Function Called Externally
    
    public void ReadCsv()
    {
        data = CsvToRead.text.Split(new string[] { ", ", "\n" }, StringSplitOptions.None);
        int tableSize = data.Length / TotNbColumn - (IgnoreFirstLine ? 1 : 0);
        MyFrameList.Frames = new Frame[tableSize];
        
        CultureInfo ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = DecimalSeparatorInCSV;

        // Tab filling
        for (int i = 0; i < tableSize; i++)
        {
            MyFrameList.Frames[i] = new Frame();

            //Frame Manager
            MyFrameList.Frames[i].Number = int.Parse(data[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + ColumnIdForFrameNb],
                NumberStyles.Any, ci);
            
            MyFrameList.Frames[i].Timestamp = double.Parse(data[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + ColumnIdForTimestamp],
                NumberStyles.Any, ci);
            
            // Poses
            for (int j = 0; j < NbColumnToIgnoreForAu - NbColumnToIgnoreForPose; j++)
            {
                MyFrameList.Frames[i].PoseDict.Add(data[NbColumnToIgnoreForPose + j], 
                    double.Parse(data[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + NbColumnToIgnoreForPose + j],
                    NumberStyles.Any, ci));
            }
            
            // Action units
            for (int j = 0; j < TotNbColumn - NbColumnToIgnoreForAu; j++)
            {
                MyFrameList.Frames[i].ActionUnitDict.Add(data[NbColumnToIgnoreForAu + j], 
                    double.Parse(data[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + NbColumnToIgnoreForAu + j],  
                        NumberStyles.Any, ci));
            }
        }
    }

    public FrameList PushMyCsvIntoFrameList(string csv)
    {
        FrameList frameList = new FrameList();
        
        string[] csvDataTab= csv.Split(new string[] { ", ", "\n" }, StringSplitOptions.None);
        int tableSize = csvDataTab.Length / TotNbColumn - (IgnoreFirstLine ? 1 : 0);
        frameList.Frames = new Frame[tableSize];
        
        CultureInfo ci = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = DecimalSeparatorInCSV;

        // Tab filling
        for (int i = 0; i < tableSize; i++)
        {
            frameList.Frames[i] = new Frame();

            //Frame Manager
            frameList.Frames[i].Number = int.Parse(csvDataTab[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + ColumnIdForFrameNb],
                NumberStyles.Any, ci);
            
            frameList.Frames[i].Timestamp = double.Parse(csvDataTab[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + ColumnIdForTimestamp],
                NumberStyles.Any, ci);
            
            // Poses
            for (int j = 0; j < NbColumnToIgnoreForAu - NbColumnToIgnoreForPose; j++)
            {
                frameList.Frames[i].PoseDict.Add(csvDataTab[NbColumnToIgnoreForPose + j], 
                    double.Parse(csvDataTab[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + NbColumnToIgnoreForPose + j],
                        NumberStyles.Any, ci));
            }
            
            // Action units
            for (int j = 0; j < TotNbColumn - NbColumnToIgnoreForAu; j++)
            {
                frameList.Frames[i].ActionUnitDict.Add(csvDataTab[NbColumnToIgnoreForAu + j], 
                    double.Parse(csvDataTab[TotNbColumn * (i + (IgnoreFirstLine ? 1 : 0)) + NbColumnToIgnoreForAu + j],  
                        NumberStyles.Any, ci));
            }
        }

        return frameList;
    }
    
    public void ClearCsv()
    {
        data = new string[] {};
        MyFrameList = new FrameList();
    }

    public void ReadFrame()
    {
        // To check
        foreach (var pair in MyFrameList.Frames[FrameToRead].PoseDict)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }
    }
    
    #endregion
}
