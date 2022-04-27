using UnityEngine;

namespace sudosilico.Tools.Events
{
    public abstract class GameEventListener : MonoBehaviour
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
        
        private void RaiseGameEvent()
        {
            Event.Raise();
        }
    }
}