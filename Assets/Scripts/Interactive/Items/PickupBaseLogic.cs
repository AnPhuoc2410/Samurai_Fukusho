using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Pickup Base Logic")]
    [Tooltip("Base class cho mọi loại item pickup. Kế thừa class này để tạo các loại item như Health, Key, Powerup...")]
    public abstract class PickupBaseLogic : MonoBehaviour
    {
        [Header("Pickup SFX")]
        public AudioClip pickupSFX;
        public float sfxVolume = 1f;

        [Header("Buff Duration (Optional)")]
        [Tooltip("Bật nếu item này là buff tạm thời (hiệu ứng sẽ tự động kết thúc sau thời gian chỉ định)")]
        public bool enableBuffDuration = false;
        [Tooltip("Thời gian tồn tại của buff (giây) sau khi player nhặt item này. Nếu = 0 sẽ là buff vĩnh viễn.")]
        public float buffDuration = 5f;

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (IsPlayer(other))
            {
                if (enableBuffDuration && buffDuration > 0f)
                {
                    OnBuffStart(other);
                    if (pickupSFX != null)
                        AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);
                    Destroy(gameObject);
                    // Gọi kết thúc buff sau thời gian chỉ định
                    var playerObj = other.gameObject;
                    playerObj.GetComponent<MonoBehaviour>().StartCoroutine(BuffTimer(playerObj));
                }
                else
                {
                    OnPickupEffect(other);
                    if (pickupSFX != null)
                        AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);
                    Destroy(gameObject);
                }
            }
        }

        private System.Collections.IEnumerator BuffTimer(GameObject playerObj)
        {
            yield return new WaitForSeconds(buffDuration);
            OnBuffEnd(playerObj);
        }

        protected virtual bool IsPlayer(Collider2D other)
        {
            return other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player");
        }

        // Logic riêng cho từng loại item (vĩnh viễn)
        protected abstract void OnPickupEffect(Collider2D player);

        // Logic buff tạm thời (override nếu cần)
        protected virtual void OnBuffStart(Collider2D player) { OnPickupEffect(player); }
        protected virtual void OnBuffEnd(GameObject playerObj) { }
    }
} 