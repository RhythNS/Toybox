using System.Collections.Generic;
using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    private Fast2DArray<float> GenerateBaseNoise(out float minValue, out float maxValue)
    {
        minValue = float.MaxValue;
        maxValue = float.MinValue;

        Fast2DArray<float> noiseArray = new Fast2DArray<float>(xSize, ySize);

        OpenSimplexNoise noiseGenerator = new OpenSimplexNoise((long)(Random.value * long.MaxValue));
        OpenSimplexNoise distortionNoise = new OpenSimplexNoise((long)(Random.value * long.MaxValue));

        // Iterate through the entire map
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // modify x and y with the feature size
                float xPer = x / featureSize;
                float yPer = y / featureSize;

                // Get the base noise and add some distortion to it
                float noise = (float)noiseGenerator.Evaluate(xPer, yPer) + ((float)distortionNoise.Evaluate(xPer, yPer) * distortionFactor);
                noiseArray.Set(noise, x, y);

                // check for min and max value of the generated noise
                if (noise < minValue)
                    minValue = noise;
                if (noise > maxValue)
                    maxValue = noise;
            }
        }

        return noiseArray;
    }

    private Fast2DArray<Vector3> GenerateBaseVerts(Fast2DArray<float> noiseArray, List<List<Vector2Int>> waterSpots, out float minHeight, out float maxHeight)
    {
        Fast2DArray<Vector3> verticies = new Fast2DArray<Vector3>(xSize, ySize);

        minHeight = float.MaxValue;
        maxHeight = float.MinValue;

        float halfStep = stepSize / 2;

        // Iterate through the entire map
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // get the baseNoise and multiply it by the amplitude
                float amplNoise = noiseArray.Get(x, y) * amplitude;

                if (amplNoise < minHeight)
                    minHeight = amplNoise;
                if (amplNoise > maxHeight)
                    maxHeight = amplNoise;

                // Random distortion of the vertex position
                float xAdder = 0, yAdder = 0;

                // Ignore the edge
                if (x != 0 && x != xSize - 1 && y != 0 && y != ySize - 1)
                {
                    xAdder = Random.Range(-halfStep, halfStep);
                    yAdder = Random.Range(-halfStep, halfStep);
                }

                // if it is water point then slightly lower the land
                for (int i = 0; i < waterSpots.Count; i++)
                {
                    if (waterSpots[i].Contains(new Vector2Int(x, y)))
                    {
                        amplNoise -= 3f;
                    }
                }

                verticies.Set(new Vector3(x * stepSize + xAdder, amplNoise, y * stepSize + yAdder), x, y);
            }
        }

        return verticies;
    }

    private void GenerateBaseMesh(Fast2DArray<Vector3> verticies, float minHeight, float maxHeight)
    {
        List<Vector3> newVerts = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        int vertCount = 0;

        for (int x = 1; x < xSize; x++)
        {
            for (int y = 0; y < ySize - 1; y++)
            {
                Vector3 zero = verticies.Get(x - 1, y);
                Vector3 one = verticies.Get(x - 1, y + 1);
                Vector3 two = verticies.Get(x, y);
                Vector3 three = verticies.Get(x, y + 1);

                // first triangle
                newVerts.Add(three);
                newVerts.Add(two);
                newVerts.Add(zero);

                triangles.Add(vertCount++);
                triangles.Add(vertCount++);
                triangles.Add(vertCount++);

                float lowestY = Mathf.Min(three.y, two.y, zero.y);
                Color color = gradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, lowestY));
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);


                // second triangle
                newVerts.Add(one);
                newVerts.Add(three);
                newVerts.Add(zero);

                triangles.Add(vertCount++);
                triangles.Add(vertCount++);
                triangles.Add(vertCount++);

                lowestY = Mathf.Min(one.y, three.y, zero.y);
                color = gradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, lowestY));
                colors.Add(color);
                colors.Add(color);
                colors.Add(color);
            }
        }

        // Generate the map mesh
        GenerateMesh(map, newVerts, triangles, material, colors);
    }
}
