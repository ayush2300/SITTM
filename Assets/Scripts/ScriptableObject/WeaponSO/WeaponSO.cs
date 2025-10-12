using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable] // ✅ makes it visible in Inspector
public class Timings
{
    public float coolDownTime;
    public float activeTime;
}

[System.Serializable]
public class Levels
{
    public Timings time;                // cooldown & active time per level
    public GameObject weaponLevelPrefab; // prefab per level
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;

    [Header("Per Level Settings")]
    public List<Levels> levels;
}
