using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Frame
{
    /// <summary>
    /// Number or "ID" is the ID of the frame of a video (it goes from 1 to infinity)
    /// </summary>
    public int Number;
    
    /// <summary>
    /// The timestamp of a frame is the timing this frame took place in the video.
    /// For example : the frame 2 takes place at 0.0231 second,
    /// the frame 3 takes place at 0.0342, ...
    /// </summary>
    public double Timestamp;
    
    /// <summary>
    /// The ActionUnitDict refers to the different values of the Action Units in that frame.
    /// For example the AU7_r could be equal to 0.45249072d;
    /// We use a dictionary to facilitate the research of those values
    /// </summary>
    public Dictionary<string, double> ActionUnitDict = new Dictionary<string, double>();

    /// <summary>
    /// The PoseDict works almost the same as the ActionUnitDict except it's for the pose of the head such as
    /// pose_rx, pose_rz, pose_ry
    /// </summary>
    public Dictionary<string, double> PoseDict = new Dictionary<string, double>();
    
    /// <summary>
    /// In the future we may need a gaze dict for the gazes of the eyes
    /// </summary>
    
    /// <summary>
    /// Method used for tests in order to see what is inside the dictionary
    /// </summary>
    public void PrintActionUnitDict()
    {
        foreach (var pair in ActionUnitDict)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }
    }

    /// TODO
    /// Put the SegmentedAudio Array in this class (maybe not)
    //public float AudioData;
}