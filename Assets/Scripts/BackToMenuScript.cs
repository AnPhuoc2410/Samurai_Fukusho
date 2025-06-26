using UnityEngine;
using UnityEngine.SceneManagement;


public class BackToMenuScript : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
