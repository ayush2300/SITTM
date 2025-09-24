using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserWeapon : MonoBehaviour
{
    [Header("Laser Settings")]
    [Range(1, 4)] public int range = 1;  // 1=Right, 2=Left, 3=Up, 4=Down
    public GameObject laserPrefab;

    [Header("Laser Size")]
    public float targetLength = 5f;
    public float laserWidth = 0.2f;
    public float growDuration = 1f;

    [Header("Offsets (distance from player)")]
    public float offsetRight = 0.5f;
    public float offsetLeft = 0.5f;
    public float offsetUp = 0.5f;
    public float offsetDown = 0.5f;

    [Header("Pendulum Settings")]
    [Range(0f, 90f)] public float deg = 15f; // max swing angle
    public float swingSpeed = 2f;            // speed of swinging

    private List<GameObject> activeLasers = new List<GameObject>();
    private Transform playerTransform;

    void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("⚠️ No object with tag 'Player' found in the scene!");
        }
    }

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

        if (range >= 1) CreateLaser(Vector2.right, offsetRight);
        if (range >= 2) CreateLaser(Vector2.left, offsetLeft);
        if (range >= 3) CreateLaser(Vector2.up, offsetUp);
        if (range >= 4) CreateLaser(Vector2.down, offsetDown);
    }

    void CreateLaser(Vector2 dir, float offset)
    {
        if (playerTransform == null) return;

        GameObject laser = Instantiate(laserPrefab, playerTransform);

        // Position relative to player
        laser.transform.localPosition = dir * offset;

        // Base direction rotation
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Start at base rotation
        laser.transform.localRotation = Quaternion.Euler(0f, 0f, baseAngle);

        // Start small
        laser.transform.localScale = new Vector3(0f, laserWidth, 1f);

        // Grow animation
        StartCoroutine(GrowLaser(laser));

        // Swing animation
        StartCoroutine(PendulumSwing(laser, baseAngle));

        activeLasers.Add(laser);
    }

    IEnumerator GrowLaser(GameObject laser)
    {
        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            float currentLength = Mathf.Lerp(0f, targetLength, t);

            if (laser != null)
                laser.transform.localScale = new Vector3(currentLength, laserWidth, 1f);

            yield return null;
        }
    }

    IEnumerator PendulumSwing(GameObject laser, float baseAngle)
    {
        while (laser != null)
        {
            float angle = Mathf.Sin(Time.time * swingSpeed) * deg;
            laser.transform.localRotation = Quaternion.Euler(0f, 0f, baseAngle + angle);
            yield return null;
        }
    }

    void ClearLasers()
    {
        foreach (var l in activeLasers)
        {
            if (l != null) Destroy(l);
        }
        activeLasers.Clear();
    }
}
