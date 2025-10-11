using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Settings")]
    public bool canDestroyOnDeath = true;
    public bool isPlayer = false;
    public bool allowRevive = false;

    [Header("Death / Damage Events")]
    public UnityEvent onDeath;
    public UnityEvent onPostDeath;
    public UnityEvent onDamageTaken;

    [Header("Effects")]
    public GameObject deathEffect;

    [Header("Death Animation")]
    public float jumpPower = 0.6f;
    public int jumpNum = 1;
    public float jumpDuration = 0.45f;
    public float scaleDuration = 0.40f;
    public Ease scaleEase = Ease.InCubic;

    [Header("Hurt Settings")]
    public float hurtCooldown = 0.1f;

    [Header("UI")]
    public Slider healthSlider;

    private bool isDead = false;
    private float lastHurtTime = -1f;
    private SpriteRenderer sr;
    private Material mat;
    private Vector3 originalScale;
    private Sequence hurtSeq;

    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    private static readonly int ExposureID = Shader.PropertyToID("_Exposure");
    private static readonly int RedOverlayID = Shader.PropertyToID("_RedOverlay");
    private static readonly int TintColorID = Shader.PropertyToID("_TintColor");

    private Color originalSpriteColor = Color.white;
    private Color originalTintColor = Color.white;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr)
        {
            sr.material = new Material(sr.material);
            mat = sr.material;

            originalSpriteColor = sr.color;
            if (mat.HasProperty(TintColorID))
                originalTintColor = mat.GetColor(TintColorID);
        }

        originalScale = transform.localScale;
        UpdateHealthUI();
    }

    private void Update()
    {
        // ? Player-only passive regen logic
        if (isPlayer && !isDead && HealthRegenItem.IsActive())
        {
            float regenPerSec = HealthRegenItem.GetRegenAmountPerSecond(maxHealth);
            float healAmount = regenPerSec * Time.deltaTime;
            Heal(Mathf.RoundToInt(healAmount));
        }
    }

    public void Damage(int damageAmount)
    {
        if (isDead || damageAmount <= 0)
            return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayHurtEffect();
            onDamageTaken?.Invoke();
        }
    }

    public void Heal(int healAmount)
    {
        if (isDead || healAmount <= 0)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        UpdateHealthUI();
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        onDeath?.Invoke();

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        hurtSeq?.Kill();
        transform.DOKill();
        transform.localScale = originalScale;

        Sequence deathSeq = DOTween.Sequence().SetUpdate(true);

        deathSeq.Join(
            transform
                .DOJump(transform.position + Vector3.up * 0.05f, jumpPower, jumpNum, jumpDuration)
                .SetEase(Ease.OutQuad)
        );

        deathSeq.Join(
            transform
                .DOScale(Vector3.zero, scaleDuration)
                .SetEase(scaleEase)
        );

        deathSeq.OnComplete(() =>
        {
            onPostDeath?.Invoke();

            if (isPlayer && canDestroyOnDeath)
                Destroy(gameObject);
            else
            {
                gameObject.SetActive(false);
                gameObject.GetComponent<EnemyAI>().SpawnXp();
            }
        });
    }

    private void PlayHurtEffect()
    {
        if (Time.time - lastHurtTime < hurtCooldown) return;
        lastHurtTime = Time.time;

        if (sr == null || mat == null) return;

        hurtSeq?.Kill();
        sr.DOKill();
        transform.DOKill();

        transform.localScale = originalScale;

        float tIn = 0.05f;
        float hold = 0.05f;
        float tOut = 0.14f;
        float total = tIn + hold + tOut;

        float flashPeak = 0.6f;
        float exposurePeak = 1.15f;

        sr.color = originalSpriteColor;

        hurtSeq = DOTween.Sequence().SetUpdate(true);

        if (mat.HasProperty(RedOverlayID))
        {
            hurtSeq.Append(DOTween
                .To(() => mat.GetFloat(RedOverlayID), v => mat.SetFloat(RedOverlayID, v), 1f, tIn));
            hurtSeq.AppendInterval(hold);
            hurtSeq.Append(DOTween
                .To(() => mat.GetFloat(RedOverlayID), v => mat.SetFloat(RedOverlayID, v), 0f, tOut));
        }

        if (mat.HasProperty(TintColorID))
        {
            Color hitTint = new Color(1f, 0.5f, 0.5f, 1f);
            hurtSeq.Join(DOTween.To(() => mat.GetColor(TintColorID),
                c => mat.SetColor(TintColorID, c), hitTint, tIn * 0.8f));
            hurtSeq.Join(DOTween.To(() => mat.GetColor(TintColorID),
                c => mat.SetColor(TintColorID, c), originalTintColor, tOut).SetDelay(tIn + hold));
        }

        if (mat.HasProperty(FlashAmountID))
        {
            hurtSeq.Join(DOTween
                .To(() => mat.GetFloat(FlashAmountID), v => mat.SetFloat(FlashAmountID, v), flashPeak, tIn));
            hurtSeq.Join(DOTween.To(() => mat.GetFloat(ExposureID),
                v => mat.SetFloat(ExposureID, v), exposurePeak, tIn));

            hurtSeq.Join(DOTween
                .To(() => mat.GetFloat(FlashAmountID), v => mat.SetFloat(FlashAmountID, v), 0f, tOut)
                .SetDelay(tIn + hold));
            hurtSeq.Join(DOTween.To(() => mat.GetFloat(ExposureID),
                v => mat.SetFloat(ExposureID, v), 1f, tOut).SetDelay(tIn + hold));
        }

        hurtSeq.Join(transform.DOPunchScale(new Vector3(0.06f, 0.06f, 0f), total, vibrato: 0, elasticity: 0f));
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        transform.localScale = originalScale;
        gameObject.SetActive(true);
        UpdateHealthUI();
    }

    public void SetMaxHealth(int newMaxHealth, bool resetCurrentHealth = true)
    {
        maxHealth = newMaxHealth;
        if (resetCurrentHealth)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
}
