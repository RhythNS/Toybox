using UnityEngine;

namespace Modularity.Scene
{
    public class ReadSceneTester : MonoBehaviour, IPausable
    {
        public string path;
        bool alreadyPlayed = false;

        private void Start()
        {
        }

        public void Pause()
        {
            print("pausing");
        }

        public void Resume()
        {
            print("resuming");
        }

        void Update()
        {
            if (!alreadyPlayed)
            {
                Viewer.Instance.DisplayScene(SceneGetter.GetScene(path), this);
                alreadyPlayed = true;
            }
        }
    }
}