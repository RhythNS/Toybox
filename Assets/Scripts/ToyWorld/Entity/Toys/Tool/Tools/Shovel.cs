using UnityEngine;

[CreateAssetMenu(fileName = "New Shovel", menuName = "Toybox/Tools/Shovel")]
public class Shovel : Tool
{
    public override ToolType ToolType => ToolType.Shovel;
}
