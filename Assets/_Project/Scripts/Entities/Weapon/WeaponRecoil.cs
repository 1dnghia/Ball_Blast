using UnityEngine;

/// <summary>
/// Quản lý hiệu ứng giật nòng pháo
/// </summary>
public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private Transform cannonBarrel;
    [SerializeField] private float recoilAmount = 0.1f;
    [SerializeField] private float recoilSpeed = 10f;
    
    private Vector3 cannonOriginalPosition;
    private Vector3 recoilOffset;
    
    private void Awake()
    {
        if (cannonBarrel != null)
        {
            cannonOriginalPosition = cannonBarrel.localPosition;
        }
    }
    
    private void Update()
    {
        HandleRecoil();
    }
    
    public void OnShoot()
    {
        if (cannonBarrel == null) return;
        recoilOffset = Vector3.down * recoilAmount;
    }
    
    private void HandleRecoil()
    {
        if (cannonBarrel == null) return;
        
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
}
