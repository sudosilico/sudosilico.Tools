using UnityEngine;

namespace sudosilico.Tools.Sets
{
    public class SetTracked : MonoBehaviour
    {
        public SetOfGameObjects RuntimeSet;

        private void OnEnable()
        {
            if (RuntimeSet != null)
            {
                RuntimeSet.Add(gameObject);
            }
        }

        private void OnDisable()
        {
            if (RuntimeSet != null)
            {
                RuntimeSet.Remove(gameObject);
            }
        }
    }
}