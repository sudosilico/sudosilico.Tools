using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/float", order = 2)]
    public class FloatGameEvent
        : ValueGameEvent<float, FloatGameEvent>
    {
    }
}