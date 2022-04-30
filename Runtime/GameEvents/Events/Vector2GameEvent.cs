using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/Vector2", order = 4)]
    public class Vector2GameEvent
        : ValueGameEvent<Vector2, Vector2GameEvent>
    {
    }
}