using UnityEngine;
using System.Collections;

/// <summary>
/// Quản lý di chuyển và physics của Obstacle
/// </summary>
public class ObstacleMovement : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnHeightRatio = 0.6f;
    [SerializeField] private float maxSpawnHeightRatio = 0.9f;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private Transform visualTransform;
    
    private Vector3 moveDirection;
    private Camera mainCamera;
    private Rigidbody2D rb2D;
    private Collider2D col2D;
    private ObstacleData obstacleData;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        rb2D = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
    }
    
    private void Update()
    {
        UpdateMovement();
        RotateVisual();
    }
    
    public void Initialize(ObstacleData data)
    {
        obstacleData = data;
        ResetPhysics();
        SpawnAtRandomEdge();
    }
    
    public void InitializeAsChild(ObstacleData data, Vector3 position, float launchAngle, float launchForce)
    {
        obstacleData = data;
        transform.position = position;
        
        // Calculate direction
        float radians = launchAngle * Mathf.Deg2Rad;
        float horizontalDir = Mathf.Sin(radians);
        moveDirection = new Vector3(horizontalDir, 0f, 0f).normalized;
        
        // Apply launch force
        Vector3 launchDirection = new Vector3(horizontalDir, 1f, 0f).normalized;
        StartCoroutine(ApplyLaunchForce(launchDirection, launchForce));
    }
    
    public bool IsOffScreen()
    {
        float screenHeight = mainCamera.orthographicSize * 2f;
        return transform.position.y < -screenHeight / 2f - 2f;
    }
    
    public void ResetPhysics()
    {
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
            rb2D.gravityScale = 0f;
        }
    }
    
    private void SpawnAtRandomEdge()
    {
        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;
        
        float spawnHeight = Random.Range(
            screenHeight * minSpawnHeightRatio,
            screenHeight * maxSpawnHeightRatio
        );
        
        bool spawnFromLeft = Random.value > 0.5f;
        
        if (spawnFromLeft)
        {
            transform.position = new Vector3(-screenWidth / 2f - 1f, spawnHeight - screenHeight / 2f, 0f);
            moveDirection = Vector3.right;
        }
        else
        {
            transform.position = new Vector3(screenWidth / 2f + 1f, spawnHeight - screenHeight / 2f, 0f);
            moveDirection = Vector3.left;
        }
        
        StartCoroutine(StartFalling());
    }
    
    private void UpdateMovement()
    {
        if (obstacleData == null || rb2D == null) return;
        
        // Horizontal movement
        Vector2 velocity = rb2D.velocity;
        velocity.x = moveDirection.x * obstacleData.horizontalSpeed;
        rb2D.velocity = velocity;
        
        CheckScreenBounds();
    }
    
    private void CheckScreenBounds()
    {
        if (col2D == null) return;
        
        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;
        float halfWidth = screenWidth / 2f;
        
        Bounds bounds = col2D.bounds;
        
        if (bounds.min.x <= -halfWidth && moveDirection.x < 0)
        {
            moveDirection = Vector3.right;
            transform.position = new Vector3(-halfWidth + col2D.bounds.extents.x, transform.position.y, transform.position.z);
        }
        else if (bounds.max.x >= halfWidth && moveDirection.x > 0)
        {
            moveDirection = Vector3.left;
            transform.position = new Vector3(halfWidth - col2D.bounds.extents.x, transform.position.y, transform.position.z);
        }
    }
    
    private void RotateVisual()
    {
        if (visualTransform != null)
        {
            visualTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
    
    private IEnumerator StartFalling()
    {
        float delay = obstacleData != null ? obstacleData.fallDelay : 1f;
        yield return new WaitForSeconds(delay);
        
        if (rb2D != null && obstacleData != null)
        {
            float targetFallSpeed = obstacleData.fallSpeed;
            float gravityY = Mathf.Abs(Physics2D.gravity.y);
            rb2D.gravityScale = gravityY > 0 ? targetFallSpeed / gravityY : 1f;
        }
    }
    
    private IEnumerator ApplyLaunchForce(Vector3 direction, float force)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        
        if (rb2D != null && obstacleData != null)
        {
            // Set gravity to 0 temporarily during launch
            rb2D.gravityScale = 0f;
            
            // Apply launch force
            while (elapsed < duration)
            {
                rb2D.velocity = direction * force;
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Enable gravity immediately after launch
            float targetFallSpeed = obstacleData.fallSpeed;
            float gravityY = Mathf.Abs(Physics2D.gravity.y);
            rb2D.gravityScale = gravityY > 0 ? targetFallSpeed / gravityY : 1f;
        }
    }
}
