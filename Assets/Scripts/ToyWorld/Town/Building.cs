using UnityEngine;

/// <summary>
/// Representation of a building
/// </summary>
public class Building : MonoBehaviour
{
    protected virtual bool ShouldBlockNavMesh { get; } = true;

    // This point is used when moving the building with the mouse
    public Transform MovePoint => movePoint;
    [SerializeField] private Transform movePoint;

    public Town Town { get; set; }

    public Vector3 BoxCenter => boxCenter;
    [SerializeField] private Vector3 boxCenter;

    public Vector3 BoxSize => boxSize;
    [SerializeField] private Vector3 boxSize;

    public virtual void OnBuildingPlaced()
    {
        if (ShouldBlockNavMesh == false)
            return;

        // Add this to the navmesh
        NavManager.Instance.AddTransform(transform, 0);
        NavManager.Instance.UpdateMesh();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // https://forum.unity.com/threads/gizmo-rotation.4817/
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
