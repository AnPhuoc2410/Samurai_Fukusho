using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;

/// <summary>
/// Manages scene transitions and player spawning across scenes
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Create a new instance if one doesn't exist
                GameObject sceneManager = new GameObject("SceneTransitionManager");
                instance = sceneManager.AddComponent<SceneTransitionManager>();
                DontDestroyOnLoad(sceneManager);
            }
            return instance;
        }
    }

    [Header("Player Spawn Data")]
    private Vector3 playerSpawnPosition = Vector3.zero;
    private bool useCustomSpawnPosition = false;
    [Tooltip("Whether to persist player data between scenes")]
    public bool persistPlayerData = false; // New field to control player data persistence
    private string targetSceneName = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initiates scene transition with spawn position data
    /// </summary>
    /// <param name="sceneName">Target scene name</param>
    /// <param name="spawnPosition">Position to spawn player</param>
    /// <param name="useCustomSpawn">Whether to use custom spawn position</param>
    public void TransitionToScene(string sceneName, Vector3 spawnPosition, bool useCustomSpawn = true)
    {
        targetSceneName = sceneName;
        playerSpawnPosition = spawnPosition;
        useCustomSpawnPosition = useCustomSpawn;

        Debug.Log($"Transitioning to {sceneName} with spawn position: {spawnPosition}");

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Initiates scene transition without custom spawn position
    /// </summary>
    /// <param name="sceneName">Target scene name</param>
    public void TransitionToScene(string sceneName)
    {
        TransitionToScene(sceneName, Vector3.zero, false);
    }

    /// <summary>
    /// Transitions to a new scene without persisting player data
    /// </summary>
    /// <param name="sceneName">Target scene name</param>
    public void TransitionToSceneNewPlayer(string sceneName)
    {
        // Set flag to indicate we don't want to persist player data
        useCustomSpawnPosition = false;
        persistPlayerData = false;

        // Start scene loading process
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Only set up player if we're persisting data
        if (persistPlayerData)
        {
            SetupSceneAfterLoad();
        }
        else
        {
            // For new player mode, just make sure time scale is normal
            Time.timeScale = 1f;
        }
    }

    private void SetupSceneAfterLoad()
    {
        StartCoroutine(SetupSceneCoroutine());
    }

    private IEnumerator SetupSceneCoroutine()
    {
        // Wait multiple frames to ensure everything is initialized
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Find and setup player first
        SetupPlayer();

        // Wait another frame before setting up camera and effects
        yield return new WaitForEndOfFrame();

        // Setup camera
        SetupCamera();

        // Setup parallax effects
        UpdateParallaxEffects();

        // Final check and setup
        yield return new WaitForEndOfFrame();
        FinalSceneSetup();
    }

    private void SetupPlayer()
    {
        // Skip if we're not persisting player data
        if (!persistPlayerData) return;

        // Find the player in the new scene
        GameObject player = FindPlayer();

        if (player != null)
        {
            // Move player to spawn position if using custom spawn
            if (useCustomSpawnPosition)
            {
                player.transform.position = playerSpawnPosition;
                Debug.Log($"Player spawned at custom position: {playerSpawnPosition}");
            }
            else
            {
                Debug.Log($"Player spawned at default position: {player.transform.position}");
            }

            // Ensure player has proper physics setup
            SetupPlayerPhysics(player);

            // Ensure player tag is set correctly
            if (!player.CompareTag("Player"))
            {
                player.tag = "Player";
                Debug.Log("Fixed player tag");
            }
        }
        else
        {
            Debug.LogError("Player not found in the new scene!");
        }
    }

    private GameObject FindPlayer()
    {
        // Try finding by tag first
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            // Try finding by layer
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                if (obj.layer == LayerMask.NameToLayer("Player"))
                {
                    player = obj;
                    break;
                }
            }
        }

        if (player == null)
        {
            // Try finding by name
            player = GameObject.Find("Player");
        }

        return player;
    }

    private void SetupPlayerPhysics(GameObject player)
    {
        // Ensure player has proper Rigidbody2D setup
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Reset velocity to prevent falling through ground
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Ensure proper gravity and physics settings
            rb.gravityScale = 1f; // Make sure gravity is enabled

            Debug.Log("Player physics reset successfully");
        }
        else
        {
            Debug.LogWarning("Player does not have Rigidbody2D component!");
        }

        // Ensure PlayerInventory is connected and persisted
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            // PlayerInventory should be a singleton, try to get the instance
            inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                Debug.Log("PlayerInventory singleton found and connected");
            }
            else
            {
                Debug.LogError("PlayerInventory not found! This may cause key checking issues.");
            }
        }
        else
        {
            Debug.Log("PlayerInventory component found on player");
        }
    }

    private void SetupCamera()
    {
        // Find Cinemachine Virtual Camera first
        CinemachineCamera vCam = FindFirstObjectByType<CinemachineCamera>();
        GameObject player = FindPlayer();

        if (vCam != null && player != null)
        {
            // Set the camera to follow the player
            vCam.Follow = player.transform;
            vCam.LookAt = player.transform;

            Debug.Log("Cinemachine Virtual Camera setup complete - now following player");
        }
        else if (vCam == null)
        {
            Debug.LogWarning("Cinemachine Virtual Camera not found in scene");
        }
        else if (player == null)
        {
            Debug.LogError("Player not found for camera setup");
        }

        // Also setup regular camera if no Cinemachine is found
        if (vCam == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null && player != null)
            {
                // Try to find a camera follow script
                MonoBehaviour[] cameraScripts = mainCamera.GetComponents<MonoBehaviour>();
                foreach (var script in cameraScripts)
                {
                    // Check if it has a followTarget field (like CameraController)
                    var followTargetField = script.GetType().GetField("followTarget");
                    if (followTargetField != null)
                    {
                        followTargetField.SetValue(script, player.transform);
                        Debug.Log($"Set followTarget on {script.GetType().Name}");
                    }

                    // Check for CameraTarget field (like in TextMesh Pro CameraController)
                    var cameraTargetField = script.GetType().GetField("CameraTarget");
                    if (cameraTargetField != null)
                    {
                        cameraTargetField.SetValue(script, player.transform);
                        Debug.Log($"Set CameraTarget on {script.GetType().Name}");
                    }
                }
            }
        }
    }

    private void UpdateParallaxEffects()
    {
        ParalaxEffect[] parallaxEffects = FindObjectsByType<ParalaxEffect>(FindObjectsSortMode.None);
        GameObject player = FindPlayer();
        Camera mainCamera = Camera.main;

        if (player != null)
        {
            foreach (ParalaxEffect parallax in parallaxEffects)
            {
                // Set the follow target to the player
                parallax.followTarget = player.transform;

                // Set the camera reference if it's missing
                if (parallax.cam == null && mainCamera != null)
                {
                    parallax.cam = mainCamera;
                }

                // Reset the parallax effect for the new scene
                parallax.ResetParallax();

                Debug.Log($"Updated ParallaxEffect on {parallax.gameObject.name} to follow player");
            }

            if (parallaxEffects.Length > 0)
            {
                Debug.Log($"Updated {parallaxEffects.Length} parallax effects to follow player");
            }
        }
        else
        {
            Debug.LogError("Player not found for parallax effects setup");
        }
    }

    private void FinalSceneSetup()
    {
        GameObject player = FindPlayer();

        if (player != null)
        {
            // Final validation - ensure player is properly positioned
            if (useCustomSpawnPosition)
            {
                // Double-check position is correct
                if (Vector3.Distance(player.transform.position, playerSpawnPosition) > 0.1f)
                {
                    player.transform.position = playerSpawnPosition;
                    Debug.Log("Final position correction applied");
                }
            }

            // Ensure player is not falling through the ground
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            Debug.Log("Scene setup complete!");
        }
    }

    /// <summary>
    /// Get current spawn position (for debugging)
    /// </summary>
    public Vector3 GetSpawnPosition()
    {
        return playerSpawnPosition;
    }

    /// <summary>
    /// Check if using custom spawn position (for debugging)
    /// </summary>
    public bool IsUsingCustomSpawn()
    {
        return useCustomSpawnPosition;
    }
}
