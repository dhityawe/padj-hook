using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    private List<Enemy> activeEnemies = new List<Enemy>();
    private List<Enemy> inactiveEnemies = new List<Enemy>();

    [SerializeField]
    private Enemy enemyPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    public static Enemy SpawnEnemy()
    {
        Enemy enemyToSpawn;

        if (Instance.inactiveEnemies.Count > 0)
        {
            enemyToSpawn = Instance.inactiveEnemies[0];
            Instance.inactiveEnemies.RemoveAt(0);
            Instance.activeEnemies.Add(enemyToSpawn);   
        }

        else
        {
            enemyToSpawn = Instantiate(Instance.enemyPrefab);
            Instance.activeEnemies.Add(enemyToSpawn);
        }

        enemyToSpawn.gameObject.SetActive(true);
        enemyToSpawn.OnSpawn();
        
        return enemyToSpawn;
    }

    public static void DestroyEnemy(Enemy enemy)
    {
        Instance.activeEnemies.Remove(enemy);
        Instance.inactiveEnemies.Add(enemy);
        enemy.gameObject.SetActive(false);
    }
}
