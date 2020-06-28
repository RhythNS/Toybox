using System.Collections.Generic;
using UnityEngine;

public partial class NewNoise : MonoBehaviour
{
    /// <summary>
    /// Generates a mesh.
    /// </summary>
    /// <param name="toGenerateOnto">The Gameobject onto which all Components for the Mesh are put onto</param>
    /// <param name="verts">A list of verts</param>
    /// <param name="tris">A list of tris</param>
    /// <param name="colors">A list of colors</param>
    private Mesh GenerateMesh(GameObject toGenerateOnto, List<Vector3> verts, List<int> tris, Material material, List<Color> colors = null)
    {
        MeshRenderer renderer = toGenerateOnto.AddComponent<MeshRenderer>();
        MeshFilter filter = toGenerateOnto.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        renderer.material = material;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        if (colors != null)
            mesh.colors = colors.ToArray();

        // Recalculate the mesh
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
