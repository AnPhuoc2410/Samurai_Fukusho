using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ChestSystem
{
    [System.Serializable]
    public struct ItemEntry
    {
        [Tooltip("Prefab của item sẽ được random vào các rương")]
        public GameObject prefab;
        [Tooltip("Số lượng item này sẽ xuất hiện trên toàn bản đồ (chỉ dùng khi bật option chỉ định số lượng từng loại)")]
        public int quantity;
    }

    [AddComponentMenu("Chest System/Chest Item Randomizer")]
    [Tooltip("Gán script này vào 1 GameObject trong scene để random toàn bộ item vào các rương theo cấu hình bên dưới.")]
    public class ChestKeyRandomizer : MonoBehaviour
    {
        [Header("Item Pool Settings")]
        [Tooltip("Danh sách prefab item sẽ được random vào các rương. Nếu bật 'Chỉ định số lượng từng loại', hãy nhập số lượng cho từng prefab.")]
        public List<ItemEntry> itemPool = new List<ItemEntry>();

        [Tooltip("Bật để chỉ định số lượng từng loại item. Nếu tắt, hệ thống sẽ random tự do từ danh sách prefab.")]
        public bool useSpecificItemQuantities = false;

        [Header("Per Chest Item Count")]
        [Tooltip("Số lượng item tối thiểu mỗi rương (áp dụng cho toàn bộ hệ thống, trừ khi override từng rương)")]
        public int minItemsPerChest = 1;
        [Tooltip("Số lượng item tối đa mỗi rương (áp dụng cho toàn bộ hệ thống, trừ khi override từng rương)")]
        public int maxItemsPerChest = 5;

        [Header("Auto Destroy Settings")]
        [Tooltip("Bật để item tự hủy sau một thời gian nếu không được nhặt.")]
        public bool enableAutoDestroy = false;
        [Tooltip("Thời gian (giây) trước khi item tự hủy. Áp dụng cho toàn bộ item spawn ra từ rương.")]
        public float autoDestroyTime = 20f;

        [Header("Key Item Settings")]
        [Tooltip("Prefab chìa khóa sẽ được random vào 1 rương duy nhất.")]
        public GameObject keyPrefab;

        private void Start()
        {
            // Lấy danh sách tất cả rương trong scene
            var allChests = FindObjectsOfType<InteractableChest>().ToList();
            if (allChests.Count == 0) return;

            // Tạo pool item tổng
            List<GameObject> itemPoolList = new List<GameObject>();
            if (useSpecificItemQuantities)
            {
                foreach (var entry in itemPool)
                {
                    for (int i = 0; i < Mathf.Max(0, entry.quantity); i++)
                        itemPoolList.Add(entry.prefab);
                }
            }
            else
            {
                // Nếu không chỉ định số lượng, cho phép random tự do (có thể trùng lặp)
                itemPoolList = itemPool.Select(e => e.prefab).ToList();
            }

            // Đảm bảo key luôn có trong pool và chỉ 1 cái
            if (keyPrefab != null)
            {
                itemPoolList.RemoveAll(go => go == keyPrefab);
                itemPoolList.Add(keyPrefab);
            }

            // Shuffle pool để random công bằng
            itemPoolList = itemPoolList.OrderBy(x => Random.value).ToList();

            // Phân phối item vào các rương
            int chestCount = allChests.Count;
            int itemIdx = 0;
            foreach (var chest in allChests)
            {
                // Xác định min-max cho rương này
                int minItem = minItemsPerChest;
                int maxItem = maxItemsPerChest;
                if (chest.overrideItemCount)
                {
                    minItem = chest.minItemsOverride;
                    maxItem = chest.maxItemsOverride;
                }
                int itemCount = Random.Range(minItem, maxItem + 1);
                chest.itemPrefabs.Clear();
                var usedThisChest = new HashSet<GameObject>();
                for (int i = 0; i < itemCount && itemIdx < itemPoolList.Count; i++)
                {
                    // Không trùng item trong 1 rương
                    GameObject nextItem = itemPoolList[itemIdx++];
                    if (usedThisChest.Contains(nextItem)) continue;
                    chest.itemPrefabs.Add(nextItem);
                    usedThisChest.Add(nextItem);
                }
                // Nếu chưa đủ item (do trùng lặp), random bổ sung từ pool còn lại (không trùng trong rương)
                while (chest.itemPrefabs.Count < minItem && itemPoolList.Count > 0)
                {
                    var candidate = itemPoolList[Random.Range(0, itemPoolList.Count)];
                    if (!usedThisChest.Contains(candidate))
                    {
                        chest.itemPrefabs.Add(candidate);
                        usedThisChest.Add(candidate);
                    }
                }
            }

            // Nếu enable auto destroy, gán script tự hủy cho toàn bộ item spawn ra từ rương
            if (enableAutoDestroy)
            {
                foreach (var chest in allChests)
                {
                    chest.autoDestroyTime = autoDestroyTime;
                    chest.enableAutoDestroy = true;
                }
            }
        }
    }
} 