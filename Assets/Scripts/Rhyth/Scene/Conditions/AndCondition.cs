namespace Modularity.Scene
{
    public class AndCondition : Condition
    {
        private Condition condition1, condition2;

        public AndCondition(Condition condition1, Condition condition2)
        {
            this.condition1 = condition1;
            this.condition2 = condition2;
        }

        public override bool IsFullfilled(Viewer viewer)
            => condition1.IsFullfilled(viewer) && condition2.IsFullfilled(viewer);
    }
}
