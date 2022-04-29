using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [AddComponentMenu("GameEvents/Responses/Load Scene on GameEvent")]
    public class LoadSceneOnGameEvent : GameEvent.Listener
    {
        public SceneReference Scene;
        public override void OnEventRaised()
        {
            Scene.LoadScene();
        }
    }
}