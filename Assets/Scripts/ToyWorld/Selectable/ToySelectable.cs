public class ToySelectable : SimpleSelectable
{
    private void Start()
    {
        // If the outlines where not found then check again for outlines.
        // This is because the Toy transforms are all not active when the toy spawns 
        if (outlines.Length == 0)
            ScanForOutlines();
    }

    protected override void InnerSelect() => ToyPanel.Instance.Open(GetComponent<Toy>());
}
