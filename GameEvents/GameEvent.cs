using System;
using System.Collections.Generic;
using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Game Event")]
    public class GameEvent : ScriptableObject
    {
        private List<Listener> _listeners = new();
        private event Action _onEventRaised;

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised();
            }

            if (_onEventRaised != null)
            {
                _onEventRaised.Invoke();
            }
        }

        internal void RegisterListener(Listener listener)
        {
            _listeners.Add(listener);
        }

        internal void UnregisterListener(Listener listener)
        {
            _listeners.Remove(listener);
        }

        public void AddCustomHandler(Action onEventRaised)
        {
            _onEventRaised += onEventRaised;
        }

        public void RemoveCustomHandler(Action onEventRaised)
        {
            _onEventRaised -= onEventRaised;
        }

        public abstract class Listener : MonoBehaviour
        {
            public GameEvent Event;

            private void OnEnable()
            {
                if (Event != null)
                {
                    Event.RegisterListener(this);
                }
            }

            private void OnDisable()
            {
                if (Event != null)
                {
                    Event.UnregisterListener(this);
                }
            }

            public abstract void OnEventRaised();
        }
    }
}