public class CraftingBuilding : Building, IBuildingsSelectable
{
    public void Select()
    {
        CraftingPanel.Instance.Open();
    }
}
