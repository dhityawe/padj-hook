using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<Transform> rightWaypoints = new List<Transform>();

    [SerializeField]
    private List<Transform> leftWaypoints = new List<Transform>();

    [SerializeField]
    [Range(0, 100)]
    private float spawnRate = 50f;

    [SerializeField] private float feverTimeCooldown = 20f;
    [SerializeField] private float feverTimeDuration = 10f;

    [SerializeField] private List<GameObject> lightings;

    private float spawnTimer;
    private bool isFeverActive = false;
    private float feverTimer = 0f;
    private float feverCooldownTimer = 0f;
    private float originalSpawnRate;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (!isFeverActive)
        {
            feverCooldownTimer += Time.deltaTime;
            if (feverCooldownTimer >= feverTimeCooldown)
            {
                ActivateFever();
                feverCooldownTimer = 0f;
            }
        }
        else
        {
            feverTimer += Time.deltaTime;
            if (feverTimer >= feverTimeDuration)
            {
                DeactivateFever();
            }
        }

        if (spawnTimer >= 1f)
        {
            spawnTimer = 0f;
            if (Random.Range(0, 100) < spawnRate)
            {
                SpawnEnemy();
            }
        }
    }
    private void ActivateFever()
    {
        isFeverActive = true;
        feverTimer = 0f;
        originalSpawnRate = spawnRate;
        spawnRate *= 2f;
        Debug.Log("Fever Time Activated! Spawn rate increased.");
    }

    private void DeactivateFever()
    {
        isFeverActive = false;
        spawnRate = originalSpawnRate;
        Debug.Log("Fever Time Deactivated. Spawn rate normalized.");
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, 2);
        Transform spawnPoint = randomIndex == 0 ? rightWaypoints[Random.Range(0, rightWaypoints.Count)] : leftWaypoints[Random.Range(0, leftWaypoints.Count)];
        Transform waypoint = randomIndex == 0 ? leftWaypoints[Random.Range(0, leftWaypoints.Count)] : rightWaypoints[Random.Range(0, rightWaypoints.Count)];

        Enemy enemy = EnemyPool.SpawnEnemy();
        enemy.SetSpeed(Random.Range(1f, 3f));
        enemy.transform.position = spawnPoint.position;
        enemy.GetWaypoint().position = waypoint.position;
        
        enemy.SetWaypointParent(waypoint);
        enemy.transform.parent = transform;
    }
}
