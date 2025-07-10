using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SoundData
{
    public AudioClip clip;
    public float volume = 1.0f;
    public float delay = 0.25f;
}

public class AnimaState_MultiSFX : StateMachineBehaviour
{
    [Header("Sound Settings")]
    public List<SoundData> soundsToPlay = new List<SoundData>();
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;

    [Header("Playback Options")]
    public bool randomPlay = false;
    [Tooltip("If enabled, all instances of this prefab will share the same sound counter. If disabled, each instance has its own counter.")]
    public bool usePrefabCounter = false;

    // Dictionary to store counters for each prefab
    private static Dictionary<string, int> prefabCounters = new Dictionary<string, int>();
    
    // Sound sequence tracking
    private int localSoundIndex = 0;
    private string prefabName;
    private float timeSinceEntered = 0;
    private bool hasDelayedSoundPlayed = false;

    private string GetPrefabName(GameObject obj)
    {
        #if UNITY_EDITOR
        var prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
        if (prefabRoot != null)
        {
            var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(prefabRoot);
            if (prefabAsset != null)
            {
                return prefabAsset.name;
            }
        }
        #endif
        return obj.name.Split(new[] { '(' })[0].Trim(); // Fallback for runtime & non-prefab
    }

    private int CurrentIndex
    {
        get
        {
            if (!usePrefabCounter) return localSoundIndex;
            if (!prefabCounters.ContainsKey(prefabName))
            {
                prefabCounters[prefabName] = 0;
            }
            return prefabCounters[prefabName];
        }
        set
        {
            if (usePrefabCounter)
                prefabCounters[prefabName] = value;
            else
                localSoundIndex = value;
        }
    }

    private void PlayCurrentSound(Animator animator)
    {
        if (soundsToPlay.Count == 0) return;

        int index = CurrentIndex;
        if (randomPlay)
        {
            index = UnityEngine.Random.Range(0, soundsToPlay.Count);
        }

        var sound = soundsToPlay[index];
        AudioSource.PlayClipAtPoint(sound.clip, animator.gameObject.transform.position, sound.volume);

        // Advance to next sound or reset to beginning
        CurrentIndex = (CurrentIndex + 1) % soundsToPlay.Count;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get and cache prefab name on first enter
        if (string.IsNullOrEmpty(prefabName))
        {
            prefabName = GetPrefabName(animator.gameObject);
        }

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

            if(timeSinceEntered > soundsToPlay[CurrentIndex].delay)
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
