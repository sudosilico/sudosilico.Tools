using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/Vector3", order = 5)]
    public class Vector3GameEvent
        : ValueGameEvent<Vector3, Vector3GameEvent>
    {
    }
}