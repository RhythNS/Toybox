using UnityEngine;

namespace Modularity.Scene
{
    public class Actor : MonoBehaviour
    {
        [SerializeField]
        private ActorInfo actorInfo;
        public ActorInfo ActorInfo
        {
            get => actorInfo;
            set
            {
                actorInfo = value;
            }
        }

        private SpriteRenderer spriteRenderer;

        public Vector2 Size
        {
            get
            {
                Vector3 temp = actorInfo.GetImage(ActorInfo.Emotion.Normal).bounds.size;
                return transform.lossyScale * new Vector2(temp.x, temp.y);
            }
        }

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale *= Camera.main.orthographicSize / 5;
        }

        public void ChangeEmotion(ActorInfo.Emotion emotion)
        {
            spriteRenderer.sprite = ActorInfo.GetImage(emotion);
        }


    }
}