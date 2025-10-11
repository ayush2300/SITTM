using UnityEngine;
using Cinemachine;

public class CameraClampFixed2D : MonoBehaviour
{
    [Header("Clamp Settings")]
    public float minY = -14f;
    public float maxY = 10f;

    [Header("Optional X Clamp")]
    public float minX = float.NegativeInfinity;
    public float maxX = float.PositiveInfinity;

    private CinemachineVirtualCamera vcam;
    private Transform followTarget;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam != null && vcam.Follow != null)
            followTarget = vcam.Follow;
    }

    void LateUpdate()
    {
        if (vcam == null || followTarget == null) return;

        // Get the camera’s current position
        Vector3 camPos = vcam.transform.position;

        // Clamp only the camera’s position, not the player
        camPos.y = Mathf.Clamp(camPos.y, minY, maxY);
        camPos.x = Mathf.Clamp(camPos.x, minX, maxX);

        vcam.transform.position = camPos;
    }
}
