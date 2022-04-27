using System;

namespace sudosilico.Tools
{
    [Serializable]
    public class GameVariableReference<TVariable, T> 
        where TVariable : GameVariable<T>
    {
        public bool UseConstant = true;
        public T ConstantValue;
        public TVariable Variable;

        public GameVariableReference()
        {
        }

        public GameVariableReference(T value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public T Value => UseConstant ? ConstantValue : Variable.Value;

        public static implicit operator T(GameVariableReference<TVariable, T> reference)
        {
            return reference.Value;
        }
    }
}