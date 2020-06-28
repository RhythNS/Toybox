using UnityEngine;

public class PlateGenerator : MonoBehaviour
{
    [SerializeField] private GameObject plate;
    [SerializeField] private float radiusPlate;
    [SerializeField] private int xSize, ySize;

    [SerializeField] private float featureSize = 1;
    [SerializeField] private float amplitude = 1;

    [SerializeField] private Vector3 tileScale;


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
        map = new GameObject("World");

        OpenSimplexNoise noise = new OpenSimplexNoise();

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                GameObject tile = Instantiate(plate, map.transform);

                float height = ((float)noise.Evaluate(x / featureSize, y / featureSize)) * amplitude;

                tile.transform.localScale = tileScale;
                tile.transform.position = new Vector3(x * radiusPlate, height, y * radiusPlate);
                tile.transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
            }
        }
    }
}
