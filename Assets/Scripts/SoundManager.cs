using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Button soundButton;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    public AudioSource backgroundMusic; // <- THÊM DÒNG NÀY để gắn nhạc

    private bool isSoundOn = true;

    void Start()
    {
        soundButton.onClick.AddListener(ToggleSound);
        UpdateButtonIcon();
    }

    void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        if (backgroundMusic != null)
        {
            backgroundMusic.mute = !isSoundOn;
        }

        UpdateButtonIcon();
    }

    void UpdateButtonIcon()
    {
        soundButton.image.sprite = isSoundOn ? soundOnIcon : soundOffIcon;
    }
}
