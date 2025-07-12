using UnityEngine;

public class ParalaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    // Vị trí bắt đầu cho hiệu ứng parallax
    Vector2 startingPosition;

    // Giá trị Z bắt đầu của đối tượng parallax
    float startingZ;

    // Khoảng cách mà camera đã di chuyển từ vị trí bắt đầu của đối tượng parallax
    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    // Nếu đối tượng ở phía trước mục tiêu, sử dụng near clip plane. Nếu sau mục tiêu, sử dụng farClipPlane
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));

    // Đối tượng càng xa người chơi, hiệu ứng ParallaxEffect sẽ di chuyển càng nhanh. Kéo giá trị Z gần mục tiêu hơn để làm cho nó di chuyển chậm hơn.
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    // Start được gọi một lần trước khi thực thi Update đầu tiên sau khi MonoBehaviour được tạo
    void Start()
    {
        InitializeParallax();
    }

    public void InitializeParallax()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
        
        // Auto-find camera if not assigned
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // Auto-find player if not assigned
        if (followTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                followTarget = player.transform;
                Debug.Log($"ParalaxEffect on {gameObject.name} auto-found player target");
            }
        }
    }

    // Update được gọi một lần mỗi frame
    void Update()
    {
        // Kiểm tra nếu thiếu references
        if (cam == null || followTarget == null)
        {
            return;
        }

        // Khi mục tiêu di chuyển, di chuyển đối tượng parallax cùng khoảng cách nhân với một hệ số
        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;

        // Vị trí X/Y thay đổi dựa trên tốc độ di chuyển của mục tiêu nhân với hệ số parallax, nhưng Z giữ nguyên
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }

    // Public method để reset parallax khi scene thay đổi
    public void ResetParallax()
    {
        InitializeParallax();
    }
}
