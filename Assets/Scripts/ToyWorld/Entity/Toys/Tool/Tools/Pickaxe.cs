using UnityEngine;

[CreateAssetMenu(fileName = "New Pickaxe", menuName = "Toybox/Tools/Pickaxe")]
public class Pickaxe : Tool
{
    public override ToolType ToolType => ToolType.Pickaxe;
}
