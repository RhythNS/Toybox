namespace Modularity.Scene
{
    public abstract class SimpleAction : Action
    {
        private bool requiresUserAction, userSkipped;

        public SimpleAction(bool requiresUserAction)
        {
            this.requiresUserAction = requiresUserAction;
        }

        public sealed override void RequestSkip()
        {
            userSkipped = true;
            InnerRequestSkip();
        }

        public virtual void InnerRequestSkip() { }

        public sealed override void Reset()
        {
            userSkipped = false;
            InnerReset();
        }

        public virtual void InnerReset() { }

        public sealed override bool Update() => (!requiresUserAction || (requiresUserAction && userSkipped)) && InnerUpdate();

        public virtual bool InnerUpdate() { return true; }

    }
}
