using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CreateAssetMenu(fileName="PrefabVariable.asset", menuName="Variables/Prefab", order=-3)]
    public class PrefabVariable : GameVariable<GameObject, PrefabVariable>
    {
    }
}