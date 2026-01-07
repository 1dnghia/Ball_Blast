using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ObjectPool Manager - CHỈ gắn vào Empty GameObject cha tên "ObjectPoolManager"
/// Tự động quét tất cả các Empty con để tạo pool
/// Mỗi Empty con chứa 1 prefab và đặt tên theo loại (VD: BulletPool, EnemyPool)
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    
    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private bool canExpand = true;
    
    // Dictionary lưu pool theo tên empty con
    private Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();
    
    private class PoolData
    {
        public GameObject prefab;
        public Transform parent;
        public Queue<GameObject> pool = new Queue<GameObject>();
        public List<GameObject> activeObjects = new List<GameObject>();
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAllPools();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeAllPools()
    {
        // Duyệt qua tất cả các empty con
        foreach (Transform child in transform)
        {
            // Tìm prefab trong child (prefab được đặt làm con đầu tiên)
            if (child.childCount > 0)
            {
                Transform prefabTransform = child.GetChild(0);
                GameObject prefab = prefabTransform.gameObject;
                
                // Disable prefab gốc (chỉ dùng làm template)
                prefab.SetActive(false);
                
                // Tạo pool data
                PoolData poolData = new PoolData
                {
                    prefab = prefab,
                    parent = child
                };
                
                // Khởi tạo pool
                for (int i = 0; i < initialPoolSize; i++)
                {
                    CreateNewObject(poolData);
                }
                
                // Lưu vào dictionary với tên của empty con
                pools[child.name] = poolData;
                
                Debug.Log($"✓ Initialized pool: {child.name} with {initialPoolSize} objects");
            }
            else
            {
                Debug.LogWarning($"✗ Empty con '{child.name}' không có prefab!");
            }
        }
    }
    
    private GameObject CreateNewObject(PoolData poolData)
    {
        GameObject obj = Instantiate(poolData.prefab, poolData.parent);
        obj.SetActive(false);
        
        // Set pool reference cho các component cần thiết
        Bullet bullet = obj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetPool(this, poolData.parent.name);
        }
        
        poolData.pool.Enqueue(obj);
        return obj;
    }
    
    /// <summary>
    /// Lấy object từ pool theo tên
    /// </summary>
    public GameObject GetObject(string poolName)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.LogError($"Pool '{poolName}' không tồn tại!");
            return null;
        }
        
        PoolData poolData = pools[poolName];
        GameObject obj;
        
        if (poolData.pool.Count > 0)
        {
            obj = poolData.pool.Dequeue();
        }
        else if (canExpand)
        {
            obj = CreateNewObject(poolData);
        }
        else
        {
            Debug.LogWarning($"Pool '{poolName}' đã hết và không thể mở rộng!");
            return null;
        }
        
        obj.SetActive(true);
        poolData.activeObjects.Add(obj);
        return obj;
    }
    
    /// <summary>
    /// Lấy object từ pool với vị trí và rotation
    /// </summary>
    public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetObject(poolName);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;
    }
    
    /// <summary>
    /// Trả object về pool
    /// </summary>
    public void ReturnObject(string poolName, GameObject obj)
    {
        if (obj == null || !pools.ContainsKey(poolName)) return;
        
        PoolData poolData = pools[poolName];
        obj.SetActive(false);
        poolData.activeObjects.Remove(obj);
        poolData.pool.Enqueue(obj);
    }
    
    /// <summary>
    /// Trả tất cả active objects về pool
    /// </summary>
    public void ReturnAllObjects(string poolName)
    {
        if (!pools.ContainsKey(poolName)) return;
        
        PoolData poolData = pools[poolName];
        for (int i = poolData.activeObjects.Count - 1; i >= 0; i--)
        {
            ReturnObject(poolName, poolData.activeObjects[i]);
        }
    }
    
    // === Helper methods cho các pool thường dùng ===
    
    public GameObject GetBulletObject(Vector3 position, Quaternion rotation)
        => GetObject("BulletPool", position, rotation);
    
    public GameObject GetEnemyObject(Vector3 position, Quaternion rotation)
        => GetObject("EnemyPool", position, rotation);
    
    public GameObject GetParticleObject(Vector3 position)
        => GetObject("ParticlePool", position, Quaternion.identity);
}