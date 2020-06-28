using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Modularity.Scene
{
    public class ActorDirectory : MonoBehaviour
    {
        [SerializeField]
        private List<Actor> actorInfo;
        public List<Actor> ActorInfo { get => actorInfo; }

        public static ActorDirectory Instance { get; private set; }

        private void Start()
        {
            Instance = this;
        }

        public Actor GetActor(string name) => 
            actorInfo.First(x => x.ActorInfo.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}