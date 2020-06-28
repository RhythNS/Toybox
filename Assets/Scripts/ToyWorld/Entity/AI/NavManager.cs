using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class for building and updating the NavMesh
/// </summary>
public class NavManager : MonoBehaviour
{
    public static NavManager Instance { get; private set; }

    private NavMeshData navMeshData;
    private List<NavMeshBuildSource> sources;

    private Bounds? bounds;

    private void Awake()
    {
        Instance = this;
        sources = new List<NavMeshBuildSource>();
        navMeshData = new NavMeshData();
        NavMesh.AddNavMeshData(navMeshData);
    }

    /// <summary>
    /// Updates the NavMesh. Should be called after adding or removing sources
    /// </summary>
    public void UpdateMesh()
    {
        NavMeshBuildSettings defualtBuildSettings = NavMesh.GetSettingsByID(0);

        if (bounds == null)
            bounds = MapValuesDict.Instance.Map.GetComponent<Renderer>().bounds;

        NavMeshBuilder.UpdateNavMeshData(navMeshData, defualtBuildSettings, sources, bounds.Value);
    }

    // Add a transform to the NavMeshSources. It is assumed that a MeshFilter is on the transform
    public void AddTransform(Transform toAdd, int area)
    {
        NavMeshBuildSource buildSource = new NavMeshBuildSource
        {
            shape = NavMeshBuildSourceShape.Mesh,
            sourceObject = toAdd.GetComponent<MeshFilter>().mesh,
            transform = toAdd.localToWorldMatrix,
            area = area
        };
        sources.Add(buildSource);
    }

    /// <summary>
    /// Adds sources to internal sources
    /// </summary>
    public void AddSources(List<NavMeshBuildSource> sources)
    {
        this.sources.AddRange(sources);
    }

    /// <summary>
    /// Add source to internal sources
    /// </summary>
    public void AddSource(NavMeshBuildSource source)
    {
        sources.Add(source);
    }

    /// <summary>
    /// Remove sources from internal sources
    /// </summary>
    public void RemoveSources(List<NavMeshBuildSource> sources)
    {
        for (int i = 0; i < sources.Count; i++)
            this.sources.Remove(sources[i]);
    }

    /// <summary>
    /// Remove source from internal sources
    /// </summary>
    public void RemoveSource(NavMeshBuildSource source)
    {
        sources.Remove(source);
    }

}
