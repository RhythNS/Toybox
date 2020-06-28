using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all ResourceFields and
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private float xOffset, yOffset, width, height;
    private float singleX, singleY;

    private Fast2DArray<List<ResourceField>> resourceFields;
    private List<List<ResourceField>> tempFields;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one Resource Managers active in this scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void Init(Vector3 mapOffset, Vector2 mapSize, int xSize, int ySize)
    {
        xOffset = mapOffset.x;
        yOffset = mapOffset.z;
        width = mapSize.x;
        height = mapSize.y;
        singleX = width / xSize;
        singleY = height / ySize;

        tempFields = new List<List<ResourceField>>();

        // iterate through every element and init the list
        resourceFields = new Fast2DArray<List<ResourceField>>(xSize, ySize);
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < xSize; y++)
            {
                resourceFields.Set(new List<ResourceField>(), x, y);
            }
        }
    }

    /// <summary>
    /// Converts a world position to an array local position.
    /// </summary>
    /// <returns>Wheter it successded or not</returns>
    private bool WorldToLocal(Vector3 world, out Vector2Int local)
    {
        world.x -= xOffset;
        world.z -= yOffset;
        if (world.x > width || world.x < 0 || world.z >= height || world.z < 0)
        {
            local = default;
            return false;
        }
        local = new Vector2Int(Mathf.FloorToInt(world.x / singleX), Mathf.FloorToInt(world.z / singleY));
        return true;
    }

    /// <summary>
    /// Adds a ResourceField to the Manager.
    /// </summary>
    /// <param name="field">The field to be added.</param>
    public void Register(ResourceField field)
    {
        if (WorldToLocal(field.transform.position, out Vector2Int local) == false || resourceFields.InBounds(local) == false)
        {
            Debug.LogError("Could not add field at position " + field.transform.position + "! OutOfBounds!");
            return;
        }
        resourceFields.Get(local).Add(field);
    }

    /// <summary>
    /// Removes a ResourceField from the Manager.
    /// </summary>
    /// <param name="field">The field to be removed</param>
    public void DeRegister(ResourceField field)
    {
        foreach (List<ResourceField> resourceField in resourceFields)
        {
            for (int i = 0; i < resourceField.Count; i++)
            {
                if (resourceField[i] == field)
                {
                    resourceField.RemoveAt(i);
                    return;
                }
            }
        }
        Debug.LogError("Could not find " + field + " in map");
    }

    /// <summary>
    /// Gets the clostest ResourceField.
    /// </summary>
    /// <param name="type">The type of the resource field.</param>
    /// <param name="position">The position of the person who wants a resource fields.</param>
    /// <param name="resourceField">The best resource field.</param>
    /// <returns>Wheter it successeded or not.</returns>
    public bool TryGetNextResourceField(ResourceType type, Vector3 position, out ResourceField resourceField)
    {
        if (WorldToLocal(position, out Vector2Int start) == false)
        {
            Debug.LogError("Character was out of bounds!" + position);
            resourceField = default;
            return false;
        }

        int incrementer = -1;
        bool running = true;
        while (running)
        {
            tempFields.Clear();
            running = false;
            incrementer++;

            // iterate through the most outer potential fields
            for (int x = -incrementer; x < incrementer + 1; x++)
            {
                for (int y = -incrementer; y < incrementer + 1; y++)
                {
                    if (x == -incrementer || x == incrementer || y == -incrementer || y == incrementer)
                    {
                        // If they are in bounds add them to the temp fields
                        Vector2Int newPos = start + new Vector2Int(x, y);
                        if (resourceFields.InBounds(newPos))
                        {
                            running = true;
                            tempFields.Add(resourceFields.Get(x, y));
                        }
                    }
                }
            }

            // try and find the best field, if any are present
            ResourceField bestField = null;
            float bestDistance = float.MaxValue;
            foreach (List<ResourceField> potentialFields in tempFields)
            {
                for (int i = 0; i < potentialFields.Count; i++)
                {
                    if (potentialFields[i].Type == type)
                    {
                        // calculate new distance
                        float newDistance = (potentialFields[i].transform.position - position).sqrMagnitude;
                        if (newDistance < bestDistance)
                        {
                            bestDistance = newDistance;
                            bestField = potentialFields[i];
                        }
                    }
                }
            }

            // If a field was assigned then it is the clostest field
            if (bestField != null)
            {
                resourceField = bestField;
                return true;
            }

            // If the incrementer is larger then half the xSize and half the ySize then stop the search
            if (incrementer > resourceFields.XSize / 2 && incrementer > resourceFields.YSize / 2)
                break;
        }

        // nothing found return an error
        Debug.LogError("Could not find resourcefield. Everything is exhausted or were none registered?");
        resourceField = default;
        return false;
    }
}
