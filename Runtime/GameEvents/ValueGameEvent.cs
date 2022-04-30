using System;
using System.Collections.Generic;
using UnityEngine;

namespace sudosilico.Tools
{
    public abstract class ValueGameEvent<TValue, TEvent> : ScriptableObject
        where TEvent : ValueGameEvent<TValue, TEvent>
    {
        private List<Listener> _listeners = new();
        private event Action<TValue> _onEventRaised;
        
        public void Raise(TValue value)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised(value);
            }
            
            if (_onEventRaised != null)
            {
                _onEventRaised.Invoke(value);
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

        public void AddCustomHandler(Action<TValue> onEventRaised)
        {
            _onEventRaised += onEventRaised;
        }

        public void RemoveCustomHandler(Action<TValue> onEventRaised)
        {
            _onEventRaised -= onEventRaised;
        }

        public abstract class Listener : MonoBehaviour
        {
            public TEvent Event;
        
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

            public abstract void OnEventRaised(TValue value);
        }
    }
}