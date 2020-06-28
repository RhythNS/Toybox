using UnityEngine;

/// <summary>
/// Access to all Renderes on a Toy
/// </summary>
public class CreatorHelper : MonoBehaviour
{
    public SkinnedMeshRenderer accessory00, accessory01, accessory02;
    public MultiSkinnedRenderer[] costumes;
    public SkinnedMeshRenderer[] eyes;
    public MultiSkinnedRenderer[] hairs;
    public SkinnedMeshRenderer[] faces;

    [System.Serializable]
    public class MultiSkinnedRenderer
    {
        public SkinnedMeshRenderer[] renderers;
    }

}
