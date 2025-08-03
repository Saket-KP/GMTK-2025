using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float speed = 10f;
    public float maxDistance = 8f;
    public float lifetime = 3f; // Backup timer in case distance check fails
    
    private Vector3 startPosition;
    private Vector2 direction;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        
        // Set velocity in the direction we're facing
        rb.velocity = direction * speed;
        
        // Destroy after lifetime as backup
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Check if traveled max distance
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            DestroyFireball();
        }
    }
    
    public void SetDirection(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Don't collide with the player who shot it
        if (other.CompareTag("Player"))
            return;
            
        // Hit something - destroy fireball
        Debug.Log("Fireball hit: " + other.name);
        DestroyFireball();
    }
    
    void DestroyFireball()
    {
        // Add explosion effect here later if wanted
        Destroy(gameObject);
    }
}