using UnityEngine;

public class WeaponRoot : MonoBehaviour
{
    [Header("Weapon Components")]
    [SerializeField] private Transform firePoint; // Vị trí bắn đạn (đầu pháo)
    [SerializeField] private Transform cannonBarrel; // Nòng pháo (sẽ bị giật)
    [SerializeField] private Transform leftWheel; // Bánh xe trái
    [SerializeField] private Transform rightWheel; // Bánh xe phải
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float screenPadding = 0.5f;
    [SerializeField] private float wheelRotationSpeed = 360f; // Tốc độ xoay bánh xe (độ/giây)
    
    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float recoilAmount = 0.1f; // Độ giật khi bắn
    [SerializeField] private float recoilSpeed = 10f; // Tốc độ phục hồi sau giật
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip shootSound;
    
    // Private variables
    private AudioSource audioSource;
    private Camera mainCamera;
    private bool isShooting = false;
    private float nextFireTime = 0f;
    private float minX, maxX;
    private float weaponHalfWidth;
    private Vector3 cannonOriginalPosition; // Vị trí ban đầu của nòng pháo
    private Vector3 recoilOffset;
    private float currentMoveSpeed; // Tốc độ di chuyển hiện tại (để xoay bánh xe)
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        
        // Lưu vị trí ban đầu của nòng pháo
        if (cannonBarrel != null)
        {
            cannonOriginalPosition = cannonBarrel.localPosition;
        }
        
        CalculateScreenBounds();
        SetupFirePoint();
    }
    
    private void Update()
    {
        HandleMovement();
        HandleInput();
        HandleShooting();
        HandleRecoil();
    }
    
    // ==================== INITIALIZATION ====================
    
    private void SetupFirePoint()
    {
        // Nếu chưa có firePoint, tạo một transform con
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePoint = firePointObj.transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.up * 0.5f;
        }
    }
    
    private void CalculateScreenBounds()
    {
        // Lấy kích thước weapon
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
        
        // Tính toán giới hạn trái phải
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        
        minX = -camWidth + weaponHalfWidth + screenPadding;
        maxX = camWidth - weaponHalfWidth - screenPadding;
    }
    
    // ==================== INPUT HANDLING ====================
    
    private void HandleInput()
    {
        // Bắn tự động khi touch/click
        isShooting = Input.GetMouseButton(0) || Input.touchCount > 0;
    }
    
    // ==================== MOVEMENT ====================
    
    
    private void HandleMovement()
    {
        Vector3? targetPosition = null;
        float previousX = transform.position.x;
        
        // Input cho Mobile (touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            targetPosition = touch.position;
        }
        // Input cho PC (mouse)
        else if (Input.GetMouseButton(0))
        {
            targetPosition = Input.mousePosition;
        }
        
        // Di chuyển theo touch/mouse
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
            // Input keyboard (A/D hoặc Left/Right)
            float moveInput = Input.GetAxisRaw("Horizontal");
            
            if (moveInput != 0)
            {
                Vector3 pos = transform.position;
                pos.x += moveInput * moveSpeed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                
                transform.position = pos;
            }
        }
        
        // Tính tốc độ di chuyển thực tế
        currentMoveSpeed = (transform.position.x - previousX) / Time.deltaTime;
        
        // Xoay bánh xe
        RotateWheels();
    }
    
    private void RotateWheels()
    {
        if (Mathf.Abs(currentMoveSpeed) < 0.01f) return;
        
        // Tính góc xoay dựa trên tốc độ di chuyển
        float rotationAmount = currentMoveSpeed * wheelRotationSpeed * Time.deltaTime;
        
        // Xoay bánh xe (ngược chiều di chuyển)
        if (leftWheel != null)
        {
            leftWheel.Rotate(0, 0, -rotationAmount);
        }
        
        if (rightWheel != null)
        {
            rightWheel.Rotate(0, 0, -rotationAmount);
        }
    }
    
    // ==================== SHOOTING ====================
    
    private void HandleShooting()
    {
        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    
    private void Shoot()
    {
        if (ObjectPool.Instance == null)
        {
            Debug.LogWarning("ObjectPool chưa được khởi tạo!");
            return;
        }
        
        // Lấy đạn từ pool
        GameObject bullet = ObjectPool.Instance.GetBulletObject(firePoint.position, firePoint.rotation);
        
        if (bullet != null)
        {
            // Tạo hiệu ứng giật
            ApplyRecoil();
            
            // Phát âm thanh bắn
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }
    
    // ==================== RECOIL (GIẬT NÒNG PHÁO) ====================
    
    private void ApplyRecoil()
    {
        if (cannonBarrel == null) return;
        
        // Giật nòng pháo xuống/lùi một chút
        recoilOffset = Vector3.down * recoilAmount;
    }
    
    private void HandleRecoil()
    {
        if (cannonBarrel == null) return;
        
        // Phục hồi từ từ về vị trí ban đầu
        if (recoilOffset.magnitude > 0.001f)
        {
            recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, recoilSpeed * Time.deltaTime);
            cannonBarrel.localPosition = cannonOriginalPosition + recoilOffset;
        }
        else
        {
            cannonBarrel.localPosition = cannonOriginalPosition;
        }
    }
    
    // ==================== LIFECYCLE ====================
    private void OnDisable()
    {
        isShooting = false;
    }
}
