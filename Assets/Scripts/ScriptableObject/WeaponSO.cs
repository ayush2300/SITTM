using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public float cooldown = 2f;
    public float activeDuration = 5f;
}
