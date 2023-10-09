using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

[Serializable]
public class AvatarAction
{
    // Filled by the config file
    public int Intensity;
    public string CsvFilePath; //.csv
    public string AudioFilePath; //.mp4
    
    // Filled by the functions
    public string Csv;
    public AudioClip Audio;
    public bool ContainsAudio;
    public double Duration;
    public FrameList ActionFrameList = new FrameList();
    public FrameManager FrameManager = new FrameManager();

    public void Init()
    {
        // Find the audio and reads it
        if (!string.IsNullOrEmpty(AudioFilePath))
        {
            Audio = GetAudioClipFromFile(AudioFilePath);
            if (Audio != null)
                ContainsAudio = true;
        }

        //Find the csv and reads it
        if (!string.IsNullOrEmpty(CsvFilePath))
            Csv = GetCsvFromFile(CsvFilePath);

        // If success
        if (!string.IsNullOrEmpty(Csv))
        {
            ActionFrameList = MainManager.Instance.CsvReader.PushMyCsvIntoFrameList(Csv);

            Duration = ActionFrameList.Frames[^1].Timestamp;
            
            FrameManager.Init(ActionFrameList.Frames);
        }
    }

    // TODO : Test, seems to work
    private AudioClip GetAudioClipFromFile(string filePath)
    {
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);
        www.SendWebRequest();

        while (!www.isDone)
        {
            // Waiting for the request to complete
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load MP4 audio clip: " + www.error);
            return null;
        }

        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
        return audioClip;
    }

    private string GetCsvFromFile(string filePath)
    {
        using StreamReader sr = File.OpenText(CsvFilePath);
        StringBuilder sb = new StringBuilder();
        string s = "";
        while ((s = sr.ReadLine()) != null)
        {
            sb.Append(s);
            sb.Append("\n");
        }

        return sb.ToString();
    }
    
    public void ForceFinish()
    {
        FrameManager.FrameNb = 0;
        
        if (ContainsAudio)
            MainManager.Instance.AudioController.StopAudioClip();
    }
}
