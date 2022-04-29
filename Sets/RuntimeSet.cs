using System.Collections.Generic;
using sudosilico.Tools.GameEvents;
using UnityEngine;

namespace sudosilico.Tools.Sets
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        public List<T> Items = new();

        [Tooltip("A GameEvent to raise whenever the set is modified.")]
        public GameEvent OnSetModified;
        
        public void Add(T item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);

                if (OnSetModified != null)
                {
                    OnSetModified.Raise();
                }
            }
        }

        public void Remove(T item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                
                if (OnSetModified != null)
                {
                    OnSetModified.Raise();
                }
            }
        }
    }
}