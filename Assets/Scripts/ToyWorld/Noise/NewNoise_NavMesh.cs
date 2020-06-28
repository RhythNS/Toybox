using System.Collections.Generic;
using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    private void GenerateNavMesh(List<Transform> waterPlanes)
    {
        NavManager navManager = NavManager.Instance;

        // Add the map to the NavMesh
        navManager.AddTransform(map.transform, 0);

        // Add all waterPlates to the NavMesh
        for (int i = 0; i < waterPlanes.Count; i++)
            navManager.AddTransform(waterPlanes[i], 1);

        navManager.UpdateMesh();
    }

}
