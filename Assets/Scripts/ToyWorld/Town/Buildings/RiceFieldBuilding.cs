using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Building for the rice field
/// </summary>
public class RiceFieldBuilding : Building
{
    protected override bool ShouldBlockNavMesh => false;

    public override void OnBuildingPlaced()
    {
        base.OnBuildingPlaced();

        // Add this rice to a new rice resource field
        Transform newParent = new GameObject("Rice Resource Field").transform;
        newParent.parent = transform.parent;

        ResourceField field = newParent.gameObject.AddComponent<ResourceField>();
        List<Resource> resources = new List<Resource>();
        Rice rice = GetComponent<Rice>();
        rice.ParentField = field;
        resources.Add(rice);

        field.Set(resources);
    }

    public void OnNoAmountLeft() => Town.RemoveBuilding(this);
}
