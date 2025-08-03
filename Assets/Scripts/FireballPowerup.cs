using UnityEngine;

public class FireballPowerup : MonoBehaviour
{
    [Header("Collision Settings")]
    public string targetTag = "Player"; // Tag of the object to collide with
    public bool useTag = true; // Whether to check for specific tag or any collision
    
    [Header("Flag Reference")]
    public GameObject targetScript; // GameObject containing the script with the flag
    public string scriptName = "player"; // Name of the script class
    public string flagName = "gameFlag"; // Name of the flag variable
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnCollisionTriggered; // Unity Event for additional actions
    
    // Reference to the script component
    private Component targetComponent;
    
    void Start()
    {
        // Make sure this object has a collider
        if (GetComponent<Collider>() == null && GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning("CollisionHandler: No collider found on " + gameObject.name);
        }
        
        // Find the target script component
        if (targetScript != null)
        {
            targetComponent = targetScript.GetComponent(scriptName);
            if (targetComponent == null)
            {
                Debug.LogError($"Script '{scriptName}' not found on {targetScript.name}");
            }
        }
        else
        {
            Debug.LogWarning("Target script GameObject not assigned!");
        }
    }
    
    // For 3D collisions
    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }
    
    // Alternative: For collision instead of trigger
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }
    
    // For 2D collisions
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }
    
    // Alternative: For 2D collision instead of trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }
    
    void HandleCollision(GameObject collidingObject)
    {
        // Check if we should respond to this collision
        if (useTag && !collidingObject.CompareTag(targetTag))
        {
            return; // Ignore collision if tag doesn't match
        }
        
        // Set the flag in the target script
        SetFlagInTargetScript(true);
        
        // Log the collision
        Debug.Log($"{gameObject.name} collided with {collidingObject.name}. Flag triggered!");
        
        // Invoke Unity Event for additional actions
        OnCollisionTriggered?.Invoke();
        
        // Destroy this game object
        Destroy(gameObject);
    }
    
    void SetFlagInTargetScript(bool value)
    {
        if (targetComponent != null)
        {
            // Use reflection to set the flag
            var field = targetComponent.GetType().GetField(flagName);
            if (field != null && field.FieldType == typeof(bool))
            {
                field.SetValue(targetComponent, value);
                Debug.Log($"Flag '{flagName}' set to {value} in {scriptName}");
            }
            else
            {
                Debug.LogError($"Boolean field '{flagName}' not found in {scriptName}");
            }
        }
    }
    
    // Method to manually set the flag (callable from other scripts)
    public void SetFlag(bool value)
    {
        SetFlagInTargetScript(value);
    }
}