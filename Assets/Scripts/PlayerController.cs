using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform barrel;
    public Transform bulletSpawner;
    public EnemyController enemyController;
    public GameObject gameOverPanel;

    public float shootingInterval = 2f;
    public float detectionRadius = 10;

    public float rotationSpeed = 5f;
    public Color[] playerColors;

    private List<GameObject> enemiesInRange = new List<GameObject>(); 
    private GameObject currentTarget = null;

    private float nextShootTime = 0f;

    private int currentColorIndex = 0;
    private Renderer playerRenderer;
    private Renderer barrelRenderer;

    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        barrelRenderer = barrel.GetComponent<Renderer>();
        ChangeColor();

        InvokeRepeating("CheckForEnemies", 0f, 0.1f);

        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchColor();
        }

        if (currentTarget != null && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootingInterval;
        }
    }

    // Check for enemies
    private void CheckForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy") && !enemiesInRange.Contains(col.gameObject))
            {
                enemiesInRange.Add(col.gameObject);
                if (currentTarget == null)
                {
                    currentTarget = col.gameObject;
                }
            }
        }

        enemiesInRange.RemoveAll(enemy => enemy == null);
        if (currentTarget == null && enemiesInRange.Count > 0)
        {
            currentTarget = enemiesInRange[0];
        }
    }

    // Fixed update
    private void FixedUpdate()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // Shooting
    void Shoot()
    {
        if (currentTarget == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawner.position, bulletSpawner.rotation);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 10f;
        bullet.GetComponent<Bullet>().SetColor(playerColors[currentColorIndex]);

        enemiesInRange.Remove(currentTarget);
        if (enemiesInRange.Count > 0)
        {
            currentTarget = enemiesInRange[0];
        }
        else
        {
            currentTarget = null;
        }
    }

    // Switch Colors
    void SwitchColor()
    {
        currentColorIndex = (currentColorIndex + 1) % playerColors.Length;
        ChangeColor();
    }

    // Change Colors
    void ChangeColor()
    {
        playerRenderer.material.color = playerColors[currentColorIndex];
        barrelRenderer.material.color = playerColors[currentColorIndex];
    }

    // Current Color
    public Color GetCurrentColor()
    {
        return playerColors[currentColorIndex];
    }

    // Collision Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TriggerGameOver();
        }
    }

    // Trigger Panel
    void TriggerGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);

        DestroyAllEnemiesAndBullets();

        if (enemyController != null)
        {
            enemyController.StopSpawning();
        }
    }

    // Destroy assets
    void DestroyAllEnemiesAndBullets()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        gameObject.SetActive(false);
    }
}
