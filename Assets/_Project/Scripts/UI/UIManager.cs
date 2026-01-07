using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Score & Coin UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;
    
    [Header("Animation")]
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float punchDuration = 0.1f;
    
    private int currentScore = 0;
    private int currentCoins = 0;
    
    private void OnEnable()
    {
        // Subscribe to events
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Subscribe<CoinCollectedEvent>(OnCoinCollected);
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Unsubscribe<CoinCollectedEvent>(OnCoinCollected);
    }
    
    private void Start()
    {
        UpdateScoreUI();
        UpdateCoinUI();
    }
    
    /// <summary>
    /// Xử lý khi score thay đổi
    /// </summary>
    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        currentScore = evt.NewScore;
        UpdateScoreUI();
        
        // Animate score text
        if (scoreText != null)
        {
            StopAllCoroutines();
            StartCoroutine(PunchAnimation(scoreText.transform));
        }
    }
    
    /// <summary>
    /// Xử lý khi nhặt coin
    /// </summary>
    private void OnCoinCollected(CoinCollectedEvent evt)
    {
        currentCoins += evt.Amount;
        UpdateCoinUI();
        
        // Animate coin text
        if (coinText != null)
        {
            StopAllCoroutines();
            StartCoroutine(PunchAnimation(coinText.transform));
        }
    }
    
    /// <summary>
    /// Cập nhật score UI
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }
    
    /// <summary>
    /// Cập nhật coin UI
    /// </summary>
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
    
    /// <summary>
    /// Animation punch scale
    /// </summary>
    private System.Collections.IEnumerator PunchAnimation(Transform target)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 punchScaleVec = Vector3.one * punchScale;
        
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < punchDuration / 2f)
        {
            target.localScale = Vector3.Lerp(originalScale, punchScaleVec, elapsed / (punchDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale down
        while (elapsed < punchDuration / 2f)
        {
            target.localScale = Vector3.Lerp(punchScaleVec, originalScale, elapsed / (punchDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        target.localScale = originalScale;
    }
    
    /// <summary>
    /// Reset score và coin (dùng khi restart game)
    /// </summary>
    public void ResetUI()
    {
        currentScore = 0;
        currentCoins = 0;
        UpdateScoreUI();
        UpdateCoinUI();
    }
    
    /// <summary>
    /// Set score trực tiếp (dùng khi load game)
    /// </summary>
    public void SetScore(int score)
    {
        currentScore = score;
        UpdateScoreUI();
    }
    
    /// <summary>
    /// Set coin trực tiếp (dùng khi load game)
    /// </summary>
    public void SetCoins(int coins)
    {
        currentCoins = coins;
        UpdateCoinUI();
    }
    
    public int GetCurrentScore() => currentScore;
    public int GetCurrentCoins() => currentCoins;
}
