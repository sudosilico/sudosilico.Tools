using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="TransformVariable.asset", menuName="Variables/Transform", order=-3)]
    public class TransformVariable : GameVariable<Transform, TransformVariable>
    {
    }
}