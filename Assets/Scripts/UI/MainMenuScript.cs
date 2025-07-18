using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scence1");
    }
    
    public void Continue()
    {
        // Lấy lại scene trước đó
        string lastScene = PlayerPrefs.GetString("LastScene", "");

        if (!string.IsNullOrEmpty(lastScene))
        {
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.Log("No saved scene to continue to.");
        }
    }
    
    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }
    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
        
        // Nếu đang chạy trong Unity Editor, dừng play mode
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
