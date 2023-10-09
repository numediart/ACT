using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class ActionUnit
{
    public string Name;
    public List<BlendShape> BlendShapeList = new List<BlendShape>();
    public Dictionary<string, double> BlendShapes = new Dictionary<string, double>();

    public void FillBlendShapesDict()
    {
        foreach (var blendShape in BlendShapeList)
        {
            BlendShapes.Add(blendShape.Key, blendShape.Value);
        }
    }

    public void PrintDict()
    {
        foreach (var blendShape in BlendShapes)
        {
            Debug.Log("key: " + blendShape.Key + " value: " + blendShape.Value);
        }
    }

    public void ClearBlendShapeList()
    {
        foreach (var blendShape in BlendShapeList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(blendShape.Key);

            if (blendShape.Value < 0.5)
            {
                sb.Append("_min");

                blendShape.Key = sb.ToString();

                blendShape.Value = (0.5d - blendShape.Value) * 2;
            }
            else
            {
                sb.Append("_max");

                blendShape.Key = sb.ToString();

                blendShape.Value = (blendShape.Value - 0.5d) * 2;
            }
        }
    }
}