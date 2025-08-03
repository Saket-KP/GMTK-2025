using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public bool autoShoot = true;
    public float y_value;

    [Header("Trigger Settings")]
    public KeyCode manualShootKey = KeyCode.Space;
    public KeyCode freezeTriggerKey = KeyCode.F;
    public Transform player;
    public bool shot = false;

    private float shootTimer = 0f;

    void Start()
    {
        if (shootPoint == null)
            shootPoint = transform;
    }

    void Update()
    {
        // Auto shooting
        if (autoShoot && player != null)
        {
            if (player.position.y >= y_value && !shot)
            {
                ShootArrow();
                shot = true;
            }
        }

        // Manual shooting
        // if (Input.GetKeyDown(manualShootKey))
        // {
        //     ShootArrow();
        // }

        // Freeze trigger
        if (Input.GetKeyDown(freezeTriggerKey))
        {
            TriggerGlobalFreeze();
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("ArrowShooter: No arrow prefab assigned!");
            return;
        }

        if (shootPoint == null)
        {
            Debug.LogError("ArrowShooter: No shoot point assigned!");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);

        // Ensure the arrow has proper initial velocity
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        TrapArrow arrowScript = arrow.GetComponent<TrapArrow>();

        if (arrowRb != null && arrowScript != null)
        {
            // Clear any existing velocity
            arrowRb.velocity = Vector2.zero;
            arrowRb.angularVelocity = 0f;

            // Set the shooting direction in 2D
            Vector2 shootDirection = shootPoint.right; // In 2D, "right" is usually forward
            arrowRb.velocity = shootDirection * arrowScript.speed;

            Debug.Log($"Arrow shot from {shootPoint.position} in direction {shootDirection} with velocity {arrowRb.velocity}");
        }
        else
        {
            Debug.LogError("Arrow prefab missing Rigidbody2D or TrapArrow script!");
        }
    }

    public void TriggerGlobalFreeze()
    {
        TrapArrow.globalFreezeFlag = true;
        Debug.Log("Global freeze triggered!");

        // Reset the flag after a brief moment so it only affects arrows once
        Invoke(nameof(ResetFreezeFlag), 0.1f);
    }

    void ResetFreezeFlag()
    {
        TrapArrow.globalFreezeFlag = false;
    }
}

// Alternative freeze trigger system (more flexible)
public class FreezeTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public bool triggerOnStart = false;
    public float triggerDelay = 0f;
    public KeyCode triggerKey = KeyCode.T;

    void Start()
    {
        if (triggerOnStart)
        {
            Invoke(nameof(TriggerFreeze), triggerDelay);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            TriggerFreeze();
        }
    }

    public void TriggerFreeze()
    {
        // Find all arrows in the scene and freeze them
        TrapArrow[] arrows = FindObjectsOfType<TrapArrow>();
        foreach (TrapArrow arrow in arrows)
        {
            arrow.TriggerFreeze();
        }

        Debug.Log($"Triggered freeze on {arrows.Length} arrows");
    }

    // Can be called from other scripts or Unity Events
    public void TriggerFreezeExternal()
    {
        TriggerFreeze();
    }
}
