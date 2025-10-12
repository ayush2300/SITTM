using UnityEngine;
using Cinemachine;

[DefaultExecutionOrder(1000)] // ensures this runs after Cinemachine updates
public class CameraClampFixed2D : MonoBehaviour
{
    [Header("Clamp Settings")]
    public float minY = -14f;
    public float maxY = 10f;

    [Header("Optional X Clamp")]
    public float minX = float.NegativeInfinity;
    public float maxX = float.PositiveInfinity;

    private Camera mainCam;
    private CinemachineBrain brain;

    void Start()
    {
        mainCam = Camera.main;
        brain = mainCam != null ? mainCam.GetComponent<CinemachineBrain>() : null;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        Vector3 camPos = mainCam.transform.position;

        camPos.y = Mathf.Clamp(camPos.y, minY, maxY);
        camPos.x = Mathf.Clamp(camPos.x, minX, maxX);

        mainCam.transform.position = camPos;
    }
}
