using System.Collections.Generic;
using UnityEngine;

namespace Modularity.Scene
{
    public class Viewer : MonoBehaviour
    {
        public RectTransform rectTransform;
        public TextDisplayer Text { get; private set; }
        private List<DecisionMaker> decisionMakers;

        public List<Actor> ActiveActors { get; private set; }

        public GameObject decisionPickerPrefab;

        public Scene CurrentScene { private set; get; }
        private Action currentAction;

        public int DecisionMade { get; set; }
        public int JumpToNode { get; set; }
        private bool exitRequest;
        public void RequestExit() => exitRequest = true;

        private IPausable pausable;

        public static Viewer Instance { get; private set; }

        void Start()
        {
            Instance = this;
            ActiveActors = new List<Actor>();
            decisionMakers = new List<DecisionMaker>();
            Text = GetComponentInChildren<TextDisplayer>();
            Text.viewer = this;
            rectTransform = (RectTransform)transform;
            Hide();
        }

        public void DisplayScene(Scene scene, IPausable pausable)
        {
            if (ActorDirectory.Instance == null)
                throw new System.Exception("ActorDirectory is not active in this scene. Maybe you forgot to add it into globals?");
            if (gameObject.activeInHierarchy)
            {
                Debug.LogWarning("A new Scene was loaded when an old one was still playing. Was this desired behaviour?");
                this.pausable = null;
                Hide();
            }
            JumpToNode = -1;
            DecisionMade = -1;
            exitRequest = false;
            CurrentScene = scene;
            gameObject.SetActive(true);
            this.pausable = pausable;
            pausable.Pause();
            NextAction();
        }

        public void ShowDecision(params string[] options)
        {
            Rect textRect = Text.rectTransform.rect;
            for (int i = options.Length - 1; i >= 0; i--)
            {
                DecisionMaker decisionMaker = Instantiate(decisionPickerPrefab, transform).GetComponent<DecisionMaker>();
                decisionMaker.SetText(options[i]);
                Rect decisionRect = decisionMaker.RectTransform.rect;

                Vector3 localDecision = decisionMaker.transform.localPosition;
                localDecision.y = Text.transform.localPosition.y + textRect.height * Text.transform.lossyScale.y +
                    (decisionRect.height * decisionMaker.transform.lossyScale.y * (1 + (2 * (options.Length - 1 - i))));
                decisionMaker.transform.localPosition = localDecision;
                decisionMakers.Add(decisionMaker);
            }
        }

        public void Hide()
        {
            ActiveActors.ForEach(x => GameObject.Destroy(x.gameObject));
            ActiveActors.Clear();
            gameObject.SetActive(false);
            if (pausable != null)
                pausable.Resume();
        }

        public void RequestSkip() => currentAction.RequestSkip();

        private void Update()
        {
            if (currentAction != null && currentAction.Update())
            {
                NextAction();
                return;
            }
            if (decisionMakers.Count > 0)
            {
                // Node got decision
                if (DecisionMade == -2)
                {
                    for (int i = 0; i < decisionMakers.Count; i++)
                    {
                        Destroy(decisionMakers[i].gameObject);
                    }
                    decisionMakers.Clear();
                    DecisionMade = -1;
                    return;
                }

                // check for user input
                for (int i = 0; i < decisionMakers.Count; i++)
                {
                    if (decisionMakers[i].Selected)
                    {
                        DecisionMade = decisionMakers.Count - 1 - i;
                        return;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
                Text.OnClick();

        }

        public void NextAction()
        {
            if (JumpToNode != -1)
            {
                CurrentScene.AtNode = JumpToNode;
                JumpToNode = -1;
            }

            if (exitRequest || SetNextAction())
            {
                Hide();
            }
        }

        private bool SetNextAction()
        {
            currentAction = CurrentScene.GetNextAction();
            if (currentAction == null)
                return true;
            currentAction.Start(this);
            return false;
        }

    }
}
