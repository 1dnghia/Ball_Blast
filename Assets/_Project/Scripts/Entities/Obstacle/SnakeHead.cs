using UnityEngine;

/// <summary>
/// Component cho đầu rắn - di chuyển theo path
/// </summary>
public class SnakeHead : MonoBehaviour
{
    private PathData pathData;
    private float currentDistance = 0f;
    private bool isMoving = false;
    
    /// <summary>
    /// Khởi tạo và bắt đầu di chuyển
    /// </summary>
    public void Initialize(PathData path)
    {
        if (path == null || !path.IsValid())
        {
            Debug.LogWarning("PathData không hợp lệ!");
            return;
        }
        
        pathData = path;
        // Bắt đầu với khoảng cách lead để đi trước obstacles
        currentDistance = pathData.headLeadDistance;
        isMoving = true;
        
        // Đặt vị trí ban đầu
        UpdatePosition();
    }
    
    private void Update()
    {
        if (!isMoving || pathData == null) return;
        
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
                isMoving = false;
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
        
        // Tìm segment hiện tại
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
    
    /// <summary>
    /// Lấy khoảng cách hiện tại trên path
    /// </summary>
    public float GetCurrentDistance()
    {
        return currentDistance;
    }
    
    /// <summary>
    /// Dừng di chuyển
    /// </summary>
    public void Stop()
    {
        isMoving = false;
    }
    
    /// <summary>
    /// Tiếp tục di chuyển
    /// </summary>
    public void Resume()
    {
        isMoving = true;
    }
}
