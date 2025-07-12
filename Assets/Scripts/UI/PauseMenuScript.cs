using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Menu UI")]
    public GameObject pauseMenuPanel;

    [Header("Buttons")]
    public Button continueButton;
    public Button settingButton;
    public Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        // Ẩn menu khi bắt đầu
        pauseMenuPanel.SetActive(false);

        // Gán sự kiện cho các nút
        continueButton.onClick.AddListener(ResumeGame);
        settingButton.onClick.AddListener(GoToMainMenu);
        exitButton.onClick.AddListener(ExitGame);
    }

    void Update()
    {
        // Khi nhấn ESC thì bật/tắt menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng thời gian game
        isPaused = true;
    }

    void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Tiếp tục thời gian game
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        // Lưu lại scene hiện tại để sau này quay lại
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        Time.timeScale = 1f; // đảm bảo game không bị pause khi quay lại
        SceneManager.LoadScene("MainMenu");
    }

    void ExitGame()
    {
        Debug.Log("Thoát game...");
        Application.Quit();
    }
}
