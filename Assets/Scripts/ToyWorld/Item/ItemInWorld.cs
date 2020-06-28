using UnityEngine;

/// <summary>
/// Instance of an Item in a world. This is so a ScriptedObject can be on a gameobject
/// </summary>
public class ItemInWorld : MonoBehaviour
{

    /// <summary>
    /// Helper method for instantiating an item
    /// </summary>
    /// <param name="item">The item to be instantiated from.</param>
    /// <param name="startingPos">The starting postion</param>
    /// <param name="rotation">The starting rotation</param>
    public static ItemInWorld CreateItem(Item item, Vector3 startingPos = default, Quaternion rotation = default)
    {
        ItemInWorld itemInWorld = Instantiate(item.ModelPrefab, startingPos, rotation).AddComponent<ItemInWorld>();
        itemInWorld.transform.parent = MapValuesDict.Instance.ItemsInWorld;
        itemInWorld.transform.localScale.Scale(MapValuesDict.Instance.ObjectScaleVector);
        itemInWorld.item = item;
        Rigidbody body = itemInWorld.gameObject.AddComponent<Rigidbody>();
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Vector3 randomDir = new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1).normalized;
        randomDir.y = 2;
        body.velocity = randomDir;
        return itemInWorld;
    }

    public Item item;
}
