using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="GameObjectVariable.asset", menuName="Variables/GameObject", order=-3)]
    public class GameObjectVariable : GameVariable<GameObject, GameObjectVariable>
    {
    }
}