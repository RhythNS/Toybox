namespace Modularity.Scene
{
    public class AbsoluteValue : Value
    {
        private int value;

        public AbsoluteValue(int value)
        {
            this.value = value;
        }

        public override int GetValue(Viewer viewer) => value;
    }
}