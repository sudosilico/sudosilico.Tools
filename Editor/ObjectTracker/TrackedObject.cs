using System;

namespace sudosilico.Tools
{
    [Serializable]
    public class TrackedObject
    {
        public SceneReference Scene;
        public UniqueID ID;
        public string Name;

        public TrackedObject(SceneReference scene, UniqueID id, string name)
        {
            Scene = scene;
            ID = id;
            Name = name;
        }
    }
}