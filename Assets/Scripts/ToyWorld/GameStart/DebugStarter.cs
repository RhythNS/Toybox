using UnityEngine;

/// <summary>
/// Debug starts the game
/// </summary>
public class DebugStarter : MonoBehaviour
{
    [SerializeField] Town town;
    [SerializeField] CollectionPoint[] collectionPoints;
    [SerializeField] Toy[] toys;

    private void Start()
    {
        TownDict.Instance.Add(town);
        ResourceManager.Instance.Init(new Vector3(0, 0), new Vector2(80, 80), 20, 20);

        Camera.main.transform.GetComponent<CameraController>().LevelRect = new Rect(-80, -80, 100, 100);

        UIManager.Instance.Town = town;
        for (int i = 0; i < collectionPoints.Length; i++)
        {
            collectionPoints[i].Town = town;
            town.CollectionPoints.Add(collectionPoints[i]);
        }
        for (int i = 0; i < toys.Length; i++)
        {
            toys[i].Town = town;
        }
    }
}
