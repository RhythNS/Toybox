using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    private void PlacePlayerStartingToori(Fast2DArray<Vector3> verticies, Fast2DArray<bool> alreadyPlacedSomething, float minHeight, float maxHeight)
    {
        Town playerTown = TownDict.Instance.Towns[0]; // Might need to rethink this if there are multiple towns spawning

        Vector2Int startingPos = new Vector2Int(alreadyPlacedSomething.XSize / 2, alreadyPlacedSomething.YSize / 2);

        int incrementer = -1;
        while (true)
        {
            incrementer++;

            if (incrementer > alreadyPlacedSomething.XSize / 2 && incrementer > alreadyPlacedSomething.YSize / 2)
                break;

            for (int x = -incrementer; x < incrementer + 1; x++)
            {
                for (int y = -incrementer; y < incrementer + 1; y++)
                {
                    // if it is not the most outer values continue
                    if (x != -incrementer && x != incrementer && y != -incrementer && y != incrementer)
                        continue;

                    Vector2Int newPos = startingPos + new Vector2Int(x, y);
                    // If there is already something placed or it is out of bounds continue
                    if (!alreadyPlacedSomething.InBounds(newPos) || alreadyPlacedSomething.Get(newPos) != false)
                        continue;

                    RaycastHit[] hits = GetHitsFromPoint(verticies.Get(newPos), minHeight, maxHeight);

                    for (int i = 0; i < hits.Length; i++)
                    {
                        // Collider is on the map, so we can place the toori there
                        if (hits[i].collider.gameObject == map)
                        {
                            // Set the vicinity to already placed something
                            for (int x2 = newPos.x - 1; x2 < newPos.x + 2; x2++)
                                for (int y2 = newPos.y - 1; y2 < newPos.y + 2; y2++)
                                    alreadyPlacedSomething.Set(true, x2, y2);

                            playerTown.PlaceToori(hits[i].point);
                            return;
                        }
                    }

                    // Could not find the map
                    Debug.LogError("Tried to place Toori on the map but the rayscast could not hit the map.");
                    return;

                } // end iteration over map y
            } // end iteration over map x
        } // end while(true)

        // Somehow iterated over the entire map and no suitable place was found.
        Debug.LogError("Could not place Toori. Could not find a place to put it to.");
    }
}
