using UnityEngine;

namespace sudosilico.Tools.Events.Responses
{
    [AddComponentMenu("GameEvents/Responses/Log on GameEvent")]
    public class LogOnGameEvent : GameEventListener
    {
        public string Message = "Event logged!";
        
        public override void OnEventRaised()
        {
            Debug.Log(Message, this);
        }
    }
}