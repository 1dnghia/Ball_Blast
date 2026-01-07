using UnityEngine;

/// <summary>
/// Quản lý di chuyển của weapon
/// </summary>
public class WeaponMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float screenPadding = 0.5f;
    [SerializeField] private float wheelRotationSpeed = 360f;
    
    [Header("Wheel References")]
    [SerializeField] private Transform leftWheel;
    [SerializeField] private Transform rightWheel;
    
    private Camera mainCamera;
    private float minX, maxX;
    private float weaponHalfWidth;
    private float currentMoveSpeed;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        CalculateScreenBounds();
    }
    
    private void Update()
    {
        HandleMovement();
    }
    
    private void CalculateScreenBounds()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            weaponHalfWidth = spriteRenderer.bounds.extents.x;
        }
        else
        {
            Collider2D collider = GetComponent<Collider2D>();
            weaponHalfWidth = collider != null ? collider.bounds.extents.x : 0.5f;
        }
        
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        
        minX = -camWidth + weaponHalfWidth + screenPadding;
        maxX = camWidth - weaponHalfWidth - screenPadding;
    }
    
    private void HandleMovement()
    {
        Vector3? targetPosition = null;
        float previousX = transform.position.x;
        
        if (Input.touchCount > 0)
        {
            targetPosition = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0))
        {
            targetPosition = Input.mousePosition;
        }
        
        if (targetPosition.HasValue)
        {
            Vector3 screenPos = targetPosition.Value;
            screenPos.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, worldPos.x, moveSpeed * Time.deltaTime);
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            
            transform.position = pos;
        }
        else
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            
            if (moveInput != 0)
            {
                Vector3 pos = transform.position;
                pos.x += moveInput * moveSpeed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                
                transform.position = pos;
            }
        }
        
        currentMoveSpeed = (transform.position.x - previousX) / Time.deltaTime;
        RotateWheels();
    }
    
    private void RotateWheels()
    {
        if (Mathf.Abs(currentMoveSpeed) < 0.01f) return;
        
        float rotationAmount = currentMoveSpeed * wheelRotationSpeed * Time.deltaTime;
        
        if (leftWheel != null)
        {
            leftWheel.Rotate(0, 0, -rotationAmount);
        }
        
        if (rightWheel != null)
        {
            rightWheel.Rotate(0, 0, -rotationAmount);
        }
    }
}
