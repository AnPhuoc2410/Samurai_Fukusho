using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum CounterMode
{
    PerInstance,    // Each instance has its own counter
    PerPrefab,      // All instances of the same prefab share a counter
    GlobalScene,    // All instances in current scene share a counter
    GlobalGame      // All instances across all scenes share a counter
}

[Serializable]
public class SoundData
{
    public AudioClip clip;
    public float volume = 1.0f;
    public float delay = 0.25f;
}

public class AnimaState_MultiSFX : StateMachineBehaviour
{
    [Header("Playback Options")]
    public bool randomPlay = false;
    [Tooltip("Determines how the sound counter is shared between instances")]
    public CounterMode counterMode = CounterMode.PerInstance;

    [Header("Sound Settings")]
    public List<SoundData> soundsToPlay = new List<SoundData>();
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;

    // Counters for different modes
    private static int globalGameCounter = 0;
    private static Dictionary<string, int> globalSceneCounters = new Dictionary<string, int>();
    private static Dictionary<string, int> prefabCounters = new Dictionary<string, int>();
    private int instanceCounter = 0;
    
    // Sound sequence tracking
    private string prefabName;
    private string sceneName;
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
            switch (counterMode)
            {
                case CounterMode.GlobalGame:
                    return globalGameCounter;

                case CounterMode.GlobalScene:
                    if (!globalSceneCounters.ContainsKey(sceneName))
                        globalSceneCounters[sceneName] = 0;
                    return globalSceneCounters[sceneName];

                case CounterMode.PerPrefab:
                    if (!prefabCounters.ContainsKey(prefabName))
                        prefabCounters[prefabName] = 0;
                    return prefabCounters[prefabName];

                case CounterMode.PerInstance:
                default:
                    return instanceCounter;
            }
        }
        set
        {
            switch (counterMode)
            {
                case CounterMode.GlobalGame:
                    globalGameCounter = value;
                    break;

                case CounterMode.GlobalScene:
                    globalSceneCounters[sceneName] = value;
                    break;

                case CounterMode.PerPrefab:
                    prefabCounters[prefabName] = value;
                    break;

                case CounterMode.PerInstance:
                default:
                    instanceCounter = value;
                    break;
            }
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
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset scene-specific counters when a new scene is loaded
        globalSceneCounters.Clear();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cache names on first enter
        if (string.IsNullOrEmpty(prefabName))
        {
            prefabName = GetPrefabName(animator.gameObject);
            sceneName = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += OnSceneLoaded;
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
