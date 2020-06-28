using System.Collections.Generic;
using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    public static NewNoise Instance { get; private set; }

    public Vector2 WorldMapSize => new Vector2(xSize * stepSize, ySize * stepSize);
    public Vector2Int SimulatedMapSize => new Vector2Int(xSize, ySize);
    public Vector3 MapOffset => mapOffset;

    private GameObject map;
    private Transform objects;
    private Vector3 scaleVector;

    [Header("General")]
    [SerializeField] private int xSize = 80;
    [SerializeField] private int ySize = 80;
    [SerializeField] private Vector3 mapOffset;

    [Header("Looks")]
    [SerializeField] private Material material;
    [SerializeField] private Material waterMaterial;
    [SerializeField] private Gradient gradient;

    [Header("Modefiers")]
    [SerializeField] private float stepSize = 2f;
    [SerializeField] private float featureSize = 30f;
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float distortionFactor = 0.4f;
    [SerializeField] private float scaleOfObjects = 1f;
    [Range(-1, 1)] [SerializeField] private float maxWaterLevel = -0.5f;

    [Header("Resources")]
    [SerializeField] private float resourceFrequency = 0.03f;
    [SerializeField] private int resourceOctaves = 3;
    [SerializeField] private float resourceLacunarity = 2.0f;
    [SerializeField] private float resourceGain = 0.5f;
    [SerializeField] private float resourceSpawnChance = 0.5f;
    [SerializeField] private int minAmountInResourceField = 4;
    [SerializeField] private Vector2 forestRange = new Vector2(-1f, -0.7f);
    [SerializeField] private Resource[] forestPrefabs;
    [SerializeField] private Vector2 bambooRange = new Vector2(-0.1f, 0.1f);
    [SerializeField] private Resource[] bambooPrefabs;
    [SerializeField] private Vector2 stoneRange = new Vector2(0.7f, 1f);
    [SerializeField] private Resource[] stonePrefabs;

    [Header("Decor")]
    [SerializeField] private GameObject[] onWaterObjects;
    [SerializeField] private GameObject[] beneathWaterObjects;
    [SerializeField] private float waterObjectDensity = 0.2f;
    [SerializeField] private GameObject[] landObjects;
    [SerializeField] private float landDensity = 0.6f;

    [Header("Seed")]
    [SerializeField] private bool useSeed = false;
    [SerializeField] private int seed;

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        int seed = useSeed ? this.seed : Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(seed);

        scaleVector = new Vector3(scaleOfObjects, scaleOfObjects, scaleOfObjects);

        MapValuesDict mapValuesDict = MapValuesDict.Instance;
        mapValuesDict.ObjectScaleVector = scaleVector;
        mapValuesDict.WorldMapSize = WorldMapSize;

        // is there already a world?
        if (map != null || (map = GameObject.Find("World")) != null)
            Destroy(map);

        map = new GameObject("World")
        {
            layer = 8
        };
        MapValuesDict.Instance.Map = map;
        objects = new GameObject("Objects").transform;
        objects.parent = map.transform;

        // First we generate the base noise Array.
        // This is done by using simplex noise
        Fast2DArray<float> noiseArray = GenerateBaseNoise(out float minNoiseValue, out float maxNoiseValue);

        // We then go through the noise Array and look for every y value which is lower then the maxWaterLevel.
        // Each of these points are then combined to form a lake. This method gets us the waterSpots and also
        // which verts we already used for these water spots.
        GenerateWaterSpots(out List<List<Vector2Int>> waterSpots, out Fast2DArray<bool> alreadyPlacedSomething, noiseArray, minNoiseValue);

        // Generate the base verticies for the map. This is based on the noiseArray. To make it look
        // low poly, we also move the verticies slightly arround.
        Fast2DArray<Vector3> verticies = GenerateBaseVerts(noiseArray, waterSpots, out float minHeight, out float maxHeight);

        // Generate the planes for the water spots. This is a simply a plane with the waterMaterial attached to it
        List<Transform> waterPlanes = GenerateWaterPlanes(waterSpots, verticies);

        // Generate the baseMesh based on the verticies and the min and max height and attach a collider onto it
        GenerateBaseMesh(verticies, minHeight, maxHeight);
        map.AddComponent<MeshCollider>();

        // Generate the Starting Toori for the player town in the middle of the map
        PlacePlayerStartingToori(verticies, alreadyPlacedSomething, minHeight, maxHeight);

        // Generate some objects and resources on top of the map
        GenerateWaterDecor(verticies, waterSpots, waterPlanes, minHeight, maxHeight);
        GenerateRessourceFields(verticies, alreadyPlacedSomething, minHeight, maxHeight);
        GenerateLandDecor(verticies, alreadyPlacedSomething, minHeight, maxHeight);

        // Generate the nav mesh
        GenerateNavMesh(waterPlanes);

        // move the map to the offset position
        map.transform.position = transform.position + mapOffset;
    }

}
