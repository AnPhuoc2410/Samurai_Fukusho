using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform launchPoint;

    public void LaunchProjectile()
    {
        Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
    }
}
