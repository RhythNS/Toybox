using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumeratorUtil;

namespace Modularity.Scene
{
    public partial class Enter : Action
    {
        public Actor prefabActor;
        private Actor leavingActor;
        public EnterDirection enterDirection;
        public bool entering;

        private Movement[] movements;
        private Viewer viewer;
        private Coroutine coroutine;
        private bool finished;
        private readonly float TIME_TO_ENTER = 2;

        public Enter(bool entering, Actor prefabActor, EnterDirection enterDirection)
        {
            this.entering = entering;
            this.prefabActor = prefabActor;
            this.enterDirection = enterDirection;
        }

        public override void Start(Viewer viewer)
        {
            this.viewer = viewer;

            movements = entering ? GetEnter(Camera.main) : GetLeave(Camera.main);

            if (enterDirection == EnterDirection.Instant)
                RequestSkip();
            else
                coroutine = viewer.StartCoroutine(GetEnumerator(movements));
        }

        public override bool Update() => finished;

        public override void Reset()
        {
            if (coroutine != null)
            {
                viewer.StopCoroutine(coroutine);
                coroutine = null;
            }
            finished = false;
        }

        public override void RequestSkip()
        {
            if (coroutine != null)
                viewer.StopCoroutine(coroutine);
            foreach (Movement movement in movements)
                movement.toMove.position = movement.to;

            if (!entering)
                Object.Destroy(leavingActor.gameObject);

            finished = true;
        }

        private Movement[] GetEnter(Camera camera)
        {
            Actor actorObject = Object.Instantiate(prefabActor, camera.transform) as Actor;
            viewer.ActiveActors.Add(actorObject);

            float height = Converter.GetScreenHeight(camera);
            float width = Converter.GetScreenWidth(camera);
            Vector2 cameraPosition = camera.transform.position;

            Vector2 fromNewActor = cameraPosition;
            switch (enterDirection)
            {
                case EnterDirection.Left:
                    fromNewActor -= new Vector2((actorObject.Size.x + width) / 2, 0);
                    break;
                case EnterDirection.Right:
                    fromNewActor += new Vector2((actorObject.Size.x + width) / 2, 0);
                    break;
            }

            Movement[] movements = GetNewMovements(cameraPosition, width, height);

            movements[movements.Length - 1].from = fromNewActor;
            return movements;
        }

        private Movement[] GetLeave(Camera camera)
        {
            float height = Converter.GetScreenHeight(camera);
            float width = Converter.GetScreenWidth(camera);
            Vector2 cameraPosition = camera.transform.position;

            leavingActor = null;
            for (int i = 0; i < viewer.ActiveActors.Count; i++)
            {
                if (viewer.ActiveActors[i].ActorInfo.Name == prefabActor.ActorInfo.Name)
                {
                    leavingActor = viewer.ActiveActors[i];
                    viewer.ActiveActors.RemoveAt(i);
                    break;
                }
            }
            if (leavingActor == null)
                throw new System.Exception("Actor " + prefabActor.ActorInfo.Name + " not found on Stage!");

            Vector2 toLeavingActor = cameraPosition;
            switch (enterDirection)
            {
                case EnterDirection.Left:
                    toLeavingActor += new Vector2((leavingActor.Size.x + width) / 2, 0);
                    break;
                case EnterDirection.Right:
                    toLeavingActor -= new Vector2((leavingActor.Size.x + width) / 2, 0);
                    break;
            }

            List<Movement> movements = new List<Movement>();
            movements.AddRange(GetNewMovements(cameraPosition, width, height));
            movements.Add(new Movement
            {
                from = leavingActor.transform.position,
                to = toLeavingActor,
                toMove = leavingActor.transform
            });
            return movements.ToArray();
        }

        private Movement[] GetNewMovements(Vector2 cameraPosition, float width, float height)
        {
            Movement[] movements = new Movement[viewer.ActiveActors.Count];
            for (int i = 0; i < movements.Length; i++)
            {
                Vector2 toPos = cameraPosition - new Vector2(-width / 2 + (width / (movements.Length + 1)) * (i + 1), 0);
                movements[i] = new Movement
                {
                    from = viewer.ActiveActors[i].transform.position,
                    to = toPos,
                    toMove = viewer.ActiveActors[i].transform
                };
            }
            return movements;
        }

        private IEnumerator GetEnumerator(params Movement[] movements)
        {
            finished = false;
            yield return EnumeratorUtil.MoveToMultiple(TIME_TO_ENTER, movements);
            if (!entering)
                Object.Destroy(leavingActor.gameObject);
            finished = true;
        }
    }
}