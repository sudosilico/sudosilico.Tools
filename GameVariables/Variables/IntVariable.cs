using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="IntVariable.asset", menuName="Variables/int", order=-3)]
    public class IntVariable : GameVariable<int, IntVariable>
    {
    }
}