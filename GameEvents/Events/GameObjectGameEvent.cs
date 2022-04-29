using UnityEngine;

namespace sudosilico.Tools.GameEvents
{
    [CreateAssetMenu(menuName = "Events/Value Game Event/GameObject", order = 8)]
    public class GameObjectGameEvent
        : ValueGameEvent<GameObject, GameObjectGameEvent>
    {
    }
}