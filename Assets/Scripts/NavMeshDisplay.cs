using UnityEngine;

public class NavMeshDisplay : MonoBehaviour
{
#if UNITY_EDITOR
    Hash128 meshSignature;
    Mesh navigationMesh;

    [SerializeField] Color meshColor = new(0, 0, 1, 0.2f);
    [SerializeField] Color meshWireColor = new(0, 0, 1, 0.4f);

    void OnDrawGizmosSelected()
    {
        var triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
        Hash128 hash = new();
        foreach (var i in triangulation.indices)
        {
            hash.Append(0);
            hash.Append(i);
        }
        foreach (var i in triangulation.vertices)
        {
            hash.Append(1);
            hash.Append(i.x);
            hash.Append(i.y);
            hash.Append(i.z);
        }

        if (meshSignature != hash)
        {
            Debug.Log("recomputing mesh to draw");
            meshSignature = hash;

            navigationMesh ??= new Mesh();
            navigationMesh.Clear();
            navigationMesh.SetVertices(triangulation.vertices);
            navigationMesh.SetTriangles(triangulation.indices, 0);
            navigationMesh.RecalculateNormals();
        }

        Gizmos.color = meshColor;
        Gizmos.DrawMesh(navigationMesh);
        Gizmos.color = meshWireColor;
        Gizmos.DrawWireMesh(navigationMesh);
    }
#endif
}