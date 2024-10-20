using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Color bulletColor;
    private Rigidbody rb;

    private float xBoundary = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;    
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > xBoundary)
        {
            Destroy(gameObject);
        }
    }

    // Set color
    public void SetColor(Color color)
    {
        bulletColor = color;
        GetComponent<Renderer>().material.color = color;
    }

    // Collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponent<Enemy>().enemyColor == bulletColor)
            {
                Destroy(other.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
