using UnityEngine;

/// <summary>
/// Inits all Scripts and starts the game
/// </summary>
public class Starter : MonoBehaviour
{
    private void Start()
    {
        NewNoise worldGen = NewNoise.Instance;
        Town playerTown = new GameObject("Player Town").AddComponent<Town>();
        TownDict.Instance.Add(playerTown);

        // Init ResourceManager
        Vector3 mapOffset = worldGen.MapOffset;
        Vector2 mapSize = worldGen.WorldMapSize;
        Vector2Int simMapSize = worldGen.SimulatedMapSize;
        ResourceManager.Instance.Init(mapOffset, mapSize, simMapSize.x / 10, simMapSize.y / 10);

        // Generate the world
        worldGen.Generate();

        // Set the bounds of the camera
        Camera camera = Camera.main;
        camera.transform.GetComponent<CameraController>().LevelRect = new Rect(mapOffset.x, mapOffset.z, mapSize.x, mapSize.y);
        Vector3 cameraPos = camera.transform.position;
        cameraPos.x = playerTown.Toori.transform.position.x;
        cameraPos.z = playerTown.Toori.transform.position.z;
        camera.transform.position = cameraPos;

        // Init the UI
        UIManager.Instance.Town = playerTown;
    }
}
