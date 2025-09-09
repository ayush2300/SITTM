using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public GameObject weaponPrefab;
    public float cooldown;       // Seconds before weapon can be used again
    public float activeDuration; // How long the weapon stays active
}

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public List<WeaponData> weapons; // Pre-configured starting weapons

    private List<ActiveWeapon> activeWeapons = new List<ActiveWeapon>();

    private class ActiveWeapon
    {
        public GameObject instance;
        public float cooldownTimer;
        public float activeTimer;
        public float cooldown;
        public float activeDuration;
    }

    void Start()
    {
        // Spawn initial weapons (if any in the list)
        foreach (var weaponData in weapons)
        {
            AddWeapon(weaponData);
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
    /// Adds a weapon dynamically using WeaponData
    /// </summary>
    public void AddWeapon(WeaponData weaponData)
    {
        GameObject weaponInstance = Instantiate(weaponData.weaponPrefab, transform.position, Quaternion.identity, transform);
        weaponInstance.SetActive(true);

        ActiveWeapon activeWeapon = new ActiveWeapon
        {
            instance = weaponInstance,
            cooldown = weaponData.cooldown,
            activeDuration = weaponData.activeDuration,
            cooldownTimer = 0f,
            activeTimer = weaponData.activeDuration
        };

        activeWeapons.Add(activeWeapon);
    }

    /// <summary>
    /// Adds a weapon dynamically using WeaponSO
    /// </summary>
    public void AddWeaponFromSO(WeaponSO weaponSO)
    {
        WeaponData newWeapon = new WeaponData
        {
            weaponPrefab = weaponSO.weaponPrefab,
            cooldown = weaponSO.cooldown,
            activeDuration = weaponSO.activeDuration
        };

        AddWeapon(newWeapon);
    }
}
