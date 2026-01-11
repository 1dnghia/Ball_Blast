using UnityEngine;

/// <summary>
/// Main coordinator cho Obstacle - gọn gàng, dễ đọc
/// </summary>
[RequireComponent(typeof(ObstacleHealth))]
[RequireComponent(typeof(ObstacleMovement))]
[RequireComponent(typeof(ObstacleSplitter))]
[RequireComponent(typeof(ObstacleCoinDropper))]
public class Obstacle : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ObstacleData obstacleData;
    
    private ObstacleHealth health;
    private ObstacleMovement movement;
    private ObstacleSplitter splitter;
    private ObstacleCoinDropper coinDropper;
    private string poolName;
    
    private void Awake()
    {
        health = GetComponent<ObstacleHealth>();
        movement = GetComponent<ObstacleMovement>();
        splitter = GetComponent<ObstacleSplitter>();
        coinDropper = GetComponent<ObstacleCoinDropper>();
    }
    
    private void Update()
    {
        if (movement.IsOffScreen())
        {
            ReturnToPool();
        }
    }
    
    #region Initialization
    
    public void Initialize(string pool)
    {
        Initialize(pool, 1f, -1, null);
    }
    
    public void Initialize(string pool, float scale, int hp = -1)
    {
        Initialize(pool, scale, hp, null);
    }
    
    public void Initialize(string pool, float scale, int hp, PathData pathData)
    {
        InitializeWithSplitCount(pool, scale, 0, hp, pathData, 0f);
    }
    
    public void InitializeWithSplitCount(string pool, float scale, int splitCount, int hp = -1)
    {
        InitializeWithSplitCount(pool, scale, splitCount, hp, null, 0f);
    }
    
    public void InitializeWithSplitCount(string pool, float scale, int splitCount, int hp, PathData pathData)
    {
        InitializeWithSplitCount(pool, scale, splitCount, hp, pathData, 0f);
    }
    
    public void InitializeWithSplitCount(string pool, float scale, int splitCount, int hp, PathData pathData, float pathStartDistance)
    {
        InitializeWithSplitCount(pool, scale, splitCount, hp, pathData, pathStartDistance, true);
    }
    
    public void InitializeWithSplitCount(string pool, float scale, int splitCount, int hp, PathData pathData, float pathStartDistance, bool enableRotation)
    {
        poolName = pool;
        
        health.Initialize(obstacleData, hp);
        movement.Initialize(obstacleData, pathData, pathStartDistance, enableRotation);
        splitter.Initialize(scale, splitCount, pool, obstacleData);
        
        EventBus.Publish(new ObstacleSpawnedEvent(gameObject, transform.position));
    }
    
    public void InitializeAsChild(string pool, float scale, int hp, Vector3 position, float launchAngle, float launchForce, int splitCount)
    {
        poolName = pool;
        
        health.Initialize(obstacleData, hp);
        movement.InitializeAsChild(obstacleData, position, launchAngle, launchForce);
        splitter.Initialize(scale, splitCount, pool, obstacleData);
        
        EventBus.Publish(new ObstacleSpawnedEvent(gameObject, transform.position));
    }
    
    #endregion
    
    #region Damage & Destroy
    
    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
        
        if (health.IsDead())
        {
            DestroyObstacle();
        }
    }
    
    private void DestroyObstacle()
    {
        StopAllCoroutines();
        movement.ResetPhysics();
        
        Vector3 destroyPosition = transform.position;
        int score = health.GetScoreValue();
        
        EventBus.Publish(new ObstacleDestroyedEvent(gameObject, destroyPosition, score));
        
        coinDropper.TryDropCoins(destroyPosition);
        
        ObjectPool.Instance.ReturnObject(poolName, gameObject);
        
        if (splitter.CanSplit())
        {
            splitter.Split(destroyPosition);
        }
    }
    
    private void ReturnToPool()
    {
        StopAllCoroutines();
        movement.ResetPhysics();
        
        EventBus.Publish(new ObstacleDestroyedEvent(gameObject, transform.position, 0));
        ObjectPool.Instance.ReturnObject(poolName, gameObject);
    }
    
    #endregion
    
    #region Collision
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.GetDamage());
                bullet.OnHitObstacle();
            }
        }
    }
    
    #endregion
    
    #region Static Settings
    
    public static void SetSplitSettings(float scaleRatio, float angle, float force, int maxCount)
    {
        ObstacleSplitter.SetSplitSettings(scaleRatio, angle, force, maxCount);
    }
    
    #endregion
}
