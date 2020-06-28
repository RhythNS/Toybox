using System.Collections.Generic;

namespace Modularity.Scene
{
    [System.Serializable]
    public class Node
    {
        public int NodeNumber { get; private set; }
        private int atNode;
        private List<Action> actions;

        public Node(int nodeNumber, List<Action> actions)
        {
            NodeNumber = nodeNumber;
            this.actions = actions;
            atNode = -1;
        }

        public void ResetNode()
        {
            for (int i = 0; i < atNode + 1; i++)
                actions[i].Reset();
            atNode = -1;
        }

        public Action GetNextAction()
        {
            return (++atNode >= actions.Count) ? null : actions[atNode];
        }
    }
}