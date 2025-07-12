using UnityEngine;

public class ControlGuideProximityFade : MonoBehaviour
{
    public Transform player;  // Gán Player tại đây
    public float showRange = 3f;
    public float fadeSpeed = 2f;

    private SpriteRenderer sr;
    private float targetAlpha = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Ẩn hint ban đầu
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0;
            sr.color = c;
        }

        // Tự tìm Player nếu chưa gán
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || sr == null) return;

        float distance = Vector2.Distance(player.position, transform.position);
        targetAlpha = (distance <= showRange) ? 1f : 0f;

        Color currentColor = sr.color;
        currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        sr.color = currentColor;
    }

}
