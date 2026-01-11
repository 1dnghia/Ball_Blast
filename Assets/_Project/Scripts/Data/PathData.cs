using UnityEngine;

/// <summary>
/// Đặt component này vào GameObject trong Scene và kéo các waypoint vào
/// </summary>
public class PathData : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Các GameObject trống làm waypoints (sắp xếp theo thứ tự đi)")]
    public Transform[] waypoints;
    
    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển")]
    [Range(0.5f, 10f)]
    public float speed = 2f;
    
    [Tooltip("Lặp lại đường đi")]
    public bool loop = true;
    
    [Header("Snake Head (Optional)")]
    [Tooltip("GameObject đầu rắn (sẽ di chuyển đầu tiên, obstacles theo sau)")]
    public GameObject snakeHead;
    
    [Tooltip("Tự động bắt đầu di chuyển head khi game start")]
    public bool autoStartHead = true;
    
    [Tooltip("Khoảng cách đầu rắn đi trước (units)")]
    [Range(1f, 5f)]
    public float headLeadDistance = 2f;
    
    [Header("Spawn Limits")]
    [Tooltip("Số lượng obstacles tối đa trên path (0 = không giới hạn)")]
    public int maxObstaclesOnPath = 10;
    
    /// <summary>
    /// Validate path
    /// </summary>
    public bool IsValid()
    {
        return waypoints != null && waypoints.Length >= 2;
    }
    
    private void Start()
    {
        // Tự động khởi tạo snake head nếu có
        if (autoStartHead && snakeHead != null)
        {
            SnakeHead headComponent = snakeHead.GetComponent<SnakeHead>();
            if (headComponent == null)
            {
                headComponent = snakeHead.AddComponent<SnakeHead>();
            }
            headComponent.Initialize(this);
        }
    }
    
    // Vẽ đường đi trong Scene view
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;
        
        Gizmos.color = Color.cyan;
        
        // Vẽ đường nối
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
        
        // Vẽ đường nối về điểm đầu nếu loop
        if (loop && waypoints[waypoints.Length - 1] != null && waypoints[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
        
        // Vẽ các điểm
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.color = i == 0 ? Color.green : (i == waypoints.Length - 1 ? Color.red : Color.yellow);
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            }
        }
    }
}
