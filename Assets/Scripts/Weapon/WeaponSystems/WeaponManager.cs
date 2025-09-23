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

            // Active timer
            if (weapon.instance.activeSelf && weapon.activeTimer > 0)
            {
                weapon.activeTimer -= Time.deltaTime;

                if (weapon.activeTimer <= 0)
                {
                    // Deactivate weapon
                    weapon.instance.SetActive(false);
                    // Start cooldown
                    weapon.cooldownTimer = weapon.cooldown;
                }
            }

            // Auto-reactivate after cooldown
            if (!weapon.instance.activeSelf && weapon.cooldownTimer <= 0f)
            {
                weapon.instance.SetActive(true);
                weapon.activeTimer = weapon.activeDuration;
            }
        }
    }

    /// <summary>
    /// Adds a weapon dynamically from a WeaponSO at a given level
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

        GameObject weaponInstance = Instantiate(levelData.weaponLevelPrefab, transform.position, Quaternion.identity, transform);
        weaponInstance.SetActive(true);

        ActiveWeapon activeWeapon = new ActiveWeapon
        {
            weaponSO = weaponSO,
            currentLevel = levelIndex,
            instance = weaponInstance,
            cooldown = levelData.time.coolDownTime,
            activeDuration = levelData.time.activeTime,
            cooldownTimer = 0f,
            activeTimer = levelData.time.activeTime
        };

        activeWeapons.Add(activeWeapon);
    }

    /// <summary>
    /// Upgrade an existing weapon to the next level
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

        // Spawn new upgraded instance
        var newLevelData = weaponSO.levels[nextLevel];
        GameObject newInstance = Instantiate(newLevelData.weaponLevelPrefab, transform.position, Quaternion.identity, transform);
        newInstance.SetActive(true);

        // Update weapon data
        activeWeapon.instance = newInstance;
        activeWeapon.currentLevel = nextLevel;
        activeWeapon.cooldown = newLevelData.time.coolDownTime;
        activeWeapon.activeDuration = newLevelData.time.activeTime;
        activeWeapon.cooldownTimer = 0f;
        activeWeapon.activeTimer = newLevelData.time.activeTime;
    }

    /// <summary>
    /// Check if player already has this weapon
    /// </summary>
    public bool HasWeapon(WeaponSO weaponSO)
    {
        return activeWeapons.Exists(w => w.weaponSO == weaponSO);
    }
}
