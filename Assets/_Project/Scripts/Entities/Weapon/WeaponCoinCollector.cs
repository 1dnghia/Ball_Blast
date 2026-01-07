using UnityEngine;

/// <summary>
/// Quản lý hút coin về weapon
/// </summary>
public class WeaponCoinCollector : MonoBehaviour
{
    [Header("Coin Collection")]
    [SerializeField] private Transform coinCollectPoint;
    [SerializeField] private float coinCollectRadius = 3f;
    [SerializeField] private LayerMask coinLayer;
    
    private void Update()
    {
        CollectNearbyCoins();
    }
    
    private void CollectNearbyCoins()
    {
        Vector3 collectPosition = coinCollectPoint != null ? coinCollectPoint.position : transform.position;
        
        Collider2D[] coins = Physics2D.OverlapCircleAll(collectPosition, coinCollectRadius, coinLayer);
        
        foreach (Collider2D coinCollider in coins)
        {
            Coin coin = coinCollider.GetComponent<Coin>();
            if (coin != null)
            {
                Transform targetTransform = coinCollectPoint != null ? coinCollectPoint : transform;
                coin.StartMovingToPlayer(targetTransform);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 collectPosition = coinCollectPoint != null ? coinCollectPoint.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(collectPosition, coinCollectRadius);
    }
}
