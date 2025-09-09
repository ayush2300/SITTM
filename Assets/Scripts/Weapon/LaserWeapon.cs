using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject laserPrefab;  // Assign the cube prefab
    public float laserLength = 10f; // Total length of each laser
    public float laserThickness = 0.5f;
    public float damage = 20f;
    public float spawnOffset = 1f;  // Distance gap from player center

    private Transform player;
    private GameObject leftLaser;
    private GameObject rightLaser;

    void Awake()
    {
        player = transform.parent; // Weapon is child of player
    }

    void OnEnable()
    {
        SpawnLasers();
    }

    void OnDisable()
    {
        if (leftLaser != null) Destroy(leftLaser);
        if (rightLaser != null) Destroy(rightLaser);
    }

    void Update()
    {
        if (player == null) return;

        // Keep lasers following the player with the offset
        if (leftLaser != null)
            leftLaser.transform.position = player.position + Vector3.left * (spawnOffset + laserLength / 2);

        if (rightLaser != null)
            rightLaser.transform.position = player.position + Vector3.right * (spawnOffset + laserLength / 2);
    }

    void SpawnLasers()
    {
        // Spawn left laser
        leftLaser = Instantiate(laserPrefab);
        leftLaser.transform.localScale = new Vector3(laserLength, laserThickness, 1);
        ProjectileDamage leftDamage = leftLaser.AddComponent<ProjectileDamage>();
        leftDamage.damage = damage;

        // Spawn right laser
        rightLaser = Instantiate(laserPrefab);
        rightLaser.transform.localScale = new Vector3(laserLength, laserThickness, 1);
        ProjectileDamage rightDamage = rightLaser.AddComponent<ProjectileDamage>();
        rightDamage.damage = damage;
    }
}
