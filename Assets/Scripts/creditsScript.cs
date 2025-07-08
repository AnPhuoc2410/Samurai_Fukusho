using UnityEngine;
using TMPro;

public class CreditsScroll : MonoBehaviour
{
    public float scrollSpeed = 100f;
    public float endYPosition = 1200f; // Tuỳ chiều cao mà text đi hết màn
    public GameObject finalText; // Dòng cuối giữ lại

    private RectTransform rectTransform;
    private bool finished = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        finalText.SetActive(false); // Ẩn dòng cuối lúc đầu
    }

    void Update()
    {
        if (!finished)
        {
            rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (rectTransform.anchoredPosition.y >= endYPosition)
            {
                finished = true;
                finalText.SetActive(true); // Hiện dòng cuối
            }
        }
    }
}
