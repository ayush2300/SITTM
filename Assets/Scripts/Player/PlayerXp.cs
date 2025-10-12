using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro; // Required for TextMeshProUGUI

public class PlayerXp : MonoBehaviour
{
    [Header("XP Settings")]
    public int currentXP = 0;
    public int level = 1;
    public int xpToNextLevel = 100;
    [Range(0f, 100f)]
    public float xpIncreasePercent = 25f; // Percent increase for next level XP

    [Header("UI")]
    public Slider xpBar; // Optional UI reference
    public float fillDuration = 0.5f; // Duration of smooth fill
    public TextMeshProUGUI levelText; // NEW: display current level

    [Header("Card Spawner")]
    public CardSpawner cardSpawner;

    private void Start()
    {
        if (xpBar != null)
            xpBar.value = 0f; // Start empty
        UpdateLevelText();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXpBar();
    }

    void LevelUp()
    {
        level++;

        // Increase XP required for next level by percentage
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * (1 + xpIncreasePercent / 100f));

        // Reset current XP for the new level
        currentXP = 0;

        Debug.Log($"Level Up! You are now level {level}. Next level requires {xpToNextLevel} XP.");

        // Reset XP bar to 0
        if (xpBar != null)
        {
            xpBar.value = 0f;
        }

        // Update level text
        UpdateLevelText();

        // Show card spawner if assigned
        if (cardSpawner != null)
        {
            cardSpawner.gameObject.SetActive(true); // activates panel and triggers OnEnable
        }
    }

    private void UpdateXpBar()
    {
        if (xpBar != null)
        {
            float targetValue = (float)currentXP / xpToNextLevel;
            xpBar.DOValue(targetValue, fillDuration).SetEase(Ease.OutCubic);
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = level.ToString();
    }
}
