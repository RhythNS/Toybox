using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets a collectionPointValue to the nearest collection point
/// </summary>
public class SetNearestCollectionPoint : BoolNode
{
    [SerializeField] CollectionPointValue collectionPoint;
    [SerializeField] private ConnectionType connectedToCity;

    private enum ConnectionType
    {
        ConnectedToCity, NotConnectedToCity, Either
    }

    [SerializeField] private bool checkFillAmount;
    [SerializeField] private float maxFillAmount;
    [SerializeField] private float minFillAmount;

    public override int MaxNumberOfChildren => 0;

    protected override bool InnerIsFulfilled()
    {
        Toy toy = tree.AttachedBrain.GetComponent<Toy>();
        List<CollectionPoint> collectionPoints = toy.Town.CollectionPoints;

        CollectionPoint bestPoint = null;
        float bestDistance = float.MaxValue;

        foreach (CollectionPoint newCollectionPoint in collectionPoints)
        {
            // Is the collection point connected or not connected to the city
            switch (connectedToCity)
            {
                case ConnectionType.ConnectedToCity:
                    if (newCollectionPoint.IsConnectedToTownSupply == false)
                        continue;
                    break;

                case ConnectionType.NotConnectedToCity:
                    if (newCollectionPoint.IsConnectedToTownSupply == true)
                        continue;
                    break;
            }

            // check to see if the fill amount is between minFillAmount and maxFillAmount
            if (checkFillAmount == true)
            {
                float fillPercentage = (float)newCollectionPoint.CurrentCapacity / (float)newCollectionPoint.MaxCapcity;
                if (MathUtil.InRangeInclusive(minFillAmount, maxFillAmount, fillPercentage) == false)
                    continue;
            }

            // if newDistance is better than the previous distance then set the bestDistance to the new one
            float newDistance = (toy.transform.position - newCollectionPoint.transform.position).sqrMagnitude;
            if (newDistance < bestDistance)
            {
                bestDistance = newDistance;
                bestPoint = newCollectionPoint;
            }
        }

        // if no best point was found return failure
        if (bestPoint == null)
            return false;

        // Set the best point to the collection point and return success
        collectionPoint.SetValue(bestPoint);
        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetNearestCollectionPoint setNearest = CreateInstance<SetNearestCollectionPoint>();
        setNearest.connectedToCity = connectedToCity;
        setNearest.checkFillAmount = checkFillAmount;
        setNearest.maxFillAmount = maxFillAmount;
        setNearest.minFillAmount = minFillAmount;
        setNearest.collectionPoint = (CollectionPointValue)CloneValue(originalValueForClonedValue, collectionPoint);
        return setNearest;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(collectionPoint) == true)
            collectionPoint = (CollectionPointValue)originalReplace[collectionPoint];
    }
}
