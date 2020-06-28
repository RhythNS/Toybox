namespace Modularity.Scene
{
    [System.Serializable]
    public abstract class Action
    {
        /// <summary>
        /// 	Called when Action is started. Should not be called multiple times.
        /// </summary>
        public abstract void Start(Viewer viewer);

        /// <summary>
        /// 	Called before Update when the player wants to skip the current action.
        /// 	Some Actions might not be skipable and can ignore this call.
        /// </summary>
        public abstract void RequestSkip();

        /// <summary>
        /// 	Called when a new Frame is processed. Should be used to update timers
        /// 	or other logic inside the Action.
        /// </summary>
        public abstract bool Update();

        /// <summary>
        /// 	Called when an Action needs to reset for a rerunning of the same action.
        /// </summary>
        public abstract void Reset();
    }
}