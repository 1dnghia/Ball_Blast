using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    
    private void OnEnable()
    {
        // Subscribe to damage event để tính score
        EventBus.Subscribe<ObstacleDamagedEvent>(OnObstacleDamaged);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<ObstacleDamagedEvent>(OnObstacleDamaged);
    }
    
    /// <summary>
    /// Xử lý khi obstacle bị damage - Tăng score theo damage dealt
    /// </summary>
    private void OnObstacleDamaged(ObstacleDamagedEvent evt)
    {
        // Score tăng bằng số damage dealt
        int damageAmount = Mathf.RoundToInt(evt.DamageDealt);
        Debug.Log($"Obstacle damaged: {damageAmount} damage, adding to score");
        AddScore(damageAmount);
    }
    
    /// <summary>
    /// Thêm score
    /// </summary>
    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log($"Score updated: {currentScore} (+{amount})");
        
        // Publish event để UI update
        EventBus.Publish(new ScoreChangedEvent(currentScore, amount));
    }
    
    /// <summary>
    /// Reset score về 0
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        EventBus.Publish(new ScoreChangedEvent(currentScore, 0));
    }
    
    /// <summary>
    /// Get current score
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }
}
