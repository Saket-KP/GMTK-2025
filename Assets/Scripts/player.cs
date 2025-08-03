using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed = 5f;
    public float defmovespeed = 5f;
    private float jumpForce;
    public float jumpForceenter = 10f;
    public float superJump = 20f;

    public bool touchslime = false;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    public float fireballcount = 7f;
    public float fireballcooldown = 1f;
    private float lastFireballTime = -1f;

    [Header("Invisibility Settings")]
    public float InvisDuration = 2f;
    public float InvisAlpha = 0.5f;
    public bool IsInvisible = false;

    [Header("Grip Shoes Settings")]
    public float GripDuration = 2f;

    public float slimespeed = 0.5f;
    public bool HasShoes = false;

    [Header("Power Ups - Toggles")]
    public bool activate = false;
    public bool jumppower = false;
    public bool fireballpower = false;
    public bool grapplingpower = false;
    public bool invispower = false;
    public bool timefreezepower = false;
    public bool gripshoespower = false;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private bool facingRight = true;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (fireballSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("FireballSpawnPoint");
            spawnPoint.transform.SetParent(transform);
            spawnPoint.transform.localPosition = new Vector3(0.5f, 0, 0);
            fireballSpawnPoint = spawnPoint.transform;
        }

        UpdateFireballSpawnPosition();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Slime")) { touchslime = true; }
        else touchslime = false;

        if (collision.collider.CompareTag("Spike"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (collision.collider.tag == "Dissappearing_Surface")
            {
                Destroy(collision.gameObject);
                Debug.Log("platform disappear");
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool firePressed = Input.GetMouseButtonDown(0);
        bool activatepressed = Input.GetMouseButtonDown(1);

        if (activatepressed) activate = !activate;

        // Invisibility Power-up Logic
        if (activate && invispower && !IsInvisible)
        {
            StartCoroutine(InvisibilityTimer());
        }

        // Grip Shoes Power-up Logic
        if (activate && gripshoespower && !HasShoes)
        {
            StartCoroutine(GripShoesTimer());
        }

        // Color Feedback
        if (!IsInvisible)
        {
            sr.color = activate ? Color.green : new Color(1f, 1f, 1f, 1f);
        }

        // Facing direction
        if (horizontalInput > 0 && !facingRight) Flip();
        else if (horizontalInput < 0 && facingRight) Flip();

        // Horizontal movement

        if (touchslime && !HasShoes) {  moveSpeed = slimespeed; }
        else moveSpeed = defmovespeed;
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        CheckGrounded();

        // Jumping
        if (jumpPressed && isGrounded)
        {
            if (jumppower && activate)
            {
                jumpForce = superJump;
                jumppower = false;
                activate = false;
            }
            else if (touchslime && !HasShoes) jumpForce = 0;
            else jumpForce = jumpForceenter;


            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Fireball
        if (firePressed && fireballpower && activate && fireballcount > 0 && (Time.time >= lastFireballTime + fireballcooldown))
        {
            ShootFireball();
            fireballcount--;
            lastFireballTime = Time.time;

            if (fireballcount == 0)
            {
                fireballpower = false;
                activate = false;
            }
        }
    }

    void CheckGrounded()
    {
        if (playerCollider == null) return;

        Bounds bounds = playerCollider.bounds;
        isGrounded = false;

        for (int i = -1; i <= 1; i++)
        {
            Vector2 rayStart = new Vector2(bounds.center.x + i * bounds.size.x * 0.3f, bounds.min.y);
            RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.1f);

            if (hit.collider != null && hit.collider != playerCollider)
            {
                if (rb.velocity.y <= 0.1f)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    void ShootFireball()
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("No fireball prefab assigned!");
            return;
        }

        Vector3 spawnPos = fireballSpawnPoint != null ? fireballSpawnPoint.position : transform.position;
        GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            fireballScript.SetDirection(direction);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        UpdateFireballSpawnPosition();
    }

    void UpdateFireballSpawnPosition()
    {
        if (fireballSpawnPoint != null)
        {
            float spawnDistance = 0.5f;
            fireballSpawnPoint.localPosition = new Vector3(
                facingRight ? spawnDistance : -spawnDistance,
                0.7f,
                0
            );
        }
    }

    void OnDrawGizmos()
    {
        if (playerCollider != null)
        {
            Bounds bounds = playerCollider.bounds;

            Gizmos.color = isGrounded ? Color.green : Color.red;
            for (int i = -1; i <= 1; i++)
            {
                Vector3 rayStart = new Vector3(bounds.center.x + i * bounds.size.x * 0.3f, bounds.min.y);
                Gizmos.DrawLine(rayStart, rayStart + Vector3.down * 0.1f);
            }
        }
    }

    private IEnumerator InvisibilityTimer()
    {
        IsInvisible = true;
        sr.color = new Color(1f, 1f, 1f, InvisAlpha);

        yield return new WaitForSeconds(InvisDuration);

        IsInvisible = false;
        sr.color = new Color(1f, 1f, 1f, 1f);
        invispower = false;
        activate = false;
    }

    private IEnumerator GripShoesTimer()
    {
        HasShoes = true;

        yield return new WaitForSeconds(GripDuration);

        HasShoes = false;
        gripshoespower = false;
        activate = false;
    }
}
