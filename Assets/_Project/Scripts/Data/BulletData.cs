using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Ball Blast/Bullet Data", order = 2)]
public class BulletData : ScriptableObject
{
    [Header("Visual")]
    public Sprite sprite;
    public Color color = Color.white;
    public Vector2 size = new Vector2(0.2f, 0.2f);
    
    [Header("Movement")]
    [Tooltip("Tốc độ bay của đạn")]
    public float speed = 10f;
    
    [Tooltip("Thời gian sống tối đa (giây)")]
    public float lifetime = 5f;
    
    [Header("Damage")]
    [Tooltip("Sát thương gây ra cho obstacle")]
    public int damage = 1;
    
    [Header("Effects")]
    public GameObject hitEffectPrefab;
    public GameObject trailEffectPrefab;
}
