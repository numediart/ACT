using System.Collections.Generic;

/// <summary>
/// This script is used because Open face works with Action Units whereas, the model of the avatar works with blend shapes.
/// As soon as the data are gathered into variables, as a list of ordered frames, each frame contains an AU key with its value extracted from the video.
/// This script will intervene in order to translate the AU values, to the different blendshape values.
///
/// It's not as easy as you may think because AU_1 != Blendshape_1, it could be AU_1 = Blendshape_1 && Blendhape_2 && ... and it's not
/// AU_1.Value = 0.1 !== corresponding_blend_shape_value = 0.1. There is also a formula for each blendshape.
/// </summary>

public class ConvertAUtoBlendShapes
{
   public Dictionary<string, double> GetDictOfBlendShapesChangedInFrame(Frame frameFromOpenFace)
   {
      // Init dict, get a clean one from JsonReader tool
      Dictionary<string, double> blendShapeDictReturned =
         MainManager.Instance.JsonReader.GetCopyOfCleanBlendShapesDict();

      // Loop over each Action Unit the Frame contains
      foreach (var actionUnitFromFrame in frameFromOpenFace.ActionUnitDict)
      {
         // Ignore the Au_c because it's useless for our case, also ignore small changes : inferior to 0.001d
         if (actionUnitFromFrame.Key.EndsWith("_c") || actionUnitFromFrame.Value < 0.001d)
            continue;
         
         // Remove _r from key to match with our Action Unit Dict
         string cleanKey = actionUnitFromFrame.Key.Replace("_r", "");
         
         // Check if key is in our Action Unit Dict, if so we extract it to an ActionUnit variable
         if (MainManager.Instance.JsonReader.MyActionUnitList.AuDict.TryGetValue(cleanKey, out ActionUnit actionUnit))
         {
            // Loop in each Blend Shape of the Action Unit in order to add the key to the returned dict
            // and also add the right value, obtained by multiplying the Action Unit from frame value with the Blend Shape multiplier
            // example for understanding : AU_2 of OpenFace refers to a move of the Blend Shape lip_upper by 0.7 and lip_under by 0.3
            foreach (var blendShape in actionUnit.BlendShapes)
            {
               // Value clamped to maintain the right ratio (1.0 being the maximum)
               blendShapeDictReturned[blendShape.Key] += blendShape.Value * actionUnitFromFrame.Value;
            }
         }
      }

      return blendShapeDictReturned;
   }
   
   // own method because mathf.clamp() takes floats and i needed doubles
   private static double DoubleClamp(double value, double min, double max)
   {
      if (value < min)
         return min;
      if (value > max)
         return max;
      return value;
   }
}
