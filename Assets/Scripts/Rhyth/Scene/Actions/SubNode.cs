using System.Collections.Generic;


namespace Modularity.Scene
{
    public class SubNode : Action
    {
        private Action[] actions;
        private int at;
        private Viewer viewer;

        public SubNode(Action[] actions)
        {
            this.actions = actions;
            at = 0;
        }

        public override void RequestSkip()
        {
            actions[at].RequestSkip();
        }

        public override void Start(Viewer viewer)
        {
            if (at < 0 || at > actions.Length - 1)
                this.viewer = null;

            actions[at].Start(viewer);
            this.viewer = viewer;
        }

        public override bool Update()
        {
            if (actions[at].Update())
            {
                if (++at == actions.Length)
                    return true;
                actions[at].Start(viewer);
            }
            return false;
        }

        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            lineStart++;
            int nextBracket = Parser.GetCorrespondingBracket(input, lineStart);

            List<Action> actions = new List<Action>();
            while (nextBracket != lineStart)
                actions.Add(Parser.GetAction(input, lineStart, stringStart, out lineStart, out stringStart));

            parsedToLine = nextBracket + 1;
            parsedToStringIndex = 0;

            return new SubNode(actions.ToArray());
        }

        public override void Reset()
        {
            at = 0;
            for (int i = 0; i < actions.Length; i++)
                actions[i].Reset();
        }
    }
}