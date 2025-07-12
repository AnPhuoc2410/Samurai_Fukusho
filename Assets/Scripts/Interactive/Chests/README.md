Dưới đây là hướng dẫn chi tiết thao tác với Unity Editor để sử dụng hệ thống rương và chìa khóa vừa xây dựng:

---

## 1. **Chuẩn bị Prefab và Scene**

### a. **Prefab rương**
- Đảm bảo prefab rương có các component:
  - **Animator** (có animation mở rương, trigger “isOpen”)
  - **Collider2D** (IsTrigger = false)
  - **Script**: Gán script `ChestSystem.InteractableChest` vào prefab rương.

### b. **Prefab item (powerup, chìa khóa,...)**
- Mỗi item là một prefab riêng, có Collider2D (IsTrigger = true).
- Prefab chìa khóa gán thêm script `ChestSystem.KeyPickup`.

---

## 2. **Set up rương trong Scene**

1. **Kéo prefab rương vào scene** (có thể đặt nhiều rương).
2. **Chọn từng rương trong Hierarchy**, ở Inspector:
   - **Item Prefabs**: Kéo tối đa 5 prefab item (trừ chìa khóa) vào list này.
   - **Key Chest Option**:
     - **KeyChestMode**: Chọn `Random` (mặc định) nếu muốn hệ thống tự random rương chứa chìa khóa, hoặc `Manual` nếu muốn chỉ định rương này chắc chắn chứa chìa khóa.
     - **isManualKeyChest**: Tick vào nếu chọn Manual và muốn rương này chứa chìa khóa.
   - **Spawn Settings**: Có thể điều chỉnh bán kính spawn item (`spawnRadius`), số lần thử tránh overlap (`maxSpawnTries`), và layer mask cho item.

---

## 3. **Set up prefab chìa khóa**

1. **Tạo prefab chìa khóa** (có Collider2D, IsTrigger = true).
2. **Gán script `ChestSystem.KeyPickup`** vào prefab.
   - Có thể đổi tên key (mặc định là `"LevelGateKey"`).
   - Kéo SFX vào trường `pickupSFX` nếu muốn có âm thanh khi nhặt.

---

## 4. **Thêm script quản lý random chìa khóa**

1. **Tạo một GameObject mới trong scene** (ví dụ: đặt tên “ChestManager”).
2. **Gán script `ChestSystem.ChestKeyRandomizer`** vào GameObject này.
   - Kéo prefab chìa khóa vào trường `keyPrefab`.
   - Để mặc định các option (autoFindChests = true, randomizeKeyChest = true) nếu muốn random tự động.

---

## 5. **Set up Player**

- Đảm bảo Player có tag `"Player"` và Collider2D.
- Player phải có script `PlayerInventory` (hoặc có sẵn trong scene).

---

## 6. **Test hệ thống**

- Chạy game, lại gần rương, bấm E để mở.
- Quan sát item spawn quanh rương, nhặt thử các item và chìa khóa.
- Kiểm tra PlayerInventory (có thể debug log hoặc UI) để xác nhận đã nhận chìa khóa.

---

### **Lưu ý**
- Nếu có nhiều loại item, mỗi prefab item nên có script riêng để xử lý hiệu ứng khi player nhặt.
- Có thể clone/copy các rương và chỉnh list item cho từng rương tùy ý.
- Nếu muốn kiểm tra rương nào chứa chìa khóa, có thể tạm thời bật log trong script hoặc gán màu đặc biệt cho prefab chìa khóa khi test.

---