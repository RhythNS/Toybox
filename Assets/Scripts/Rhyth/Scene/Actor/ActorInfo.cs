using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularity.Scene
{
    [System.Serializable]
    public class ActorInfo 
    {
        [SerializeField]
        private string name;
        public string Name { get => name; }

        [System.Serializable]
        public struct EmotionStruct
        {
            public Emotion emotion;
            public Sprite image;
        }

        [System.Serializable]
        public enum Emotion
        {
            Normal, Annoyed, Suprised, Happy, Hurt
        }

        [SerializeField]
        private EmotionStruct[] emotions;
        public EmotionStruct[] Emotions { get => emotions; }

        public Sprite GetImage(Emotion emotion)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i].emotion == emotion)
                {
                    return emotions[i].image;
                }
            }
            throw new System.Exception("Sprite for emotion not found: " + emotion);
        }

    }
}