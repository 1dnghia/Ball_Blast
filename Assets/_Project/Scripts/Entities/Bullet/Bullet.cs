using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private BulletData bulletData;
    
    private Rigidbody2D rb;
    private float timer;
    private ObjectPool poolManager;
    private string poolName;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    /// <summary>
    /// Set pool manager và tên pool cho bullet này
    /// </summary>
    public void SetPool(ObjectPool manager, string name)
    {
        poolManager = manager;
        poolName = name;
    }
    
    private void OnEnable()
    {
        timer = 0f;
        // Đảm bảo có Rigidbody2D
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
            }
        }
        
        // Reset velocity và bắn đạn lên trên
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        float bulletSpeed = bulletData != null ? bulletData.speed : 15f;
        rb.velocity = Vector2.up * bulletSpeed;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        float maxLifetime = bulletData != null ? bulletData.lifetime : 3f;
        
        // Tự động trả về pool sau khi hết lifetime
        if (timer >= maxLifetime)
        {
            ReturnToPool();
        }
    }
    
    public int GetDamage()
    {
        return bulletData != null ? bulletData.damage : 1;
    }
    
    /// <summary>
    /// Gọi bởi Obstacle khi bullet chạm vào
    /// </summary>
    public void OnHitObstacle()
    {
        // Bullet biến mất ngay khi chạm Obstacle
        ReturnToPool();
    }
    
    private void OnBecameInvisible()
    {
        // Trả về pool khi ra khỏi camera
        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        // Nếu có pool manager, trả về pool để tái sử dụng
        if (poolManager != null && !string.IsNullOrEmpty(poolName))
        {
            poolManager.ReturnObject(poolName, gameObject);
        }
        else
        {
            // Fallback: chỉ disable nếu không có pool
            gameObject.SetActive(false);
        }
    }
}
