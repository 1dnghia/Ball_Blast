using UnityEngine;

/// <summary>
/// Quản lý bắn đạn
/// </summary>
public class WeaponShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.15f;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip shootSound;
    
    private AudioSource audioSource;
    private bool isShooting = false;
    private float nextFireTime = 0f;
    
    public bool IsShooting
    {
        get => isShooting;
        set => isShooting = value;
    }
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SetupFirePoint();
    }
    
    private void Update()
    {
        HandleInput();
        HandleShooting();
    }
    
    private void HandleInput()
    {
        isShooting = Input.GetMouseButton(0) || Input.touchCount > 0;
    }
    
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
        if (ObjectPool.Instance == null) return;
        
        GameObject bullet = ObjectPool.Instance.GetBulletObject(firePoint.position, firePoint.rotation);
        
        if (bullet != null)
        {
            // Publish bullet fired event
            EventBus.Publish(new BulletFiredEvent(firePoint.position, bullet));
            
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
            
            // Notify recoil
            SendMessage("OnShoot", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    private void SetupFirePoint()
    {
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePoint = firePointObj.transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.up * 0.5f;
        }
    }
    
    private void OnDisable()
    {
        isShooting = false;
    }
}
