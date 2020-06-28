namespace Modularity.Scene
{
    public class And : Action
    {
        private Action action1, action2;
        private bool action1Finished, action2Finished;

        public And(Action action1, Action action2)
        {
            this.action1 = action1;
            action1Finished = false;
            this.action2 = action2;
            action2Finished = false;
        }

        public static Action Parse(Action action1, string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            Action action2 = Parser.GetAction(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);
            return new And(action1, action2);
        }

        public override void RequestSkip()
        {
            if (!action1Finished)
                action1.RequestSkip();
            if (!action2Finished)
                action2.RequestSkip();
        }

        public override void Reset()
        {
            action1Finished = false;
            action2Finished = false;
            action1.Reset();
            action2.Reset();
        }

        public override void Start(Viewer viewer)
        {
            action1.Start(viewer);
            action2.Start(viewer);
        }

        public override bool Update()
        {
            if (!action1Finished)
                action1Finished = action1.Update();
            if (!action2Finished)
                action2Finished = action2.Update();
            return action1Finished && action2Finished;
        }
    }
}