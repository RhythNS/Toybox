using UnityEngine;

public class Rice : Resource
{
    [SerializeField] Transform[] collectionPoints;
    [SerializeField] private float collectionPointRadius;

    public override ResourceType ResourceType => ResourceType.Food;
    public override ToolType RequiredToolType => ToolType.Shovel;

    private int atIndex = -1;
    public override Circle3D NextCollectionPoint
    {
        get
        {
            if (++atIndex == collectionPoints.Length)
                atIndex = 0;
            return new Circle3D(collectionPoints[atIndex].position, collectionPointRadius);
        }
    }

    public override bool AtCollectionPoint(Vector3 position)
    {
        foreach (Transform collectionPoint in collectionPoints)
        {
            if ((collectionPoint.position - position).sqrMagnitude < collectionPointRadius * collectionPointRadius * 1.1f)
                return true;
        }
        return false;
    }

    public override GatheringDrop Gather(out ResourceItem item)
    {
        item = ScriptableObject.CreateInstance<RiceItem>();
        item.Amount = Random.Range(1, Mathf.Min(3, AmountLeft));
        AmountLeft -= item.Amount;
        return GatheringDrop.SpawnInWorld;
    }

    protected override void OnNoAmountLeft()
    {
        base.OnNoAmountLeft();
        GetComponent<RiceFieldBuilding>().OnNoAmountLeft();
    }

    // TODO:
    public override bool TryCollecting(Tool tool)
    {
        return true;
    }


    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < collectionPoints.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(collectionPoints[i].position, collectionPointRadius);
        }
    }

}
