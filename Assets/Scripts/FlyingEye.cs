using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public DetectionZone detectionZone;
    Animator animator;
    Rigidbody2D rb;
    public bool _hasTarget = false;

    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = detectionZone.detectedColliders.Count > 0;
    }
}
