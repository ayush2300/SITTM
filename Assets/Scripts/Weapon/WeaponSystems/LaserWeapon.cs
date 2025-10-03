using UnityEngine;
using System.Collections;

public class ScanLaserWeapon : MonoBehaviour
{
    [Header("Laser Settings")]
    public GameObject laserPrefab;
    public float laserLength = 5f;
    public float laserWidth = 0.2f;
    public float moveSpeed = 2f;

    [Header("Scan Points")]
    public Transform topPoint;
    public Transform bottomPoint;

    private GameObject activeLaser;
    private bool movingUp = true;

    void OnEnable()
    {
        SpawnLaser();
    }

    void OnDisable()
    {
        if (activeLaser != null) Destroy(activeLaser);
    }

    void Update()
    {
        if (activeLaser == null || topPoint == null || bottomPoint == null) return;

        // Move laser between points
        Vector3 target = movingUp ? topPoint.position : bottomPoint.position;
        activeLaser.transform.position = Vector3.MoveTowards(
            activeLaser.transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        // Switch direction when reaching point
        if (Vector3.Distance(activeLaser.transform.position, target) < 0.05f)
        {
            movingUp = !movingUp;
        }
    }

    void SpawnLaser()
    {
        if (laserPrefab == null) return;

        activeLaser = Instantiate(laserPrefab, transform);
        activeLaser.transform.localPosition = Vector3.zero;
        activeLaser.transform.localRotation = Quaternion.identity;

        // Set initial size
        activeLaser.transform.localScale = new Vector3(laserLength, laserWidth, 1f);
    }
}
