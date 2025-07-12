using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scence1");
    }
    
    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay"); 
    }
}
