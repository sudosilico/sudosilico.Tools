using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="BoolVariable.asset", menuName="Variables/bool", order=-3)]
    public class BoolVariable : GameVariable<bool, BoolVariable>
    {
    }
}