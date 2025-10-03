using System.Collections.Generic;
using UnityEngine;

public class LaserWeaponSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Range(1, 3)] public int range = 1;      // 1 = Right, 2 = Right+Left, 3 = Right+Left+Up
    public GameObject laserPrefab;           // Prefab designed to fire right

    [Header("Offsets (distance from center)")]
    public float offsetRight = 0.5f;
    public float offsetLeft = 0.5f;
    public float offsetUp = 0.5f;

    [Header("Oscillation Settings")]
    public float moveRange = 0.5f;   // How far they move
    public float moveSpeed = 2f;     // Speed of oscillation

    private readonly List<GameObject> activeLasers = new List<GameObject>();

    void OnEnable()
    {
        SpawnLasers();
    }

    void OnDisable()
    {
        ClearLasers();
    }

    void SpawnLasers()
    {
        ClearLasers();
        if (laserPrefab == null) return;

        // Always spawn Right
        CreateLaser(Vector2.right, 0f, offsetRight);

        if (range >= 2)
            CreateLaser(Vector2.left, 180f, offsetLeft);

        if (range >= 3)
            CreateLaser(Vector2.up, 90f, offsetUp);
    }

    void CreateLaser(Vector2 dir, float angle, float distanceOffset)
    {
        GameObject laser = Instantiate(laserPrefab, transform);

        // Position relative to spawner
        laser.transform.localPosition = dir * distanceOffset;

        // Rotate correctly
        laser.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Add oscillator
        LaserOscillator osc = laser.AddComponent<LaserOscillator>();
        osc.range = moveRange;
        osc.speed = moveSpeed;
        osc.startPos = laser.transform.localPosition;

        // Random phase for non-simultaneous motion
        osc.phaseOffset = Random.Range(0f, Mathf.PI * 2f);

        activeLasers.Add(laser);
    }

    void ClearLasers()
    {
        foreach (var laser in activeLasers)
            if (laser != null) Destroy(laser);

        activeLasers.Clear();
    }
}

/// <summary>
/// Oscillates along the laser's local perpendicular axis with random phase.
/// </summary>
public class LaserOscillator : MonoBehaviour
{
    public float range = 0.5f;
    public float speed = 2f;
    public float phaseOffset = 0f;
    [HideInInspector] public Vector3 startPos;

    void Update()
    {
        // Local perpendicular direction
        Vector3 perpDir = Vector3.up; // default for Right/Left lasers
        if (Mathf.Approximately(transform.localRotation.eulerAngles.z, 90f) ||
            Mathf.Approximately(transform.localRotation.eulerAngles.z, 270f))
        {
            // Up/Down laser → move horizontally
            perpDir = Vector3.right;
        }

        float offset = Mathf.Sin(Time.time * speed + phaseOffset) * range;
        transform.localPosition = startPos + perpDir * offset;
    }
}
