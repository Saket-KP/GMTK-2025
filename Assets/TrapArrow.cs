using UnityEngine;

// Main trap arrow behavior (2D version)
public class TrapArrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    public float speed = 100f;
    public float freezeDuration = 3f;
    public float lifetime = 10f; // Total time before arrow disappears
    public float stuckLifetime = 5f; // Time arrow stays after hitting something
    public LayerMask collisionLayers = -1;

    [Header("Visual Effects")]
    public GameObject freezeEffect; // Optional particle effect when frozen
    public Color normalColor = Color.white;
    public Color frozenColor = Color.cyan;

    private Rigidbody2D rb;
    private SpriteRenderer arrowRenderer;
    private Collider2D arrowCollider;
    private TrailRenderer trailRenderer;

    private bool isFrozen = false;
    private bool hasCollided = false;
    private Vector2 frozenVelocity;
    private float freezeTimer = 0f;
    private float lifetimeTimer = 0f;
    private float fadeStartTime = 1f; // Start fading 1 second before destruction

    // Static flag that can be triggered globally
    public static bool globalFreezeFlag = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        arrowRenderer = GetComponent<SpriteRenderer>();
        arrowCollider = GetComponent<Collider2D>();
        trailRenderer = GetComponent<TrailRenderer>();

        // Initialize lifetime timer
        lifetimeTimer = lifetime;

        // Debug checks
        if (rb == null)
        {
            Debug.LogError("TrapArrow: No Rigidbody2D found! Arrow won't move.");
            return;
        }

        // Ensure proper Rigidbody2D settings
        rb.gravityScale = 0f; // No gravity initially
        rb.drag = 0f; // No air resistance initially
        rb.angularDrag = 0f;
        rb.isKinematic = false;

        // Ensure the arrow moves forward (in 2D, use right)
        Vector2 shootDirection = transform.right * speed;
        rb.velocity = shootDirection;

        Debug.Log($"Arrow shot with velocity: {rb.velocity}, Speed: {speed}");

        // Optional: Add gravity after a delay for realistic arc
        if (speed > 0)
        {
            Invoke(nameof(EnableGravity), 0.5f);
        }
    }

    void EnableGravity()
    {
        if (rb != null && !hasCollided)
        {
            rb.gravityScale = 0.5f; // Slight downward arc
            rb.drag = 0.1f; // Slight air resistance
        }
    }

    void Update()
    {
        // Handle lifetime countdown (only when not frozen)
        if (!isFrozen)
        {
            lifetimeTimer -= Time.deltaTime;
        }

        // Check if arrow should be destroyed
        if (lifetimeTimer <= 0f)
        {
            DestroyArrow();
            return;
        }

        // Handle fade effect near end of lifetime
        HandleFadeEffect();

        // Check if we should freeze
        if (globalFreezeFlag && !isFrozen && !hasCollided)
        {
            FreezeArrow();
        }

        // Handle freeze timer
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                UnfreezeArrow();
            }
        }

        // Rotate arrow to face movement direction (2D)
        if (!isFrozen && rb != null && rb.velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void FreezeArrow()
    {
        if (rb == null) return;

        isFrozen = true;
        freezeTimer = freezeDuration;

        // Store current velocity and stop the arrow
        frozenVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // Visual feedback
        if (arrowRenderer != null)
        {
            arrowRenderer.color = frozenColor;
        }

        // Spawn freeze effect
        if (freezeEffect != null)
        {
            GameObject effect = Instantiate(freezeEffect, transform.position, transform.rotation);
            Destroy(effect, freezeDuration);
        }

        // Stop trail if present
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }

        Debug.Log("Arrow frozen for " + freezeDuration + " seconds");
    }

    void UnfreezeArrow()
    {
        if (rb == null) return;

        isFrozen = false;

        // Restore movement
        rb.isKinematic = false;
        rb.velocity = frozenVelocity;

        // Visual feedback
        if (arrowRenderer != null)
        {
            arrowRenderer.color = normalColor;
        }

        // Resume trail if present
        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
        }

        Debug.Log("Arrow unfrozen, resuming movement");
    }

    void HandleFadeEffect()
    {
        // Start fading when close to destruction
        if (lifetimeTimer <= fadeStartTime && arrowRenderer != null)
        {
            float fadeAlpha = lifetimeTimer / fadeStartTime;
            Color currentColor = arrowRenderer.color;
            currentColor.a = fadeAlpha;
            arrowRenderer.color = currentColor;

            // Also fade the trail if present
            if (trailRenderer != null)
            {
                Color trailColor = trailRenderer.material.color;
                trailColor.a = fadeAlpha;
                trailRenderer.material.color = trailColor;
            }
        }
    }

    void DestroyArrow()
    {
        Debug.Log("Arrow destroyed after lifetime expired");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Don't collide if frozen or already collided
        if (isFrozen || hasCollided) return;

        // Check if we hit something on the collision layers
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            HandleCollision(other);
        }
    }

    void HandleCollision(Collider2D other)
    {
        hasCollided = true;

        // Stop the arrow
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Stick to the object (optional)
        transform.SetParent(other.transform);

        // Set shorter lifetime for stuck arrows
        lifetimeTimer = Mathf.Min(lifetimeTimer, stuckLifetime);

        // Add your collision effects here (damage, sound, etc.)
        Debug.Log("Arrow hit: " + other.name + ", will disappear in " + stuckLifetime + " seconds");
    }

    // Public method to trigger freeze externally
    public void TriggerFreeze()
    {
        if (!isFrozen && !hasCollided)
        {
            FreezeArrow();
        }
    }
}
