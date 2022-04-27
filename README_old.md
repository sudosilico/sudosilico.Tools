# sudosilico.Tools

This package includes a collection of useful tools for Unity.

---

## Features:

[UniqueID](#) - The standard C# `System.Guid` type is not normally serialized by Unity. `UniqueID` is a serializable class that wraps a `System.Guid` and serializes it using Unity's `ISerializationCallbackReceiver`. It also implements the necessary GetHashCode and comparator methods to be used efficiently as a dictionary key type. 

### UniqueIDComponent

The `UniqueIDComponent` is a MonoBehaviour that associates a UniqueID with a GameObject and registers it with the UniqueIDManager, which can be used to retrieve any loaded GameObject based on it's UniqueID. A `UniqueIDComponent` is automatically added to a GameObject when it is expected on one (such as when a GameObject is used in a UniqueIDReference), so you don't usually need to add these yourself.

### UniqueIDReference

The `UniqueIDReference` is used to store references to GameObjects based on their UniqueID, allowing references to GameObjects in other scenes.




---

## Features

- Multiple ScriptableObject-oriented-architecture APIs built on top of concepts from 
  - GameEvent assets, including implementations for common behaviors such as 'Load Scene on Game Event', 'Debug Log on Game Event', and 'Raise Game Event on Collision'. [Learn more...](#)
  - Variable assets and variable references.  [Learn more...](#)
- Audio Pooling

### References:

- [GUID Based reference workflow for System Shock 3 - Unite Copenhagen](https://www.youtube.com/watch?v=6lRzXqfMXRo) - William Armstrong
- [Unite Austin 2017 - Game Architecture with Scriptable Objects
](https://www.youtube.com/watch?v=raQ3iHhE_Kk) - Ryan Hipple
- [Unite 2016 - Overthrowing the MonoBehaviour Tyranny in a Glorious Scriptable Object Revolution
](https://www.youtube.com/watch?v=6vmRwLYWNRo) - Richard Fine
