using UnityEngine;

public class FadedRemove : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    private float elapsedTime = 0f;
    SpriteRenderer spriteRenderer;
    GameObject gameObject;
    Color startColor;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        gameObject = animator.gameObject;
        startColor = spriteRenderer.color;
        elapsedTime = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime += Time.deltaTime;

        float newAlpha = (1 - (elapsedTime / fadeTime)) * startColor.a;

        spriteRenderer.color = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            newAlpha
        );

        if (elapsedTime >= fadeTime)
        {
            Destroy(gameObject);
        }
    }


}
