using UnityEngine;

/// <summary>
/// Quản lý spawn coin khi obstacle destroyed
/// </summary>
public class ObstacleCoinDropper : MonoBehaviour
{
    [Header("Coin Drop Settings")]
    [SerializeField] private bool canDropCoin = true;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] [Range(0f, 1f)] private float coinDropChance = 0.3f;
    [SerializeField] private int minCoins = 1;
    [SerializeField] private int maxCoins = 3;
    [SerializeField] private string coinPoolName = "Coin";
    
    public void TryDropCoins(Vector3 position)
    {
        if (!canDropCoin || Random.value > coinDropChance) return;
        if (coinPrefab == null || ObjectPool.Instance == null) return;
        
        int coinCount = Random.Range(minCoins, maxCoins + 1);
        
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0f
            );
            
            Vector3 spawnPos = position + offset;
            GameObject coin = ObjectPool.Instance.GetObject(coinPoolName, spawnPos, Quaternion.identity);
            
            if (coin == null)
            {
                coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
            
            if (coin != null)
            {
                Coin coinScript = coin.GetComponent<Coin>();
                if (coinScript != null)
                {
                    coinScript.SetPoolName(coinPoolName);
                    coinScript.ApplyRandomForce(3f, 6f);
                }
            }
        }
    }
}
