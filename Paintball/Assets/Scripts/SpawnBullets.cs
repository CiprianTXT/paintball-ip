using System.Collections;
using UnityEngine;

public class SpawnBullets : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to spawn bullets
        StartCoroutine(SpawnBulletRoutine());
    }

    // Coroutine to spawn bullets at regular intervals
    IEnumerator SpawnBulletRoutine()
    {
        while (true)
        {
            // Instantiate a new bullet at the spawn point
            GameObject newBullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

            // Wait for the specified interval before spawning the next bullet
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

