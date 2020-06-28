namespace Modularity.Scene
{
    public abstract class ActionAdapter : Action
    {
        public override void RequestSkip() { }

        public override void Reset() { }

        public override void Start(Viewer viewer) { }

        public override bool Update() => true;
    }
}