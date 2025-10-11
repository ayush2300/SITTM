using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public List<WeaponSO> startingWeapons; // Pre-configured starting weapons (ScriptableObjects)

    private List<ActiveWeapon> activeWeapons = new List<ActiveWeapon>();

    private class ActiveWeapon
    {
        public WeaponSO weaponSO;
        public int currentLevel;
        public GameObject instance;
        public float cooldownTimer;
        public float activeTimer;
        public float cooldown;
        public float activeDuration;
    }

    void Start()
    {
        // Spawn initial weapons (if any in the list)
        foreach (var weaponSO in startingWeapons)
        {
            AddWeaponFromSO(weaponSO, 0); // Start at level 0
        }
    }

    void Update()
    {
        for (int i = activeWeapons.Count - 1; i >= 0; i--)
        {
            var weapon = activeWeapons[i];

            // Cooldown countdown
            if (weapon.cooldownTimer > 0)
                weapon.cooldownTimer -= Time.deltaTime;

            // Active timer countdown
            if (weapon.instance.activeSelf && weapon.activeTimer > 0)
            {
                weapon.activeTimer -= Time.deltaTime;

                if (weapon.activeTimer <= 0)
                {
                    // Deactivate weapon and start cooldown
                    weapon.instance.SetActive(false);
                    weapon.cooldownTimer = weapon.cooldown;
                }
            }

            // Reactivate after cooldown
            if (!weapon.instance.activeSelf && weapon.cooldownTimer <= 0f)
            {
                weapon.instance.SetActive(true);
                weapon.activeTimer = weapon.activeDuration;
            }
        }
    }

    /// <summary>
    /// Adds a weapon dynamically from a WeaponSO at a given level.
    /// </summary>
    public void AddWeaponFromSO(WeaponSO weaponSO, int levelIndex)
    {
        if (weaponSO.levels == null || weaponSO.levels.Count == 0)
        {
            Debug.LogWarning($"WeaponSO {weaponSO.weaponName} has no levels!");
            return;
        }

        if (levelIndex < 0 || levelIndex >= weaponSO.levels.Count)
            levelIndex = 0; // fallback to level 0

        var levelData = weaponSO.levels[levelIndex];

        // Apply cooldown & active time modifiers
        float modifiedCooldown = CooldownDecreaseItem.GetModifiedCooldown(levelData.time.coolDownTime);
        float modifiedActiveTime = ActiveTimeIncrease.GetModifiedActiveTime(levelData.time.activeTime);

        GameObject weaponInstance = Instantiate(levelData.weaponLevelPrefab, transform.position, Quaternion.identity, transform);
        weaponInstance.SetActive(true);

        ActiveWeapon activeWeapon = new ActiveWeapon
        {
            weaponSO = weaponSO,
            currentLevel = levelIndex,
            instance = weaponInstance,
            cooldown = modifiedCooldown,
            activeDuration = modifiedActiveTime,
            cooldownTimer = 0f,
            activeTimer = modifiedActiveTime
        };

        activeWeapons.Add(activeWeapon);
    }

    /// <summary>
    /// Returns the current level of a weapon (or -1 if not owned).
    /// </summary>
    public int GetWeaponLevel(WeaponSO weaponSO)
    {
        var activeWeapon = activeWeapons.Find(w => w.weaponSO == weaponSO);
        return activeWeapon != null ? activeWeapon.currentLevel : -1;
    }

    /// <summary>
    /// Upgrades an existing weapon to the next level.
    /// </summary>
    public void UpgradeWeapon(WeaponSO weaponSO)
    {
        var activeWeapon = activeWeapons.Find(w => w.weaponSO == weaponSO);
        if (activeWeapon == null) return;

        int nextLevel = activeWeapon.currentLevel + 1;
        if (nextLevel >= weaponSO.levels.Count)
        {
            Debug.Log($"{weaponSO.weaponName} is already at max level!");
            return;
        }

        // Destroy old instance
        Destroy(activeWeapon.instance);

        // Spawn upgraded instance
        var newLevelData = weaponSO.levels[nextLevel];

        // Apply modifiers again (in case item effects changed)
        float modifiedCooldown = CooldownDecreaseItem.GetModifiedCooldown(newLevelData.time.coolDownTime);
        float modifiedActiveTime = ActiveTimeIncrease.GetModifiedActiveTime(newLevelData.time.activeTime);

        GameObject newInstance = Instantiate(newLevelData.weaponLevelPrefab, transform.position, Quaternion.identity, transform);
        newInstance.SetActive(true);

        // Update weapon data
        activeWeapon.instance = newInstance;
        activeWeapon.currentLevel = nextLevel;
        activeWeapon.cooldown = modifiedCooldown;
        activeWeapon.activeDuration = modifiedActiveTime;
        activeWeapon.cooldownTimer = 0f;
        activeWeapon.activeTimer = modifiedActiveTime;
    }

    /// <summary>
    /// Check if the player already owns this weapon.
    /// </summary>
    public bool HasWeapon(WeaponSO weaponSO)
    {
        return activeWeapons.Exists(w => w.weaponSO == weaponSO);
    }
}
