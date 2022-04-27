using UnityEngine.Events;
using UnityEngine;

namespace sudosilico.Tools.Events.Responses
{
    [AddComponentMenu("GameEvents/Responses/Trigger Unity Event on Game Event")]
    public class TriggerUnityEventOnGameEvent : GameEventListener
    {
        public UnityEvent Response;
        
        public override void OnEventRaised()
        {
            Response?.Invoke();
        }
    }
}