using System.Collections.Generic;

namespace Modularity.Scene
{
    public class Decision : Action
    {
        private string[] strings;
        private Action[] actions;
        private Action selectedAction;
        private Viewer viewer;

        public Decision(string[] strings, Action[] actions)
        {
            this.strings = strings;
            this.actions = actions;
        }

        public override void RequestSkip()
        {
            if (selectedAction != null)
                selectedAction.RequestSkip();
        }

        public override void Start(Viewer viewer)
        {
            this.viewer = viewer;
            viewer.ShowDecision(strings);
        }

        public override bool Update()
        {
            if (selectedAction != null)
                return selectedAction.Update();

            if (viewer.DecisionMade >= 0)
            {
                selectedAction = actions[viewer.DecisionMade];
                viewer.DecisionMade = -2;
                selectedAction.Start(viewer);
            }
            return false;
        }

        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            /*
             * decision §decision 1§ §decision 2§ §decision 3§
               {
               1 = goto node 3
               2 = say ui §Nevermind§
               3 = say ui §Something else§
               }
            */
            int atLine = lineStart;
            List<string> stringList = new List<string>();
            while (atLine == lineStart)
                stringList.Add(Parser.ParseParagraph(input, lineStart, stringStart, out lineStart, out stringStart));
            int closingBracket = Parser.GetCorrespondingBracket(input, lineStart + 1);
            if (closingBracket - lineStart - 1 != stringList.Count)
                throw new ParserException(lineStart, "Decision has too many or too less arguments!");

            List<Action> actions = new List<Action>();
            for (int i = lineStart + 1; i < closingBracket; i++)
            {
                actions.Add(Parser.GetAction(input, i, 4, out _, out _));
            }

            parsedToLine = closingBracket + 1;
            parsedToStringIndex = 0;

            return new Decision(stringList.ToArray(), actions.ToArray());
        }

        public override void Reset()
        {
            if (selectedAction != null)
            {
                selectedAction.Reset();
                selectedAction = null;
            }
        }
    }
}