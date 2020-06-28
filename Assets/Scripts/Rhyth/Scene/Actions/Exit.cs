namespace Modularity.Scene
{
    public class Exit : Action
    {
        bool requestSkip = false;
        private Viewer viewer;

        public override void RequestSkip()
        {
            requestSkip = true;
        }

        public override void Reset()
        {
            requestSkip = false;
        }

        public override void Start(Viewer viewer)
        {
            this.viewer = viewer;
            requestSkip = false;
        }

        public override bool Update()
        {
            if (requestSkip)
                viewer.RequestExit();
            return requestSkip;
        }
    }
}
