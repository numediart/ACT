using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("UI")] [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TMP_InputField _password;

    [Header("Infos")] public string Id;
    private string _passwordInput;

    public void PersonalizeRoomPrefab()
    {
        _name.text = Id;
    }

    private void ResetInput()
    {
        _passwordInput = "";
        _password.text = "";
    }
    
    #region Listeners

    public void TryJoinRoomAsAdmin()
    {
        NetworkManager.Instance.RequestToJoinRoom(Id, _passwordInput, true);
        ResetInput();
    }

    public void TryJoinRoomAsUser()
    {
        NetworkManager.Instance.RequestToJoinRoom(Id, _passwordInput, false);
        ResetInput();
    }

    public void PasswordInputUpdate(string newInput)
    {
        _passwordInput = newInput;
    }
    
    #endregion
}
