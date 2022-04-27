using UnityEngine;

namespace sudosilico.Tools.Events.Triggers
{
    [AddComponentMenu("GameEvents/Triggers/GameEvent Overlap Trigger")]
    [RequireComponent(typeof(Collider))]
    public class GameEventOverlapTrigger : MonoBehaviour
    {
        public GameEvent OnEnter;
        public GameEvent OnExit;

        private void OnCollisionEnter(Collision other)
        {
            if (OnEnter != null)
            {
                OnEnter.Raise();
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (OnExit != null)
            {
                OnExit.Raise();
            }
        }
    }
}