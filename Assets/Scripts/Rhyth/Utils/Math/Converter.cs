using UnityEngine;

public class Converter : MonoBehaviour
{
    [SerializeField] private int pixelsPerUnit = 16;
    public int PixelsPerUnit { get => pixelsPerUnit; }

    [SerializeField] private int tileWidth = 16;
    public int TileWidth { get => tileWidth; }

    [SerializeField] private float cellSize = 1f;
    public float CellSize { get => cellSize; }

    public static Converter Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }
    public Vector2 FloorWorldPosition(Vector2 position)
    {
        position.x -= position.x / cellSize;
        position.y -= position.y / cellSize;
        return position;
    }

    public Vector2 TiledToWorld(Vector2 position) => position * cellSize;

    public Vector2 WorldToTiled(Vector2 position) => position / cellSize;

    //public Vector2 TiledToWorld(Vector2 position) => position / Instance.pixelsPerUnit * Instance.tileWidth;
    //public Vector2 WorldToTiled(Vector2 position) => position / Instance.tileWidth * Instance.pixelsPerUnit;

    public static float GetScreenHeight(Camera camera)
        => camera.orthographicSize;

    public static float GetScreenWidth(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        return screenAspect * camera.orthographicSize * 2;
    }

}
