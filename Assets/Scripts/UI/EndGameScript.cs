using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : MonoBehaviour
{
    public void BackToMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}
