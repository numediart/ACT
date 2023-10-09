using UnityEngine;

[System.Serializable]
public class AlphaVer_ManualAction
{
   [SerializeField] private string _landmarkKey;
   [SerializeField] private Vector3 _movement;
   [SerializeField] private Vector3 _rotation;
   [SerializeField] private float _timeBeforeRewind;

   public void ExecuteAction()
   {
      AlphaVer_MainScript.s_instance.alphaVerAvatarController.HeadModificationEvent(_landmarkKey, _movement, _rotation, _timeBeforeRewind);
   }
}
