using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Holds multiple Resources of a single ResourceType
/// </summary>
public class ResourceField : MonoBehaviour
{
    [SerializeField] private List<Resource> resources;
    [SerializeField] private ResourceType type;

    public ResourceType Type { get => type; private set => type = value; }
    public List<Resource> Resources { get => resources; private set => resources = value; }
    public Resource RandomResource => Resources[Random.Range(0, Resources.Count - 1)];
    public int TotalRemaining => Resources.Sum(x => x.AmountLeft);

    /// <summary>
    /// Removes a resource from the resource field. If none are left then the resourcefield deletes itself
    /// </summary>
    public void Remove(Resource resource)
    {
        Resources.Remove(resource);
        if (resources.Count == 0)
        {
            ResourceManager.Instance.DeRegister(this);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inits the ResourceField on the given resources. Parents all resources to this field
    /// </summary>
    public void Set(List<Resource> resources)
    {
        Vector3 positions = new Vector3();
        for (int i = 0; i < resources.Count; i++)
            positions += resources[i].transform.position;

        transform.position = positions / resources.Count;

        for (int i = 0; i < resources.Count; i++)
            resources[i].transform.parent = transform;

        Resources = resources;
        Type = resources[0].ResourceType;
        ResourceManager.Instance.Register(this);
    }

}
