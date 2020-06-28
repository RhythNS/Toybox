public class TooriSelectable : SimpleSelectable
{
    protected override void InnerSelect() => TooriPanel.Instance.Open(GetComponent<Toori>());
}
