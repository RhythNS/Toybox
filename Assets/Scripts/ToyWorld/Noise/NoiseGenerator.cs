using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public GameObject grassPlate;
    public Material material;

    public int xSize, ySize;
    public float stepSize = 0.3f;
    public float featureSize = 1;
    public float amplitude = 1;
    public float distortionFactor = 0.2f;
    public Gradient gradient;

    private GameObject map;

    private void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (map != null)
            Destroy(map);
        map = new GameObject();
        MeshRenderer renderer = map.AddComponent<MeshRenderer>();
        MeshFilter filter = map.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;

        OpenSimplexNoise noiseGenerator = new OpenSimplexNoise((long)(Random.value * long.MaxValue));
        OpenSimplexNoise distortionNoise = new OpenSimplexNoise((long)(Random.value * long.MaxValue));

        float minVal = float.MaxValue, maxValue = float.MinValue;

        float halfStep = stepSize / 2;

        List<Vector3> verticies = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float xPer = x / featureSize;
                float yPer = y / featureSize;
                float noise = (float)noiseGenerator.Evaluate(xPer, yPer) + ((float)distortionNoise.Evaluate(xPer, yPer) * distortionFactor);

                if (noise < minVal)
                    minVal = noise;
                if (noise > maxValue)
                    maxValue = noise;

                float amplNoise = noise * amplitude;
                //print("x:" + xPer + ", y:" + yPer + ", n:" + noise + ", ampNo:" + amplNoise);

                float xAdder = 0, yAdder = 0;
                if (x != 0 && x != xSize - 1 && y != 0 && y != ySize - 1)
                {
                    xAdder = Random.Range(-halfStep, halfStep);
                    yAdder = Random.Range(-halfStep, halfStep);
                }

                verticies.Add(new Vector3(x * stepSize + xAdder, amplNoise, y * stepSize + yAdder));
                colors.Add(gradient.Evaluate((noise + 1) / 2));
                //verticies.Add(new Vector3(x * stepSize, Random.Range(-4,4), y * stepSize));
            }
        }

        print("Max:" + maxValue + ", Min:" + minVal);

        for (int y = 1; y < ySize; y++)
        {
            for (int x = 0; x < xSize - 1; x++)
            {
                int zero = (y - 1) * xSize + x;
                int one = zero + 1;
                int two = y * xSize + x;
                int three = two + 1;

                triangles.Add(zero);
                triangles.Add(two);
                triangles.Add(three);

                triangles.Add(zero);
                triangles.Add(three);
                triangles.Add(one);
            }
        }
        renderer.material = material;

        mesh.vertices = verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
    }

}
