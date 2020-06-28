using System.Collections.Generic;

namespace Modularity.Scene
{
    [System.Serializable]
    public class Scene
    {
        private List<Node> nodes;
        private int atNode;
        public int AtNode
        {
            get => atNode;
            set
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].NodeNumber == value)
                    {
                        atNode = i;
                        nodes[atNode].ResetNode();
                        return;
                    }
                }
                throw new System.Exception("Node Number " + value + " not found!");
            }
        }

        public int Size { get => nodes.Count; }

        public Scene(List<Node> nodes)
        {
            this.nodes = nodes;
            atNode = 0;
        }

        public Action GetNextAction()
        {
            return nodes[AtNode].GetNextAction();
        }

    }
}