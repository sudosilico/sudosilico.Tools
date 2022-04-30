using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="StringVariable.asset", menuName="Variables/string", order=-3)]
    public class StringVariable : GameVariable<string, StringVariable>
    {
    }
}