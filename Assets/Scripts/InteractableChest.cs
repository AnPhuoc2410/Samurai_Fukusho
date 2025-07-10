using UnityEngine;

public class InteractableChest : PressE_ToOpen
{
    protected override void OnInteract()
    {
        base.OnInteract();

        // TODO: Thêm logic riêng cho rương ở đây
        // Ví dụ: Hiện UI inventory, spawn items, etc.
    }
}
