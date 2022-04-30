using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="ColorVariable.asset", menuName="Variables/Color", order=-3)]
    public class ColorVariable : GameVariable<Color, ColorVariable>
    {
    }
}