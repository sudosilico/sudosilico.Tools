using sudosilico.Tools.GameEvents;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    public class VariableStuff : MonoBehaviour
    {
        [Header("Game Events")]
        public GameEvent GameEvent1;
        public GameEvent GameEvent2;
        public GameEvent GameEvent3;
        
        [Header("Value Events")]
        public IntGameEvent IntGameEvent;
        public FloatGameEvent FloatGameEvent;
        public BoolGameEvent BoolGameEvent;

        [Header("Variables")]
        public FloatVariable FloatVariable;
        public BoolVariable BoolVariable;
        public GameObjectVariable GameObjectVariable1;
        public GameObjectVariable GameObjectVariable2;
        public GameObjectVariable GameObjectVariable3;
        public PrefabVariable PrefabVariable1;
        public PrefabVariable PrefabVariable2;
        
        [Header("References")]
        public FloatReference FloatReference1;
        public FloatReference FloatReference2;
        public FloatReference FloatReference3;
        public BoolReference BoolReference;
        public ColorReference ColorReference;
        public GameObjectReference GameObjectReference1;
        public GameObjectReference GameObjectReference2;
        public GameObjectReference GameObjectReference3;
        public GameObjectReference GameObjectReference4;
        public PrefabReference PrefabReference1;
        public PrefabReference PrefabReference2;
        public PrefabReference PrefabReference3;

        [Header("Prefab Attribute")]
        [Prefab()]
        public GameObject AnyPrefab;
    }
}