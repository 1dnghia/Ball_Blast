using UnityEngine;

/// <summary>
/// Định nghĩa tất cả các events trong game
/// </summary>
/// 
// Obstacle Events
public struct ObstacleSpawnedEvent : IGameEvent
{
    public GameObject Obstacle;
    public Vector3 Position;
    
    public ObstacleSpawnedEvent(GameObject obstacle, Vector3 position)
    {
        Obstacle = obstacle;
        Position = position;
    }
}

public struct ObstacleDestroyedEvent : IGameEvent
{
    public GameObject Obstacle;
    public Vector3 Position;
    public int ScoreValue;
    
    public ObstacleDestroyedEvent(GameObject obstacle, Vector3 position, int scoreValue = 10)
    {
        Obstacle = obstacle;
        Position = position;
        ScoreValue = scoreValue;
    }
}

public struct ObstacleHitEvent : IGameEvent
{
    public GameObject Obstacle;
    public int Damage;
    
    public ObstacleHitEvent(GameObject obstacle, int damage)
    {
        Obstacle = obstacle;
        Damage = damage;
    }
}

// Game Events
public struct GameStartedEvent : IGameEvent
{
    public GameStartedEvent(bool dummy = false) { }
}

public struct GameOverEvent : IGameEvent
{
    public int FinalScore;
    
    public GameOverEvent(int finalScore)
    {
        FinalScore = finalScore;
    }
}

public struct GamePausedEvent : IGameEvent
{
    public bool IsPaused;
    
    public GamePausedEvent(bool isPaused)
    {
        IsPaused = isPaused;
    }
}

// Score Events
public struct ScoreChangedEvent : IGameEvent
{
    public int NewScore;
    public int ScoreAdded;
    
    public ScoreChangedEvent(int newScore, int scoreAdded)
    {
        NewScore = newScore;
        ScoreAdded = scoreAdded;
    }
}

public struct CoinCollectedEvent : IGameEvent
{
    public int Amount;
    
    public CoinCollectedEvent(int amount)
    {
        Amount = amount;
    }
}

public struct ObstacleDamagedEvent : IGameEvent
{
    public Obstacle Obstacle;
    public float DamageDealt;
    public float RemainingHealth;
    
    public ObstacleDamagedEvent(Obstacle obstacle, float damageDealt, float remainingHealth)
    {
        Obstacle = obstacle;
        DamageDealt = damageDealt;
        RemainingHealth = remainingHealth;
    }
}

// Weapon Events
public struct BulletFiredEvent : IGameEvent
{
    public Vector3 Position;
    public GameObject Bullet;
    
    public BulletFiredEvent(Vector3 position, GameObject bullet)
    {
        Position = position;
        Bullet = bullet;
    }
}

// Player Events
public struct PlayerHitEvent : IGameEvent
{
    public int CurrentHealth;
    public int Damage;
    
    public PlayerHitEvent(int currentHealth, int damage)
    {
        CurrentHealth = currentHealth;
        Damage = damage;
    }
}
