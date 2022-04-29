using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [AddComponentMenu("GameEvents/Responses/Log on GameEvent")]
    public class LogOnGameEvent : GameEvent.Listener
    {
        public string Message = "Event logged!";
        
        public override void OnEventRaised()
        {
            Debug.Log(Message, this);
        }
    }
}