using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 2f;
    public Color[] enemyColors;
    private Transform player;

    private bool canSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval);    
    }

    // Spawning
    void SpawnEnemy()
    {
        if (!canSpawn) return;

        Vector3 spawnPosition = GetRandomPositionOutsideCircle();

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Color randomColor = enemyColors[Random.Range(0, enemyColors.Length)];
        enemy.GetComponent<Enemy>().SetColor(randomColor);
    }

    // Spawn Position
    Vector3 GetRandomPositionOutsideCircle()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);

        float radius = spawnRadius + Random.Range(1f, 3f);

        float x = player.position.x + radius * Mathf.Cos(angle);
        float z = player.position.z + radius * Mathf.Sin(angle);

        return new Vector3(x, 1, z); 
    }

    public void StopSpawning()
    {
        canSpawn = false;
        CancelInvoke("SpawnEnemy");
    }    
}
