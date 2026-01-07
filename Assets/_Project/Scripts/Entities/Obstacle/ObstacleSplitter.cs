using UnityEngine;

/// <summary>
/// Quản lý logic split obstacle
/// </summary>
public class ObstacleSplitter : MonoBehaviour
{
    private static float splitScaleRatio = 0.6f;
    private static float splitAngle = 45f;
    private static float splitForce = 3f;
    private static int maxSplitCount = 2;
    
    private int currentSplitCount = 0;
    private float currentScale = 1f;
    private string poolName;
    private ObstacleData obstacleData;
    
    public static void SetSplitSettings(float scaleRatio, float angle, float force, int maxCount)
    {
        splitScaleRatio = scaleRatio;
        splitAngle = angle;
        splitForce = force;
        maxSplitCount = maxCount;
    }
    
    public void Initialize(float scale, int splitCount, string pool, ObstacleData data)
    {
        currentScale = scale;
        currentSplitCount = splitCount;
        poolName = pool;
        obstacleData = data;
        
        transform.localScale = Vector3.one * scale;
    }
    
    public bool CanSplit() => currentSplitCount < maxSplitCount;
    
    public void Split(Vector3 position)
    {
        if (!CanSplit() || ObjectPool.Instance == null || obstacleData == null) return;
        
        float newScale = currentScale * splitScaleRatio;
        int nextSplitCount = currentSplitCount + 1;
        
        for (int i = 0; i < 2; i++)
        {
            GameObject child = ObjectPool.Instance.GetObject(poolName, position, Quaternion.identity);
            if (child != null)
            {
                Obstacle obstacle = child.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    int childHealth = Random.Range(obstacleData.minHealth, obstacleData.maxHealth + 1);
                    float angle = i == 0 ? -splitAngle : splitAngle;
                    obstacle.InitializeAsChild(poolName, newScale, childHealth, position, angle, splitForce, nextSplitCount);
                }
            }
        }
    }
}
