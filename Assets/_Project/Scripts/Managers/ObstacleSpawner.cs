using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject obstaclePool;
    [SerializeField] private LevelData levelData;
    [SerializeField] private bool autoStart = true;
    
    private bool isSpawning = false;
    private float currentMinInterval;
    private float currentMaxInterval;
    private float progressionTimer = 0f;
    
    void Start()
    {
        // Khởi tạo interval từ level data
        if (levelData != null)
        {
            currentMinInterval = levelData.minSpawnInterval;
            currentMaxInterval = levelData.maxSpawnInterval;
        }
        
        if (autoStart)
        {
            StartSpawning();
        }
    }
    
    void Update()
    {
        // Tăng độ khó theo thời gian nếu enabled
        if (isSpawning && levelData != null && levelData.enableProgression)
        {
            progressionTimer += Time.deltaTime;
            
            if (progressionTimer >= levelData.progressionInterval)
            {
                progressionTimer = 0f;
                IncreaseDifficulty();
            }
        }
    }
    
    private void IncreaseDifficulty()
    {
        if (levelData == null) return;
        
        // Giảm spawn interval để spawn nhanh hơn
        currentMinInterval = Mathf.Max(
            levelData.minimumInterval, 
            currentMinInterval - levelData.intervalDecreaseAmount
        );
        currentMaxInterval = Mathf.Max(
            levelData.minimumInterval, 
            currentMaxInterval - levelData.intervalDecreaseAmount
        );
        
        Debug.Log($"Difficulty increased! New interval: {currentMinInterval:F2} - {currentMaxInterval:F2}");
    }
    
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
        progressionTimer = 0f;
        StopAllCoroutines();
    }
    
    /// <summary>
    /// Thay đổi level data (dùng khi chuyển level)
    /// </summary>
    public void SetLevelData(LevelData newLevelData)
    {
        levelData = newLevelData;
        if (levelData != null)
        {
            currentMinInterval = levelData.minSpawnInterval;
            currentMaxInterval = levelData.maxSpawnInterval;
            progressionTimer = 0f;
        }
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnObstacle();
            
            // Random spawn interval từ config hiện tại
            float waitTime = Random.Range(currentMinInterval, currentMaxInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    private void SpawnObstacle()
    {
        if (obstaclePool == null)
        {
            Debug.LogError("ObstaclePool chưa được gán!");
            return;
        }
        
        if (levelData == null)
        {
            Debug.LogError("LevelData chưa được gán!");
            return;
        }
        
        // Chọn loại obstacle dựa trên weight từ level data
        ObstacleSpawnInfo spawnInfo = levelData.GetRandomObstacleSpawnInfo();
        if (spawnInfo == null)
        {
            Debug.LogError("Không thể chọn ObstacleSpawnInfo! Kiểm tra obstacleSpawnInfos trong LevelData.");
            return;
        }
        
        Debug.Log($"Spawning obstacle: scale={spawnInfo.scale}, splitCount={spawnInfo.splitCount}");
        
        string poolName = obstaclePool.name;
        GameObject obstacle = ObjectPool.Instance.GetObject(poolName);
        if (obstacle != null)
        {
            Obstacle obstacleScript = obstacle.GetComponent<Obstacle>();
            if (obstacleScript != null)
            {
                // Initialize với scale và splitCount từ spawn info
                obstacleScript.InitializeWithSplitCount(poolName, spawnInfo.scale, spawnInfo.splitCount);
                Debug.Log($"Obstacle initialized with scale: {spawnInfo.scale}, localScale: {obstacle.transform.localScale}");
            }
            else
            {
                Debug.LogError("Obstacle prefab không có Obstacle component!");
            }
        }
        else
        {
            Debug.LogError($"ObjectPool không thể lấy object: {poolName}");
        }
    }
}
