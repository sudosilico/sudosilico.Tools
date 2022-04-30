using System;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [Serializable]
    public class GameVariableReference<T, TVariable> 
        where TVariable : GameVariable<T, TVariable>
    {
        [SerializeField]
        private bool _useValue = true;
        
        [SerializeField]
        private T _value;
        
        [SerializeField]
        private TVariable _variable;

        public GameVariableReference()
        {
        }

        public GameVariableReference(T value)
        {
            _useValue = true;
            _value = value;
        }

        public T Value
        {
            get
            {
                if (_useValue)
                {
                    return _value;
                }

                return _variable.Value;
            }

            set
            {
                if (_useValue)
                {
                    _value = value;
                }
                else
                {
                    _variable.Value = value;
                }
            }
        }

        public static implicit operator T(GameVariableReference<T, TVariable> reference)
        {
            return reference.Value;
        }
    }
}