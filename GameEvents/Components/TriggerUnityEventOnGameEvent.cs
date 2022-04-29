using UnityEngine.Events;
using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [AddComponentMenu("GameEvents/Responses/Trigger Unity Event on Game Event")]
    public class TriggerUnityEventOnGameEvent : GameEvent.Listener
    {
        public UnityEvent Response;
        
        public override void OnEventRaised()
        {
            Response?.Invoke();
        }
    }
}