using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SoundData
{
    public AudioClip clip;
    public float volume = 1f;
    public float delay = 0.25f;
}

public class AnimaState_MultiSFX : StateMachineBehaviour
{
    public List<SoundData> soundsToPlay = new List<SoundData>();
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;
    public bool randomPlay = false;

    // Sound sequence tracking
    private int currentSoundIndex = 0;
    private float timeSinceEntered = 0;
    private bool hasDelayedSoundPlayed = false;

    private void PlayCurrentSound(Animator animator)
    {
        if (soundsToPlay.Count == 0) return;

        int index = currentSoundIndex;
        if (randomPlay)
        {
            index = UnityEngine.Random.Range(0, soundsToPlay.Count);
        }

        var sound = soundsToPlay[index];
        AudioSource.PlayClipAtPoint(sound.clip, animator.gameObject.transform.position, sound.volume);

        // Advance to next sound or reset to beginning
        currentSoundIndex = (currentSoundIndex + 1) % soundsToPlay.Count;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter)
        {
            PlayCurrentSound(animator);
        }

        timeSinceEntered = 0f;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playAfterDelay && !hasDelayedSoundPlayed && soundsToPlay.Count > 0)
        {
            timeSinceEntered += Time.deltaTime;

            if(timeSinceEntered > soundsToPlay[currentSoundIndex].delay)
            {
                PlayCurrentSound(animator);
                hasDelayedSoundPlayed = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit)
        {
            PlayCurrentSound(animator);
        }
    }
}
