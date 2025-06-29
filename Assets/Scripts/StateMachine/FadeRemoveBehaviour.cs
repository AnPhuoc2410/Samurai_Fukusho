using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeDuration = 1f;
    private float timeElapsed = 0f;
    SpriteRenderer spriteRenderer;
    GameObject targetObject;
    Color startColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        targetObject = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(timeElapsed / fadeDuration);
        float alpha = Mathf.Lerp(startColor.a, 0f, t);

        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (timeElapsed >= fadeDuration)
        {
            Object.Destroy(targetObject);
        }

    }
}
