using UnityEngine;

namespace ChestSystem
{
    public class KeyPickup : MonoBehaviour
    {
        [Tooltip("Tên chìa khóa sẽ add vào PlayerInventory")] public string keyName = "LevelGateKey";
        [Tooltip("SFX khi nhặt chìa khóa (tùy chọn)")] public AudioClip pickupSFX;
        [Tooltip("Âm lượng SFX")] public float sfxVolume = 1f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // Add key to inventory
                PlayerInventory.Instance.AddKey(keyName);

                // Play SFX nếu có
                if (pickupSFX != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);
                }

                // Hủy object
                Destroy(gameObject);
            }
        }
    }
} 