using UnityEngine;

[CreateAssetMenu(fileName = "New Axe", menuName = "Toybox/Tools/Axe")]
public class Axe : Tool
{
    public override ToolType ToolType => ToolType.Axe;

}
