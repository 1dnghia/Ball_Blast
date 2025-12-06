using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 3f;
    
    private Rigidbody2D rb;
    private float timer;
    private ObjectPool poolManager; // Reference đến ObjectPool manager
    private string poolName; // Tên pool (VD: "BulletPool")
    
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
        rb.velocity = Vector2.up * speed;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        // Tự động trả về pool sau khi hết lifetime
        if (timer >= lifetime)
        {
            ReturnToPool();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Xử lý va chạm với enemy hoặc obstacles
        // Kiểm tra tag an toàn (không báo lỗi nếu tag chưa tồn tại)
        string tag = collision.gameObject.tag;
        
        if (tag == "Enemy" || tag == "Obstacle")
        {
            // Gây damage cho enemy (bạn có thể implement sau)
            ReturnToPool();
        }
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
