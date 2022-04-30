using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/string", order = 7)]
    public class StringGameEvent
        : ValueGameEvent<string, StringGameEvent>
    {
    }
}