using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/Color", order = 9)]
    public class ColorGameEvent
        : ValueGameEvent<Color, ColorGameEvent>
    {
    }
}