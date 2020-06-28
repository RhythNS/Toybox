using System.Collections.Generic;
using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    /// <summary>
    /// Generates Waterspots based on the lowest y value of the noise.
    /// </summary>
    /// <param name="waterSpots">A list of lists of verts in a water spot.</param>
    /// <param name="alreadyCheckedForWater">A 2D array which marks which positions have already been checked for.</param>
    /// <param name="noiseArray">The 2D array of generated noise.</param>
    /// <param name="minVal">The minimal value of the generated noise</param>
    private void GenerateWaterSpots(out List<List<Vector2Int>> waterSpots, out Fast2DArray<bool> alreadyCheckedForWater,
        Fast2DArray<float> noiseArray, float minVal)
    {
        waterSpots = new List<List<Vector2Int>>();
        alreadyCheckedForWater = new Fast2DArray<bool>(xSize, ySize);

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // if the noise is under waterlevel and it has not been checked already
                if (noiseArray.Get(x, y) < maxWaterLevel && alreadyCheckedForWater.Get(x, y) == false)
                {
                    waterSpots.Add(AllNeighbours(noiseArray, new Vector2Int(x, y), minVal, maxWaterLevel, alreadyCheckedForWater));
                }
            }
        }
    }

    /// <summary>
    /// Generates the water planes on top of every waterSpot.
    /// </summary>
    private List<Transform> GenerateWaterPlanes(List<List<Vector2Int>> waterSpots, Fast2DArray<Vector3> verts)
    {
        Transform waterParent = new GameObject("Water Planes").transform;
        waterParent.parent = map.transform;

        List<Transform> waterPlanes = new List<Transform>();

        List<Vector3> waterVerts = new List<Vector3>
        {
            verts.Get(0, 0),
            verts.Get(verts.XSize - 1, 0),
            verts.Get(0, verts.YSize - 1),
            verts.Get(verts.XSize - 1, verts.YSize - 1)
        };

        // change y
        for (int i = 0; i < waterVerts.Count; i++)
        {
            Vector3 temp = waterVerts[i];
            temp.y = maxWaterLevel * amplitude;
            waterVerts[i] = temp;
        }

        List<int> tris = new List<int>
        {
            0, 2, 1,
            2, 3, 1
        };

        GameObject plane = new GameObject("Water Plane");
        plane.transform.parent = waterParent;
        Mesh mesh = GenerateMesh(plane, waterVerts, tris, waterMaterial);
        plane.AddComponent<MeshCollider>().sharedMesh = mesh;
        waterPlanes.Add(plane.transform);


        /*
        foreach (List<Vector2Int> waterSpot in waterSpots)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (Vector2Int point in waterSpot)
            {
                if (point.x < minX)
                    minX = point.x;
                if (point.x > maxX)
                    maxX = point.x;
                if (point.y < minY)
                    minY = point.y;
                if (point.y > maxY)
                    maxY = point.y;
            }

            // Set all y values to the water level and add/ subtract one stepsize from their position
            float newY = maxWaterLevel * amplitude;
            float stepSize = this.stepSize * 2;

            Vector3 zero = verts.Get(minX, minY);
            zero.Set(zero.x - stepSize, newY, zero.z - stepSize);

            Vector3 one = verts.Get(minX, maxY);
            one.Set(one.x - stepSize, newY, one.z + stepSize);

            Vector3 two = verts.Get(maxX, maxY);
            two.Set(two.x + stepSize, newY, two.z + stepSize);

            Vector3 three = verts.Get(maxX, minY);
            three.Set(three.x + stepSize, newY, three.z - stepSize);

            List<Vector3> waterVerts = new List<Vector3>(4)
            {
                zero, one, two, three
            };

            List<int> tris = new List<int>(6)
            {
                0,1,3,
                1,2,3
            };

            GameObject plane = new GameObject("Water Plane");
            plane.transform.parent = waterParent;
            GenerateMesh(plane, waterVerts, tris, waterMaterial);
            plane.AddComponent<BoxCollider>();
            waterPlanes.Add(plane.transform);
        }
        */
        return waterPlanes;
    }

    /// <summary>
    /// Creates a List of all neighbours for a given point based on a min and max float value.
    /// </summary>
    /// <param name="noiseArray">Array containing all points of the generated noise</param>
    /// <param name="startingPoint">The starting vert</param>
    /// <param name="min">The lowest the noise can be</param>
    /// <param name="max">The highest the noise can be</param>
    /// <param name="alreadyChecked">A 2D array which marks which positions have already been checked for</param>
    /// <returns>A list of all verts of all neighbours</returns>
    private List<Vector2Int> AllNeighbours(Fast2DArray<float> noiseArray, Vector2Int startingPoint, float min, float max, Fast2DArray<bool> alreadyChecked = null)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        Queue<Vector2Int> toConsider = new Queue<Vector2Int>();

        if (alreadyChecked == null)
            alreadyChecked = new Fast2DArray<bool>(noiseArray.XSize, noiseArray.YSize);

        toConsider.Enqueue(startingPoint);
        alreadyChecked.Set(true, startingPoint.x, startingPoint.y);

        while (toConsider.Count > 0)
        {
            Vector2Int toInspect = toConsider.Dequeue();
            float noiseVal = noiseArray.Get(toInspect.x, toInspect.y);

            // If the noise value is inside the range that we are trying to find
            if (MathUtil.InRangeInclusive(min, max, noiseVal) == false)
                continue;

            neighbours.Add(toInspect);

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        Vector2Int newValue = new Vector2Int(toInspect.x + x, toInspect.y + y);
                        // is the value inside the array bounds and has not been already checked
                        // then add it to the toConsider Queue
                        if (noiseArray.InBounds(newValue.x, newValue.y) && alreadyChecked.Get(newValue.x, newValue.y) == false)
                        {
                            // Set the current tile to already checked
                            alreadyChecked.Set(true, newValue.x, newValue.y);
                            toConsider.Enqueue(newValue);
                        }
                    }
                }
            }
        } // end while

        return neighbours;
    }

}
