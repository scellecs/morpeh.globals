# Morpeh.Globals
⚡️ Morpeh ECS Scriptable Object Singletons and Events for Unity Game Engine.  

## 📖 How To Install
### Unity Engine

Minimal Unity Version is 2019.4.*  
Require [Git](https://git-scm.com/) + [Git LFS](https://git-lfs.github.com/) for installing package.  
Currently require [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) for drawing in inspector.

Open Unity Package Manager and add Morpeh Globals URL

&nbsp;&nbsp;&nbsp;&nbsp;⭐ Main: https://github.com/scellecs/morpeh.globals.git  
&nbsp;&nbsp;&nbsp;&nbsp;🚧 Dev:  https://github.com/scellecs/morpeh.globals.git#develop  
&nbsp;&nbsp;&nbsp;&nbsp;🏷️ Tag:  https://github.com/scellecs/morpeh.globals.git#2022.2.0  

## 📘 Singletons and Globals Assets. Event system.
There is an execution order in the ECS pattern, so we cannot use standard delegates or events, they will break it.

ECS uses the concept of a deferred call, or the events are data.  
In the simplest cases, events are used as entities with empty components called tags or markers.  
That is, in order to notify someone about the event, you need to create an entity and add an empty component to it.  
Another system creates a filter for this component, and if there are entities, we believe that the event is published.

In general, this is a working approach, but there are several problems.  
Firstly, these tags overwhelm the project with their types, if you wrote a message bus, then you understand what I mean.  
Each event in the game has its own unique type and there is not enough imagination to give everyone a name.  
Secondly, it’s uncomfortable working with these events from MonoBehaviours, UI, Visual Scripting Frameworks (Playmaker, Bolt, etc.)

As a solution to this problem, global assets were created.

🔖 **Singleton** is a simple ScriptableObject that is associated with one specific entity.  
It is usually used to add dynamic components to one entity without using filters.

🔖 **GlobalEvent** is a Singleton, which has the functionality of publishing events to the world by adding a tag to its entity.  
It has 4 main methods for working with it:
1) Publish (arg) - publish in the next frame, all systems will see this.
2) IsPublished - did anyone publish this event
3) BatchedChanges - a data stack where publication arguments are added.

🔖 **GlobalVariable** is a GlobalEvent that stores the start value and the last value after the changes.  
It also has the functionality of saving and loading data from PlayerPrefs.

You can create globals by context menu `Create/ECS/Globals/` in Project Window.  
You can declare globals in any systems, components and scripts and set it by Inspector Window, for example:
```c#  
public sealed class HealthSystem : UpdateSystem {
    public GlobalEvent myEvent;
    ...
}
```

And check their publication with:
```c#  
public sealed class HealthSystem : UpdateSystem {
    public GlobalEvent myEvent;
    ...
    public override void OnUpdate(float deltaTime) {
        if (myEvent.IsPublished) {
            Debug.Log("Event is published");
        }
    }
}
```

And there is also a variation with checking for null:
```c#  
public sealed class HealthSystem : UpdateSystem {
    public GlobalEvent myEvent;
    ...
    public override void OnUpdate(float deltaTime) {
        if (myEvent) {
            Debug.Log("Event is not null and is published");
        }
    }
}
```

---


## 📘 License

📄 [MIT License](LICENSE)

## 💬 Contacts

✉️ Telegram: [olegmrzv](https://t.me/olegmrzv)  
📧 E-Mail: [benjminmoore@gmail.com](mailto:benjminmoore@gmail.com)  
👥 Telegram Community RU: [Morpeh ECS Development](https://t.me/morpeh_development_chat)
