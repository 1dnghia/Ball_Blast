using UnityEngine;

/// <summary>
/// Loại di chuyển của obstacle
/// </summary>
public enum MovementType
{
    Normal,  // Di chuyển bình thường (rơi từ trên xuống)
    Path     // Di chuyển theo path được vẽ
}

[System.Serializable]
public class ObstacleSpawnInfo
{
    [Tooltip("Tên loại (để dễ nhận biết)")]
    public string typeName = "Large";
    
    [Tooltip("Kích thước (1 = bình thường, 1.5 = lớn)")]
    [Range(0.5f, 2f)]
    public float scale = 1f;
    
    [Tooltip("Số lần đã tách (0 = tách 2 lần, 1 = tách 1 lần, 2 = không tách)")]
    [Range(0, 2)]
    public int splitCount = 0;
    
    [Tooltip("Tỉ lệ xuất hiện (càng cao càng nhiều)")]
    [Range(1, 100)]
    public int spawnWeight = 10;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Ball Blast/Level Data", order = 3)]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";
    
    [Header("Movement Type")]
    [Tooltip("Loại di chuyển: Normal = rơi tự do, Path = theo đường vẽ")]
    public MovementType movementType = MovementType.Normal;
    
    [Tooltip("Cho phép obstacles tự xoay (chỉ áp dụng cho Normal movement)")]
    public bool enableRotation = true;
    
    [Header("Spawn Settings")]
    [Tooltip("Thời gian spawn obstacle tối thiểu")]
    public float minSpawnInterval = 1f;
    
    [Tooltip("Thời gian spawn obstacle tối đa")]
    public float maxSpawnInterval = 3f;
    
    [Tooltip("Số lượng obstacle tối đa cùng lúc")]
    public int maxObstaclesOnScreen = 5;
    
    [Header("Obstacle Types")]
    [Tooltip("Danh sách các loại obstacle với thông tin spawn")]
    public ObstacleSpawnInfo[] obstacleSpawnInfos;
    
    [Header("Split Settings (Áp dụng cho tất cả obstacle trong level)")]
    [Tooltip("Tỷ lệ scale của obstacle con so với cha")]
    [Range(0.3f, 0.9f)]
    public float splitScaleRatio = 0.6f;
    
    [Tooltip("Góc bắn ra của obstacle con (độ)")]
    [Range(30f, 90f)]
    public float splitAngle = 45f;
    
    [Tooltip("Lực bắn ra của obstacle con")]
    public float splitForce = 3f;
    
    [Tooltip("Số lần tách tối đa (maxSplitCount = 2 nghĩa là tách tối đa 2 lần)")]
    [Range(0, 3)]
    public int maxSplitCount = 2;
    
    [Header("Progression")]
    [Tooltip("Tự động tăng độ khó theo thời gian")]
    public bool enableProgression = false;
    
    [Tooltip("Giảm spawn interval sau mỗi X giây")]
    public float progressionInterval = 30f;
    
    [Tooltip("Giảm bao nhiêu giây mỗi lần")]
    public float intervalDecreaseAmount = 0.1f;
    
    [Tooltip("Spawn interval tối thiểu")]
    public float minimumInterval = 0.3f;
    
    [Header("Difficulty")]
    [Tooltip("Tốc độ tăng độ khó theo thời gian (multiplier/second)")]
    public float difficultyIncreaseRate = 0.01f;
    
    [Tooltip("Tốc độ tối đa có thể đạt được")]
    public float maxSpeedMultiplier = 3f;
    
    [Header("Win/Loss Conditions")]
    [Tooltip("Điểm cần đạt để hoàn thành level (0 = không giới hạn)")]
    public int targetScore = 100;
    
    [Tooltip("Thời gian tối đa để hoàn thành level (0 = vô hạn)")]
    public float timeLimit = 0f;
    
    [Tooltip("Số obstacle cần phá (0 = không giới hạn)")]
    public int targetObstacleCount = 0;
    
    [Header("Next Level")]
    [Tooltip("Level tiếp theo")]
    public LevelData nextLevel;
    
    /// <summary>
    /// Lấy ObstacleSpawnInfo ngẫu nhiên dựa trên spawn weights
    /// </summary>
    public ObstacleSpawnInfo GetRandomObstacleSpawnInfo()
    {
        if (obstacleSpawnInfos == null || obstacleSpawnInfos.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawnInfos is empty! Creating default.");
            return CreateDefaultSpawnInfo();
        }
        
        // Tính tổng weights
        int totalWeight = 0;
        foreach (var info in obstacleSpawnInfos)
        {
            totalWeight += info.spawnWeight;
        }
        
        if (totalWeight <= 0)
        {
            return ValidateSpawnInfo(obstacleSpawnInfos[0]);
        }
        
        // Random theo weights
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (var info in obstacleSpawnInfos)
        {
            currentWeight += info.spawnWeight;
            if (randomValue < currentWeight)
            {
                return ValidateSpawnInfo(info);
            }
        }
        
        return ValidateSpawnInfo(obstacleSpawnInfos[0]);
    }
    
    /// <summary>
    /// Validate và fix giá trị spawn info
    /// </summary>
    private ObstacleSpawnInfo ValidateSpawnInfo(ObstacleSpawnInfo info)
    {
        if (info.scale <= 0f)
        {
            Debug.LogWarning($"ObstacleSpawnInfo scale is {info.scale}, setting to default 1.0");
            info.scale = 1f;
        }
        return info;
    }
    
    /// <summary>
    /// Tạo spawn info mặc định
    /// </summary>
    private ObstacleSpawnInfo CreateDefaultSpawnInfo()
    {
        return new ObstacleSpawnInfo
        {
            typeName = "Default",
            scale = 1f,
            splitCount = 1,
            spawnWeight = 100
        };
    }
    
    /// <summary>
    /// Kiểm tra điều kiện thắng
    /// </summary>
    public bool CheckVictoryCondition(int currentScore, int obstaclesDestroyed)
    {
        if (targetScore > 0 && currentScore < targetScore)
            return false;
        
        if (targetObstacleCount > 0 && obstaclesDestroyed < targetObstacleCount)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Kiểm tra hết thời gian
    /// </summary>
    public bool IsTimeUp(float currentTime)
    {
        return timeLimit > 0 && currentTime >= timeLimit;
    }
}
