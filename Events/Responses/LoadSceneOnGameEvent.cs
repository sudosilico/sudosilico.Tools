using UnityEngine;

namespace sudosilico.Tools.Events.Responses
{
    [AddComponentMenu("GameEvents/Responses/Load Scene on GameEvent")]
    public class LoadSceneOnGameEvent : GameEventListener
    {
        public SceneReference Scene;
        
        public override void OnEventRaised()
        {
            Scene.LoadScene();
        }
    }
}