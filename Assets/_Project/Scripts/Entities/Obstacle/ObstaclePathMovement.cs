using UnityEngine;

/// <summary>
/// Di chuyển obstacle theo waypoints đơn giản
/// </summary>
public class ObstaclePathMovement : MonoBehaviour
{
    private PathData pathData;
    private float currentDistance = 0f; // Khoảng cách đã đi trên path
    private bool isActive = false;
    
    public void Initialize(PathData path, float startDistance = 0f)
    {
        if (path == null || !path.IsValid())
        {
            Debug.LogWarning("PathData không hợp lệ!");
            isActive = false;
            return;
        }
        
        pathData = path;
        currentDistance = startDistance;
        isActive = true;
        
        // Đặt vị trí ban đầu
        UpdatePosition();
    }
    
    private void Update()
    {
        if (!isActive || pathData == null) return;
        
        MoveAlongPath();
    }
    
    private void MoveAlongPath()
    {
        // Di chuyển theo path
        currentDistance += pathData.speed * Time.deltaTime;
        
        // Tính tổng độ dài path
        float totalLength = GetTotalPathLength();
        
        // Kiểm tra loop
        if (currentDistance >= totalLength)
        {
            if (pathData.loop)
            {
                currentDistance = currentDistance % totalLength;
            }
            else
            {
                currentDistance = totalLength;
                isActive = false;
            }
        }
        
        UpdatePosition();
    }
    
    private void UpdatePosition()
    {
        if (pathData == null || pathData.waypoints == null || pathData.waypoints.Length < 2)
            return;
        
        float totalLength = GetTotalPathLength();
        if (totalLength <= 0) return;
        
        float targetDistance = currentDistance;
        float accumulatedDistance = 0f;
        
        // Tìm segment hiện tại và vị trí trên segment đó
        for (int i = 0; i < pathData.waypoints.Length - 1; i++)
        {
            if (pathData.waypoints[i] == null || pathData.waypoints[i + 1] == null)
                continue;
            
            float segmentLength = Vector3.Distance(
                pathData.waypoints[i].position, 
                pathData.waypoints[i + 1].position
            );
            
            if (accumulatedDistance + segmentLength >= targetDistance)
            {
                // Vị trí nằm trong segment này
                float segmentProgress = (targetDistance - accumulatedDistance) / segmentLength;
                transform.position = Vector3.Lerp(
                    pathData.waypoints[i].position,
                    pathData.waypoints[i + 1].position,
                    segmentProgress
                );
                return;
            }
            
            accumulatedDistance += segmentLength;
        }
        
        // Nếu vượt quá, đặt ở waypoint cuối
        transform.position = pathData.waypoints[pathData.waypoints.Length - 1].position;
    }
    
    private float GetTotalPathLength()
    {
        if (pathData == null || pathData.waypoints == null || pathData.waypoints.Length < 2)
            return 0f;
        
        float totalLength = 0f;
        for (int i = 0; i < pathData.waypoints.Length - 1; i++)
        {
            if (pathData.waypoints[i] != null && pathData.waypoints[i + 1] != null)
            {
                totalLength += Vector3.Distance(
                    pathData.waypoints[i].position,
                    pathData.waypoints[i + 1].position
                );
            }
        }
        
        return totalLength;
    }
    
    public void Stop()
    {
        isActive = false;
    }
    
    public bool IsActive()
    {
        return isActive;
    }
    
    public float GetCurrentDistance()
    {
        return currentDistance;
    }
}
