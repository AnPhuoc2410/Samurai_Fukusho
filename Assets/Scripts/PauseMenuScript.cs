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
        settingButton.onClick.AddListener(GoToMenu);
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

    void GoToMenu()
    {
        Time.timeScale = 1f; // Trả lại thời gian bình thường trước khi chuyển scene
        SceneManager.LoadScene("MainMenu");
    }

    void ExitGame()
    {
        Time.timeScale = 1f; // Trả lại thời gian bình thường trước khi chuyển scene
        SceneManager.LoadScene("MainMenu");
    }
}
