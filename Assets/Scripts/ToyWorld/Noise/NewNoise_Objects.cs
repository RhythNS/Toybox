using System.Collections.Generic;
using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    private void GenerateWaterDecor(Fast2DArray<Vector3> verticies, List<List<Vector2Int>> waterSpots, List<Transform> waterPlanes, float minHeight, float maxHeight)
    {
        for (int i = 0; i < waterSpots.Count; i++)
        {
            List<Vector2Int> waterSpot = waterSpots[i];
            //Transform waterPlane = waterPlanes[i];
            Transform waterPlane = waterPlanes[0];

            for (int j = 0; j < waterSpot.Count; j++)
            {
                if (Random.value > waterObjectDensity)
                    continue;

                RaycastHit[] hits = GetHitsFromRandomPoint(verticies.Get(waterSpot[j]), minHeight, maxHeight);

                bool settingOnWater = Random.value > 0.5f;

                for (int k = 0; k < hits.Length; k++)
                {
                    // hit the map and we want to do not set on top of water
                    if (hits[k].collider.gameObject == map.gameObject && settingOnWater == false)
                    {
                        PlaceObject(Instantiate(ArrayUtil<GameObject>.RandomElement(beneathWaterObjects)), hits[k].point);
                    }
                    // hit the waterPlane and we want to set on top of water
                    else if (hits[k].collider.gameObject == waterPlane.gameObject && settingOnWater == true)
                    {
                        PlaceObject(Instantiate(ArrayUtil<GameObject>.RandomElement(onWaterObjects)), hits[k].point);
                    }
                }
            }
        }
    }

    private void GenerateLandDecor(Fast2DArray<Vector3> verticies, Fast2DArray<bool> alreadyPlaced, float minHeight, float maxHeight)
    {
        for (int x = 0; x < verticies.XSize; x++)
        {
            for (int y = 0; y < verticies.YSize; y++)
            {
                // if there already is something place or randomvalue is lower then landDenstiy, then skip it
                if (alreadyPlaced.Get(x, y) == true || Random.value > landDensity)
                    continue;

                RaycastHit[] hits = GetHitsFromRandomPoint(verticies.Get(x, y), minHeight, maxHeight);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.gameObject == map)
                        PlaceObject(Instantiate(ArrayUtil<GameObject>.RandomElement(landObjects)), hits[i].point);
                }
            }
        }
    }

    private void GenerateRessourceFields(Fast2DArray<Vector3> verts, Fast2DArray<bool> alreadyPlaced, float minHeight, float maxHeight)
    {
        Transform resourceFieldsParent = new GameObject("Resource Fields").transform;
        resourceFieldsParent.parent = map.transform;

        // Set all fast noise values from the fields of this class
        FastNoise noiceGenerator = new FastNoise(seed);
        noiceGenerator.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
        noiceGenerator.SetFrequency(resourceFrequency);
        noiceGenerator.SetFractalOctaves(resourceOctaves);
        noiceGenerator.SetFractalLacunarity(resourceLacunarity);
        noiceGenerator.SetFractalGain(resourceGain);

        float biomeMinValue = float.MaxValue, biomeMaxValue = float.MinValue;

        Fast2DArray<float> resourceNoise = new Fast2DArray<float>(xSize, ySize);
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                float noise = noiceGenerator.GetSimplexFractal(x, y);
                resourceNoise.Set(noise, x, y);

                // normalize values
                if (noise < biomeMinValue)
                    biomeMinValue = noise;
                if (noise > biomeMaxValue)
                    biomeMaxValue = noise;
            }
        } // end noise gen

        // normalize values
        float maxMinusMin = biomeMaxValue - biomeMinValue;
        Vector2 minVector = new Vector2(biomeMinValue, biomeMinValue);
        Vector2 forestRange = (this.forestRange * maxMinusMin) + minVector;
        Vector2 bambooRange = (this.bambooRange * maxMinusMin) + minVector;
        Vector2 stoneRange = (this.stoneRange * maxMinusMin) + minVector;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (alreadyPlaced.Get(x, y) == true)
                    continue;

                float noise = resourceNoise.Get(x, y);
                List<Vector2Int> positions;
                Vector2 range;
                Resource[] resourcePrefabs;

                // Go through all ranges and assign a resource to the above created values
                if (MathUtil.InRangeInclusive(forestRange.x, forestRange.y, noise))
                {
                    resourcePrefabs = forestPrefabs;
                    range = forestRange;
                }
                else if (MathUtil.InRangeInclusive(bambooRange.x, bambooRange.y, noise))
                {
                    resourcePrefabs = bambooPrefabs;
                    range = bambooRange;
                }
                else if (MathUtil.InRangeInclusive(stoneRange.x, stoneRange.y, noise))
                {
                    resourcePrefabs = stonePrefabs;
                    range = stoneRange;
                }
                else
                    continue;

                // get all positions from the range
                positions = AllNeighbours(resourceNoise, new Vector2Int(x, y), range.x, range.y, alreadyPlaced);

                // if the amount of positions is less then the minimum or the random chance a field should not spawn, simply continue the loop
                if (positions.Count < minAmountInResourceField || Random.value > resourceSpawnChance)
                {
                    positions.ForEach(t => alreadyPlaced.Set(false, t));
                    continue;
                }

                // Create the resourceField
                Transform resourceFieldTransform = new GameObject().transform;
                resourceFieldTransform.parent = resourceFieldsParent;
                ResourceField resourceField = resourceFieldTransform.gameObject.AddComponent<ResourceField>();

                // Create the resources
                List<Resource> resources = new List<Resource>();
                for (int i = 0; i < positions.Count; i++)
                {
                    RaycastHit[] hits = GetHitsFromRandomPoint(verts.Get(positions[i]), minHeight, maxHeight);

                    for (int j = 0; j < hits.Length; j++)
                    {
                        if (hits[j].collider.gameObject == map)
                        {
                            Resource resource = Instantiate(ArrayUtil<Resource>.RandomElement(resourcePrefabs));
                            resource.ParentField = resourceField;
                            resources.Add(resource);

                            resource.transform.localScale = scaleVector;
                            resource.transform.position = hits[j].point;
                            resource.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                            break;
                        }
                    } // end iteration hits
                } // end iteration positions

                // if something went wrong simply delete the resourcefield again
                if (resources.Count == 0)
                {
                    positions.ForEach(t => alreadyPlaced.Set(false, t));
                    Destroy(resourceField.gameObject);
                    continue;
                }

                // assign the resources to the field
                resourceField.Set(resources);
            }
        } // end iteration through noise

    } // end generate resourceFields

    /// <summary>
    /// Returns all hits on a random position (-halfStepSize -> halfStepSize) around the vert within the min and maxHeight
    /// </summary>
    private RaycastHit[] GetHitsFromRandomPoint(Vector3 vert, float minHeight, float maxHeight)
    {
        float halfSize = stepSize / 2;
        Vector3 location = vert + new Vector3(Random.Range(-halfSize, halfSize), 0, Random.Range(-halfSize, halfSize));

        return GetHitsFromPoint(location, minHeight, maxHeight);
    }

    /// <summary>
    /// Returns all hits from given point and minHeight/ MaxHeight
    /// </summary>
    private RaycastHit[] GetHitsFromPoint(Vector3 vert, float minHeight, float maxHeight)
    {
        Vector3 rayCastFrom = vert;
        rayCastFrom.y = maxHeight;

        Vector3 rayCastTo = vert;
        rayCastTo.y = minHeight;

        return Physics.RaycastAll(rayCastFrom, rayCastTo - rayCastFrom);
    }

    /// <summary>
    /// Places an object with a random y rotation onto the specified place and assignes the object transform
    /// as a parent to the placed object.
    /// </summary>
    private void PlaceObject(GameObject instantiated, Vector3 toPlace)
    {
        instantiated.transform.parent = objects;
        instantiated.transform.tag = "decor";
        instantiated.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        instantiated.transform.localScale = scaleVector;
        instantiated.transform.position = toPlace;
    }
}
