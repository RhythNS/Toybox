using UnityEngine;

/// <summary>
/// Dict for all things related to the map
/// </summary>
public class MapValuesDict : MonoBehaviour
{
    public static MapValuesDict Instance { get; private set; }

    public Vector3 ObjectScaleVector { get; set; }
    public Vector2 WorldMapSize { get; set; }

    public GameObject Map { get; set; }

    public Transform ItemsInWorld { get; set; }

    private void Awake()
    {
        Instance = this;
        ItemsInWorld = new GameObject("Items in World").transform;
    }

}
