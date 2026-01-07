using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Ball Blast/Obstacle Data", order = 1)]
public class ObstacleData : ScriptableObject
{
    [Header("Visual")]
    public Sprite sprite;
    public Color color = Color.white;
    public Vector2 size = Vector2.one;
    
    [Header("Movement")]
    [Tooltip("Tốc độ di chuyển ngang")]
    public float horizontalSpeed = 3f;
    
    [Tooltip("Tốc độ rơi xuống")]
    public float fallSpeed = 2f;
    
    [Tooltip("Thời gian delay trước khi bắt đầu rơi")]
    public float fallDelay = 1f;
    
    [Header("Stats")]
    [Tooltip("Máu tối thiểu của obstacle")]
    public int minHealth = 1;
    
    [Tooltip("Máu tối đa của obstacle")]
    public int maxHealth = 5;
    
    [Tooltip("Điểm nhận được khi phá hủy")]
    public int scoreValue = 10;
    
    [Header("Effects")]
    public GameObject destroyEffectPrefab;
    public GameObject hitEffectPrefab;
}
