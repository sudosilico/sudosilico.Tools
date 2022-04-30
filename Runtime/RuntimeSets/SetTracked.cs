using UnityEngine;

namespace sudosilico.Tools.RuntimeSets
{
    public class SetTracked : MonoBehaviour
    {
        public GameObjectRuntimeSet RuntimeGameObjectRuntimeSet;

        private void OnEnable()
        {
            if (RuntimeGameObjectRuntimeSet != null)
            {
                RuntimeGameObjectRuntimeSet.Add(gameObject);
            }
        }

        private void OnDisable()
        {
            if (RuntimeGameObjectRuntimeSet != null)
            {
                RuntimeGameObjectRuntimeSet.Remove(gameObject);
            }
        }
    }
}