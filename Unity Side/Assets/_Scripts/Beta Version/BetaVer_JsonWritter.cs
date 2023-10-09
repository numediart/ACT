using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BetaVer_JsonWritter : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectWorldCoordinate
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    [System.Serializable]
    public class GoWcList
    {
        public List<GameObjectWorldCoordinate> GameObjectWorldCoordinates = new List<GameObjectWorldCoordinate>();
    }

    public GoWcList MyGoWcList = new GoWcList();
    public string JsonName;
    
    public void OutputJson()
    {
        string strOutput = JsonUtility.ToJson(MyGoWcList);
        
        File.WriteAllText(Application.dataPath + "/Json Files/" + JsonName.Trim() + ".json", strOutput);
    }

    public void FillGoWcList(AlphaVer_Landmark alphaVerLandmarkToAdd)
    {
        GameObjectWorldCoordinate newGoWcList = new GameObjectWorldCoordinate();

        Transform landmarkToAddTransform = alphaVerLandmarkToAdd.gameObject.transform;
        
        newGoWcList.Name = alphaVerLandmarkToAdd.gameObject.name;
        newGoWcList.Position = landmarkToAddTransform.TransformPoint(landmarkToAddTransform.position);;
        newGoWcList.Rotation = landmarkToAddTransform.TransformDirection(landmarkToAddTransform.localEulerAngles);
        newGoWcList.Scale = landmarkToAddTransform.lossyScale;
        
        MyGoWcList.GameObjectWorldCoordinates.Add(newGoWcList);
    }
}
