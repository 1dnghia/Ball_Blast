using UnityEngine;
using TMPro;

/// <summary>
/// Quản lý health và damage của Obstacle
/// </summary>
public class ObstacleHealth : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshPro hpText;
    
    private int currentHealth;
    private ObstacleData obstacleData;
    
    public int CurrentHealth => currentHealth;
    
    public void Initialize(ObstacleData data, int health = -1)
    {
        obstacleData = data;
        
        if (health > 0)
        {
            currentHealth = health;
        }
        else if (data != null)
        {
            currentHealth = Random.Range(data.minHealth, data.maxHealth + 1);
        }
        
        UpdateDisplay();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        EventBus.Publish(new ObstacleDamagedEvent(GetComponent<Obstacle>(), damage, currentHealth));
        EventBus.Publish(new ObstacleHitEvent(gameObject, damage));
        
        UpdateDisplay();
    }
    
    public bool IsDead() => currentHealth <= 0;
    
    public int GetScoreValue() => obstacleData != null ? obstacleData.scoreValue : 10;
    
    private void UpdateDisplay()
    {
        if (hpText != null)
        {
            hpText.text = currentHealth.ToString();
        }
    }
}
