using System;

namespace Dennis.Tools.DialogueGraph.Event
{
    /// <summary>
    /// Sample Game Event System without value
    /// </summary>
    public class GameEvent
    {
        private event Action _action;

        /// <summary>
        /// Publish Event
        /// </summary>
        public void Publish()
        {
            _action?.Invoke();
        }

        /// <summary>
        /// Add Subscriber
        /// </summary>
        /// <param name="subscriber"></param>
        public void Add(Action subscriber)
        {
            _action += subscriber;
        }

        /// <summary>
        /// Remove Subscriber
        /// </summary>
        /// <param name="subscriber"></param>
        public void Remove(Action subscriber)
        {
            _action -= subscriber;
        }
    }

    /// <summary>
    /// Sample Game Event System with value
    /// </summary>
    /// <typeparam name="T">any value</typeparam>
    public class GameEvent<T>
    {
        private event Action<T> _action;

        /// <summary>
        /// Publish Event
        /// </summary>
        /// <param name="param"></param>
        public void Publish(T param)
        {
            _action?.Invoke(param);
        }

        /// <summary>
        /// Add Subscriber
        /// </summary>
        /// <param name="subscriber"></param>
        public void Add(Action<T> subscriber)
        {
            _action += subscriber;
        }

        /// <summary>
        /// Remove Subscriber
        /// </summary>
        /// <param name="subscriber"></param>
        public void Remove(Action<T> subscriber)
        {
            _action -= subscriber;
        }
    }
}