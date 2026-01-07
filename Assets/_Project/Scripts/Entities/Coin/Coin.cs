using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float collectRadius = 1f;
    [SerializeField] private float gravity = 9.8f;
    
    [Header("Effects")]
    [SerializeField] private GameObject collectEffectPrefab;
    
    private Transform playerTransform;
    private bool isMovingToPlayer = false;
    private bool isOnGround = false;
    private string poolName = "Coin";
    private Vector2 velocity = Vector2.zero;
    private Camera mainCamera;
    private CircleCollider2D coinCollider;
    
    private void Start()
    {
        // Tìm weaponroot (player)
        GameObject weaponRoot = GameObject.Find("weaponroot");
        if (weaponRoot != null)
        {
            playerTransform = weaponRoot.transform;
        }
        
        // Get camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Get coin collider
        coinCollider = GetComponent<CircleCollider2D>();
    }
    
    private void OnEnable()
    {
        // Reset khi được spawn từ pool
        isMovingToPlayer = false;
        isOnGround = false;
        velocity = Vector2.zero;
    }
    
    private void Update()
    {
        if (isMovingToPlayer)
        {
            MoveToPlayer();
        }
        else if (!isOnGround)
        {
            ApplyGravityAndMove();
            CheckScreenBounds();
            CheckGroundCollision();
        }
    }
    
    private void MoveToPlayer()
    {
        if (playerTransform == null) return;
        
        // Di chuyển về phía player
        transform.position = Vector3.MoveTowards(
            transform.position, 
            playerTransform.position, 
            moveSpeed * Time.deltaTime
        );
        
        // Nếu đã chạm player thì collect
        if (Vector3.Distance(transform.position, playerTransform.position) < 0.2f)
        {
            CollectCoin();
        }
    }
    
    private void CollectCoin()
    {
        // Spawn effect nếu có
        if (collectEffectPrefab != null)
        {
            GameObject effect = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Publish event để tăng coin
        EventBus.Publish(new CoinCollectedEvent(coinValue));
        
        // Return về pool
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.ReturnObject(poolName, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Set coin value khi spawn
    /// </summary>
    public void SetCoinValue(int value)
    {
        coinValue = value;
    }
    
    /// <summary>
    /// Bắt đầu di chuyển về phía player (gọi từ WeaponRoot)
    /// </summary>
    public void StartMovingToPlayer(Transform target)
    {
        playerTransform = target;
        isMovingToPlayer = true;
    }
    
    /// <summary>
    /// Set pool name cho object pooling
    /// </summary>
    public void SetPoolName(string name)
    {
        poolName = name;
    }
    
    /// <summary>
    /// Áp dụng force ngẫu nhiên để coin rơi theo đường cong
    /// </summary>
    public void ApplyRandomForce(float minForce = 2f, float maxForce = 5f)
    {
        // Random hướng (trái hoặc phải)
        float horizontalDirection = Random.value > 0.5f ? 1f : -1f;
        float horizontalForce = Random.Range(minForce, maxForce) * horizontalDirection;
        
        // Force hướng lên một chút để tạo đường cong
        float upwardForce = Random.Range(minForce * 0.5f, maxForce * 0.5f);
        
        // Set velocity ban đầu
        velocity = new Vector2(horizontalForce, upwardForce);
    }
    
    /// <summary>
    /// Kiểm tra và bật lại khi chạm cạnh màn hình
    /// </summary>
    private void CheckScreenBounds()
    {
        if (mainCamera == null) return;
        
        // Get screen bounds
        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;
        
        // Kiểm tra cạnh trái/phải
        if (transform.position.x < -screenWidth / 2f || transform.position.x > screenWidth / 2f)
        {
            // Đảo hướng velocity.x để bật lại
            velocity.x = -velocity.x;
            
            // Clamp position về trong màn hình
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -screenWidth / 2f, screenWidth / 2f),
                transform.position.y,
                transform.position.z
            );
        }
    }
    
    /// <summary>
    /// Áp dụng trọng lực và di chuyển coin
    /// </summary>
    private void ApplyGravityAndMove()
    {
        // Áp dụng trọng lực
        velocity.y -= gravity * Time.deltaTime;
        
        // Di chuyển coin
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
    
    /// <summary>
    /// Kiểm tra chạm ground bằng CircleCast
    /// </summary>
    private void CheckGroundCollision()
    {
        if (coinCollider == null) return;
        
        float radius = coinCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
        
        // CircleCast xuống dưới để check ground (tính đến kích thước collider)
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        
        if (hit.collider != null)
        {
            isOnGround = true;
            velocity = Vector2.zero;
            
            // Snap về vị trí sao cho bottom của coin chạm ground
            float bottomY = hit.point.y + radius;
            transform.position = new Vector3(transform.position.x, bottomY, transform.position.z);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Vẽ bán kính collect trong editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
        
        // Vẽ raycast check ground
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);
    }
}
