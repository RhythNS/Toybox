using UnityEngine;

public partial class Toy : MonoBehaviour, IDieable, ICommandable
{
    /// <summary>
    /// Resets the assigned job.
    /// </summary>
    public void ResetAssignments()
    {
        assignedCollectionPoint = null;
        assignedResourceField = null;
        assignedItemToPickUp = null;
        BrainRecognizedJobChange = false;
    }

    /// <summary>
    /// Returns wheter the given job is assigned to the toy
    /// </summary>
    public bool JobAssigned(ToyJobs job)
    {
        switch (job)
        {
            case ToyJobs.ResourceField:
                return assignedResourceField != null;
            case ToyJobs.Itempickup:
                return assignedItemToPickUp != null;
            case ToyJobs.CollectionPoint:
                return assignedCollectionPoint != null;
        }
        Debug.LogError("Umimplented case: " + job);
        return false;
    }

    /// <summary>
    /// Called when camera controller right clicked somewhere
    /// </summary>
    /// <param name="hit"></param>
    public void Interact(RaycastHit hit)
    {
        Transform hitTrans = hit.collider.gameObject.transform;

        if (hitTrans.TryGetComponent(out Resource resource))
        {
            AssignedResourceField = resource.ParentField;
        }
        else if (hitTrans.TryGetComponent(out ItemInWorld itemInWorld))
        {
            AssignedItemToPickUp = itemInWorld;
        }
        else if (hitTrans.TryGetComponent(out CollectionPoint collectionPoint))
        {
            AssignedCollectionPoint = collectionPoint;
        }
    }
}
