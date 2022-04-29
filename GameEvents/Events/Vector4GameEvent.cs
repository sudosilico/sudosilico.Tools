using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/Vector4", order = 6)]
    public class Vector4GameEvent
        : ValueGameEvent<Vector4, Vector4GameEvent>
    {
    }
}