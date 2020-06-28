using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Modularity.Scene
{
    public class Emote : Action
    {
        private Actor actor;
        private ActorInfo.Emotion emote;
        private bool finished;
        private Coroutine coroutine;
        private Viewer viewer;

        private static readonly float TIME_TO_EMOTE = 0.5f;

        public Emote(Actor actor, ActorInfo.Emotion emote)
        {
            this.actor = actor;
            this.emote = emote;
            finished = false;
        }

        public override void Start(Viewer viewer)
        {
            this.viewer = viewer;
            actor = viewer.ActiveActors.First(x => x.ActorInfo.Name == actor.ActorInfo.Name);
            coroutine = viewer.StartCoroutine(GetEnumerator());
        }

        public override void RequestSkip()
        {
            if (coroutine != null)
            {
                viewer.StopCoroutine(coroutine);
                finished = true;
                actor.transform.rotation = Quaternion.identity;
                actor.ChangeEmotion(emote);
            }
        }

        public override bool Update() => finished;

        private IEnumerator GetEnumerator()
        {
            Quaternion from = Quaternion.identity;
            Quaternion to = Quaternion.Euler(0, 90, 0);
            yield return EnumeratorUtil.RotateTo(actor.transform, from, to, TIME_TO_EMOTE / 2);
            actor.ChangeEmotion(emote);
            yield return EnumeratorUtil.RotateTo(actor.transform, to, from, TIME_TO_EMOTE / 2);
            finished = true;
        }

        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            // expected: "name emote"
            string actorName = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out stringStart);
            if (parsedToLine != lineStart)
                throw new ParserException(lineStart, "Enter needs a character name and direction!");

            Actor actor = ActorDirectory.Instance.GetActor(actorName);
            if (actor == null)
                throw new ParserException(lineStart, "Name " + actorName + " not found!");

            string emotionString = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);
            if (!Enum.TryParse(emotionString, true, out ActorInfo.Emotion emote))
                throw new ParserException(lineStart, "Emoition " + emotionString + " could not be found!");

            return new Emote(actor, emote);
        }

        public override void Reset()
        {
            if (coroutine != null)
            {
                viewer.StopCoroutine(coroutine);
                coroutine = null;
            }
            finished = false;
        }
    }
}