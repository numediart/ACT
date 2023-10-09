using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void TryLoadUserScene()
    {
        // Connect using the network manager
        NetworkManager.Instance.UserSelectedView(false);
    }

    public void TryLoadAdminScene()
    {
        // Connect using the network manager
        NetworkManager.Instance.UserSelectedView(true);
    }
    
    public void LoadUserScene()
    {
        // Load scene
        SceneManager.LoadScene("_Ver 1.0 - User");
    }

    public void LoadAdminScene()
    {
        // Load scene
        SceneManager.LoadScene("_Ver 1.0 - Admin");
    }

    public void LoadRecordScene()
    {
        SceneManager.LoadScene("_Ver 1.0 - Record");
    }

    public void LoadRoomSelectionScene()
    {
        SceneManager.LoadScene("_Ver 1.0 - Room Selection");
    }

    public void LoadRoomSelectionSceneFromMenu()
    {
        NetworkManager.Instance.ConnectToRoomSelection();
        LoadRoomSelectionScene();
    }

    public void ReturnToMainMenu()
    {
        NetworkManager.Instance.ReturnToMainMenuEvent();
        SceneManager.LoadScene("_Ver 1.0 - Menu");
    }

    public void LoadConfigurationScene()
    {
        SceneManager.LoadScene("_Ver 1.0 - Configuration");
    }

    public void QuitProgram()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
