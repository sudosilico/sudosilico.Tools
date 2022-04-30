using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/int", order = 3)]
    public class IntGameEvent 
        : ValueGameEvent<int, IntGameEvent>
    {
    }
}