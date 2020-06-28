using UnityEngine;

/// <summary>
/// Simple Implementation of a resource.
/// </summary>
public abstract class SimpleResource : Resource
{
    [SerializeField] protected Circle3D collectionPoint;

    public override Circle3D NextCollectionPoint => collectionPoint;

    public override bool AtCollectionPoint(Vector3 position)
        => (collectionPoint.position - position).sqrMagnitude < collectionPoint.radius * collectionPoint.radius * 1.1f;

    private void Start()
    {
        collectionPoint.SetPosition(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (Application.isPlaying == true)
            Gizmos.DrawWireSphere(collectionPoint.position, collectionPoint.radius);
        else
            Gizmos.DrawWireSphere(transform.position + collectionPoint.position, collectionPoint.radius);
    }

    // TODO:
    public override bool TryCollecting(Tool tool)
    {
        return true;
    }
}
