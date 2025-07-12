using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform launchPoint;

    public void LaunchProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
        Vector3 originScale = projectile.transform.localScale;


        projectile.transform.localScale = new Vector3(
            originScale.x * transform.localScale.x > 0 ? 1 : -1,
            originScale.y,
            originScale.z
        );
    }
}
