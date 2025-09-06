using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyConfig
{
    public Enemy EnemyPrefab;
    public float SpawnChance;
}

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    private List<Enemy> activeEnemies = new List<Enemy>();
    private List<Enemy> inactiveEnemies = new List<Enemy>();

    [SerializeField]
    private List<EnemyConfig> enemyConfigs;

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

        EnemyConfig selectedConfig = null;
        float totalChance = 0f;
        foreach (var config in Instance.enemyConfigs)
        {
            totalChance += config.SpawnChance;
        }
        float randomValue = Random.value * totalChance;
        float cumulative = 0f;
        foreach (var config in Instance.enemyConfigs)
        {
            cumulative += config.SpawnChance;
            if (randomValue <= cumulative)
            {
                selectedConfig = config;
                break;
            }
        }
        if (selectedConfig == null && Instance.enemyConfigs.Count > 0)
        {
            selectedConfig = Instance.enemyConfigs[0]; // fallback
        }

        enemyToSpawn = null;
        if (selectedConfig != null)
        {
            for (int i = 0; i < Instance.inactiveEnemies.Count; i++)
            {
                if (Instance.inactiveEnemies[i].GetType() == selectedConfig.EnemyPrefab.GetType())
                {
                    enemyToSpawn = Instance.inactiveEnemies[i];
                    Instance.inactiveEnemies.RemoveAt(i);
                    break;
                }
            }
        }
        if (enemyToSpawn == null && selectedConfig != null)
        {
            enemyToSpawn = Instantiate(selectedConfig.EnemyPrefab);
        }
        if (enemyToSpawn != null)
        {
            Instance.activeEnemies.Add(enemyToSpawn);
            enemyToSpawn.gameObject.SetActive(true);
            enemyToSpawn.OnSpawn();
        }
        return enemyToSpawn;
    }

    public static void DestroyEnemy(Enemy enemy)
    {
        Instance.activeEnemies.Remove(enemy);
        Instance.inactiveEnemies.Add(enemy);
        enemy.gameObject.SetActive(false);
    }
}
