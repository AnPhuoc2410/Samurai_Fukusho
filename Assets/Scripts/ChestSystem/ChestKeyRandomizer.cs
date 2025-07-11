using UnityEngine;
using System.Collections.Generic;

namespace ChestSystem
{
    public class ChestKeyRandomizer : MonoBehaviour
    {
        [Tooltip("Prefab chìa khóa sẽ được add vào rương")] public GameObject keyPrefab;
        [Tooltip("Tìm tất cả rương trong scene khi Start")] public bool autoFindChests = true;
        [Tooltip("Nếu true, random rương chứa chìa khóa mỗi lần scene load. Nếu false, chỉ dùng các rương có isManualKeyChest = true")] public bool randomizeKeyChest = true;

        private void Start()
        {
            List<InteractableChest> allChests = new List<InteractableChest>();
            if (autoFindChests)
            {
                allChests.AddRange(FindObjectsOfType<InteractableChest>());
            }
            // Nếu không autoFind, dev có thể add thủ công qua Inspector (mở rộng sau)

            // Xóa chìa khóa cũ nếu có (tránh double key khi reload scene)
            foreach (var chest in allChests)
            {
                chest.itemPrefabs.RemoveAll(go => go == keyPrefab);
            }

            if (randomizeKeyChest)
            {
                // Lọc các rương cho phép random (Manual hoặc Random mode)
                List<InteractableChest> candidates = new List<InteractableChest>();
                foreach (var chest in allChests)
                {
                    if (chest.keyChestMode == InteractableChest.KeyChestMode.Random ||
                        (chest.keyChestMode == InteractableChest.KeyChestMode.Manual && chest.isManualKeyChest))
                    {
                        candidates.Add(chest);
                    }
                }
                if (candidates.Count > 0)
                {
                    int idx = Random.Range(0, candidates.Count);
                    candidates[idx].AddKeyToChest(keyPrefab);
                }
                else
                {
                    Debug.LogWarning("[ChestKeyRandomizer] Không tìm thấy rương hợp lệ để random chìa khóa!");
                }
            }
            else
            {
                // Chỉ add key vào các rương có isManualKeyChest = true
                foreach (var chest in allChests)
                {
                    if (chest.keyChestMode == InteractableChest.KeyChestMode.Manual && chest.isManualKeyChest)
                    {
                        chest.AddKeyToChest(keyPrefab);
                    }
                }
            }
        }
    }
} 