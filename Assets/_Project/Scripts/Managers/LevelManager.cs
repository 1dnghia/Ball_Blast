using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private LevelData[] allLevels; // Danh sách tất cả levels
    
    [Header("References")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioSource musicSource;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    
    // Game state
    private int currentScore = 0;
    private float currentTime = 0f;
    private int obstaclesDestroyed = 0;
    private bool isLevelActive = false;
    
    private void OnEnable()
    {
        // Subscribe to events
        EventBus.Subscribe<ObstacleDestroyedEvent>(OnObstacleDestroyed);
        EventBus.Subscribe<PlayerDeathEvent>(OnPlayerDeath);
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        EventBus.Unsubscribe<ObstacleDestroyedEvent>(OnObstacleDestroyed);
        EventBus.Unsubscribe<PlayerDeathEvent>(OnPlayerDeath);
    }
    
    void Start()
    {
        if (currentLevel != null)
        {
            LoadLevel(currentLevel);
        }
    }
    
    void Update()
    {
        if (!isLevelActive) return;
        
        // Update timer
        currentTime += Time.deltaTime;
        UpdateUI();
        
        // Kiểm tra time limit
        if (currentLevel != null && currentLevel.IsTimeUp(currentTime))
        {
            OnTimeUp();
        }
    }
    
    /// <summary>
    /// Load một level mới
    /// </summary>
    public void LoadLevel(LevelData level)
    {
        if (level == null)
        {
            Debug.LogError("Level data is null!");
            return;
        }
        
        currentLevel = level;
        
        // Reset game state
        currentScore = 0;
        currentTime = 0f;
        obstaclesDestroyed = 0;
        isLevelActive = true;
        
        // Hide panels
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        
        // Apply level settings
        ApplyLevelSettings();
        
        // Start spawning
        if (obstacleSpawner != null && level != null)
        {
            obstacleSpawner.SetLevelData(level);
            obstacleSpawner.StartSpawning();
        }
        
        Debug.Log($"Loaded {level.levelName}");
    }
    
    /// <summary>
    /// Load level theo số thứ tự
    /// </summary>
    public void LoadLevelByNumber(int levelNumber)
    {
        if (allLevels == null || allLevels.Length == 0)
        {
            Debug.LogError("No levels configured!");
            return;
        }
        
        foreach (var level in allLevels)
        {
            if (level != null && level.levelNumber == levelNumber)
            {
                LoadLevel(level);
                return;
            }
        }
        
        Debug.LogError($"Level {levelNumber} not found!");
    }
    
    /// <summary>
    /// Load level tiếp theo
    /// </summary>
    public void LoadNextLevel()
    {
        if (currentLevel != null && currentLevel.nextLevel != null)
        {
            LoadLevel(currentLevel.nextLevel);
        }
        else
        {
            Debug.Log("No next level!");
        }
    }
    
    /// <summary>
    /// Restart level hiện tại
    /// </summary>
    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }
    
    private void ApplyLevelSettings()
    {
        if (currentLevel == null) return;
        
        // Set split settings cho tất cả obstacles
        Obstacle.SetSplitSettings(
            currentLevel.splitScaleRatio,
            currentLevel.splitAngle,
            currentLevel.splitForce,
            currentLevel.maxSplitCount
        );
        
        // Apply background (nếu có)
        // Có thể thêm background sprite vào LevelData sau
        
        // Apply music (nếu có)
        // Có thể thêm music clip vào LevelData sau
        
        // Update UI
        if (levelNameText != null)
        {
            levelNameText.text = currentLevel.levelName;
        }
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
            
            // Hiển thị target nếu có
            if (currentLevel != null && currentLevel.targetScore > 0)
            {
                scoreText.text = $"Score: {currentScore}/{currentLevel.targetScore}";
            }
        }
        
        if (timeText != null)
        {
            if (currentLevel != null && currentLevel.timeLimit > 0)
            {
                float remainingTime = currentLevel.timeLimit - currentTime;
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
            else
            {
                int minutes = Mathf.FloorToInt(currentTime / 60f);
                int seconds = Mathf.FloorToInt(currentTime % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }
    }
    
    private void OnObstacleDestroyed(ObstacleDestroyedEvent evt)
    {
        if (!isLevelActive) return;
        
        // Cộng điểm
        currentScore += evt.ScoreValue;
        obstaclesDestroyed++;
        
        // Kiểm tra điều kiện thắng
        if (currentLevel != null && currentLevel.CheckVictoryCondition(currentScore, obstaclesDestroyed))
        {
            OnLevelComplete();
        }
    }
    
    private void OnPlayerDeath(PlayerDeathEvent evt)
    {
        OnLevelFailed();
    }
    
    private void OnTimeUp()
    {
        // Kiểm tra có đạt điều kiện thắng không
        if (currentLevel != null && currentLevel.CheckVictoryCondition(currentScore, obstaclesDestroyed))
        {
            OnLevelComplete();
        }
        else
        {
            OnLevelFailed();
        }
    }
    
    private void OnLevelComplete()
    {
        isLevelActive = false;
        
        // Stop spawning
        if (obstacleSpawner != null)
        {
            obstacleSpawner.StopSpawning();
        }
        
        // Show victory panel
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
        
        Debug.Log($"Level Complete! Score: {currentScore}");
        
        // Publish victory event
        EventBus.Publish(new LevelCompleteEvent(currentLevel.levelNumber, currentScore));
    }
    
    private void OnLevelFailed()
    {
        isLevelActive = false;
        
        // Stop spawning
        if (obstacleSpawner != null)
        {
            obstacleSpawner.StopSpawning();
        }
        
        // Show defeat panel
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }
        
        Debug.Log("Level Failed!");
        
        // Publish defeat event
        EventBus.Publish(new LevelFailedEvent(currentLevel.levelNumber, currentScore));
    }
}

// Events
public class LevelCompleteEvent : IGameEvent
{
    public int LevelNumber { get; }
    public int Score { get; }
    
    public LevelCompleteEvent(int levelNumber, int score)
    {
        LevelNumber = levelNumber;
        Score = score;
    }
}

public class LevelFailedEvent : IGameEvent
{
    public int LevelNumber { get; }
    public int Score { get; }
    
    public LevelFailedEvent(int levelNumber, int score)
    {
        LevelNumber = levelNumber;
        Score = score;
    }
}

public class PlayerDeathEvent : IGameEvent
{
    // Có thể thêm thông tin về cách chết
}
