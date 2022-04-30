using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/bool", order = 1)]
    public class BoolGameEvent
        : ValueGameEvent<bool, BoolGameEvent>
    {
    }
}