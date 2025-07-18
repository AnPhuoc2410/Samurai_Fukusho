using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    public RectTransform creditText;     // Gán object chứa text cuộn
    public float scrollSpeed = 100f;
    public float endYPosition = 1500f;


    public GameObject finalText;
    public GameObject backToMenuButton;

    private bool finished = false;

    void Start()
    {
        if (creditText == null)
        {
            Debug.LogError("⚠️ Chưa gán CreditText trong Inspector!");
            return;
        }

        if (finalText != null)
            finalText.SetActive(false);

        if (backToMenuButton != null)
        {
            backToMenuButton.SetActive(false);
            backToMenuButton.GetComponent<Button>().onClick.AddListener(BackToMenu);
        }
    }

    void Update()
    {
        if (!finished && creditText != null)
        {
            creditText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Debug vị trí Y hiện tại
            Debug.Log("📍 Y Position: " + creditText.anchoredPosition.y);

            // Khi cuộn vượt qua vị trí Y kết thúc
            if (creditText.anchoredPosition.y >= endYPosition)
            {
                Debug.Log("✅ Đã đến cuối credits, hiển thị finalText và button!");
                finished = true;
                ShowFinalTextAndButton(); // gọi trực tiếp, không chờ 2 giây
            }
        }
    }

    void ShowFinalTextAndButton()
    {
        if (finalText != null)
            finalText.SetActive(true);

        if (backToMenuButton != null)
            backToMenuButton.SetActive(true);
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Đặt đúng tên Scene Menu chính của bạn
    }
}
