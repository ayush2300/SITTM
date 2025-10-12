using UnityEngine;
using Cinemachine;

public class CameraClamp2D : MonoBehaviour
{
    [Header("Clamp Settings")]
    public float minY = -14f;
    public float maxY = 10f;

    [Header("Optional X Clamp")]
    public float minX = float.NegativeInfinity;
    public float maxX = float.PositiveInfinity;

    public CinemachineVirtualCamera vcam;
    private Transform followTarget;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam != null)
            followTarget = vcam.Follow;
    }

    void LateUpdate()
    {
        if (followTarget == null) return;

        Vector3 pos = followTarget.position;

        // Clamp Y
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        // Optional: Clamp X
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        followTarget.position = pos;
    }
}
