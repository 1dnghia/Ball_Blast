using UnityEngine;

/// <summary>
/// Main coordinator cho Weapon - gọn gàng, dễ hiểu
/// </summary>
[RequireComponent(typeof(WeaponMovement))]
[RequireComponent(typeof(WeaponShooter))]
[RequireComponent(typeof(WeaponRecoil))]
[RequireComponent(typeof(WeaponCoinCollector))]
public class WeaponRoot : MonoBehaviour
{
    // Tất cả logic đã được tách ra các component riêng
    // Class này chỉ làm coordinator nếu cần
}
