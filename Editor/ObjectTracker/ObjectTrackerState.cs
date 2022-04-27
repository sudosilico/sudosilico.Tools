using System.Collections.Generic;
using UnityEngine;

namespace sudosilico.Tools
{
    public class ObjectTrackerState : ScriptableObject
    {
        public List<TrackedObject> TrackedObjects = new List<TrackedObject>();
    }
}