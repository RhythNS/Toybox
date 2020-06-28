using System;
using UnityEngine;

/// <summary>
/// Dict for Meshes for a Resource based on the remaining resource
/// </summary>
public class ResourceItemDict : MonoBehaviour
{
    public static ResourceItemDict Instance;

    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct ResourceForObject
    {
        public GameObject gameObject;
        public ResourceType resourceType;
    }

    [SerializeField] private ResourceForObject[] resourceForObjects;

    public GameObject Get(ResourceType type)
        => Array.Find(resourceForObjects, x => x.resourceType == type).gameObject;

    [System.Serializable]
    public struct TypeForMesh
    {
        public ResourceType resourceType;
        public AmountForMesh[] amountForMeshes;

        [System.Serializable]
        public struct AmountForMesh
        {
            public int minimumAmount;
            public Mesh mesh;
        }
    }

    /// <summary>
    /// Should be sorted by lowest first
    /// </summary>
    [SerializeField] private TypeForMesh[] typesForMesh;

    public bool GetNewModel(ResourceType resourceType, int previousAmount, int newAmount, out Mesh newMesh)
    {
        foreach (TypeForMesh typeForMesh in typesForMesh)
        {
            if (typeForMesh.resourceType == resourceType)
            {
                if (newAmount < previousAmount) // the new amount is smaller than the prev amount, i.e. the plant was harvested
                {
                    // check for the first element that is less than the current amount.
                    for (int i = 0; i < typeForMesh.amountForMeshes.Length; i++)
                    {
                        // is the newAmount smaller then the minimum amount
                        if (newAmount < typeForMesh.amountForMeshes[i].minimumAmount)
                        {
                            // if the previous amount already had the same model, then ignore the request
                            if (previousAmount < typeForMesh.amountForMeshes[i].minimumAmount)
                            {
                                newMesh = default;
                                return false;
                            }

                            // Previous amount was higher and a new model is needed
                            newMesh = typeForMesh.amountForMeshes[i].mesh;
                            return true;
                        }
                    }
                }
                else // the newAmount is bigger than the previous amount, i.e. the plant grew
                {
                    // check for the first element that is bigger than the current amount
                    for (int i = typeForMesh.amountForMeshes.Length - 1; i > -1; i--)
                    {
                        if (newAmount > typeForMesh.amountForMeshes[i].minimumAmount)
                        {
                            // if the previous amount was already bigger than this, ignore the request
                            if (previousAmount > typeForMesh.amountForMeshes[i].minimumAmount)
                            {
                                newMesh = default;
                                return false;
                            }

                            // Previous amount was smaller and a new model is needed
                            newMesh = typeForMesh.amountForMeshes[i].mesh;
                            return true;
                        }
                    }
                }
            }
        }
        Debug.Log("RessourceType " + resourceType + " not found!");
        newMesh = default;
        return false;
    }

}
