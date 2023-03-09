[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Scellecs.Morpeh.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Scellecs.Morpeh.TestSuite")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Scellecs.Morpeh.TestSuite.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Scellecs.Morpeh.Native")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Scellecs.Morpeh.Workaround")]

namespace Scellecs.Morpeh.Globals {
    using System;
    using System.Collections.Generic;
    using Scellecs.Morpeh;
    using Scellecs.Morpeh.Collections;
    using UnityEngine;
    using UnityEngine.Scripting;
    namespace ECS {

        [Serializable]
        public struct GlobalEventMarker : IComponent {
        }

        //rework for multithread
        internal abstract class GlobalEventComponentUpdater : IDisposable {
            internal static Dictionary<int, List<GlobalEventComponentUpdater>> updaters = new Dictionary<int, List<GlobalEventComponentUpdater>>();

            protected Filter filterPublished;
            protected Filter filterNextFrame;

            internal abstract void Awake(World world);

            internal abstract void Update();

            public abstract void Dispose();
        }

        internal sealed class GlobalEventComponentUpdater<T> : GlobalEventComponentUpdater {
            public static BitMap initialized = new BitMap();

            private int worldId;
            
            private Stash<GlobalEventComponent<T>> eventsCache;
            private Stash<GlobalEventPublished>    publishedCache;
            private Stash<GlobalEventNextFrame>    nextFrameCache;
            
            internal override void Awake(World world) {
                this.worldId = world.identifier;
                
                initialized.Set(this.worldId);
                
                var common = world.Filter.With<GlobalEventMarker>().With<GlobalEventComponent<T>>();
                this.filterPublished = common.With<GlobalEventPublished>().Without<GlobalEventNextFrame>().Build();
                this.filterNextFrame = common.With<GlobalEventNextFrame>().Build();

                this.eventsCache    = world.GetStash<GlobalEventComponent<T>>();
                this.publishedCache = world.GetStash<GlobalEventPublished>();
                this.nextFrameCache = world.GetStash<GlobalEventNextFrame>();
            }

            internal override void Update() {
                foreach (var entity in this.filterPublished) {
                    ref var evnt = ref this.eventsCache.Get(entity);
                    evnt.Action?.Invoke(evnt.Data);
                    evnt.Data.Clear();
                    this.publishedCache.Remove(entity);
                }
                
                foreach (var entity in this.filterNextFrame) {
                    this.publishedCache.Set(entity);
                    this.nextFrameCache.Remove(entity);
                    
                    ref var evnt = ref this.eventsCache.Get(entity);
                    while (evnt.NewData.Count > 0) {
                        evnt.Data.Push(evnt.NewData.Dequeue());
                    }
                }
            }

            public override void Dispose() {
                initialized.Unset(this.worldId);
            }
        }


        [Serializable]
        public struct GlobalEventComponent<TData> : IComponent {
            public Action<IEnumerable<TData>> Action;
            public Stack<TData>               Data;
            public Queue<TData>               NewData;
        }
        [Serializable]
        public struct GlobalEventLastToString : IComponent {
            public Func<string> LastToString;
        }

        [Serializable]
        public struct GlobalEventPublished : IComponent {
        }

        [Serializable]
        public struct GlobalEventNextFrame : IComponent {
        }

        internal sealed class ProcessEventsSystem : ILateSystem {
            public World World { get; set; }
            public int worldId;

            public void OnAwake() {
                this.worldId = this.World.identifier;
            }

            public void OnUpdate(float deltaTime) {
                if (GlobalEventComponentUpdater.updaters.TryGetValue(this.worldId, out var updaters)) {
                    foreach (var updater in updaters) {
                        updater.Update();
                    }
                }
            }

            public void Dispose() {
                if (GlobalEventComponentUpdater.updaters.TryGetValue(this.worldId, out var updaters)) {
                    foreach (var updater in updaters) {
                        updater.Dispose();
                    }
                    updaters.Clear();
                }
            }
        }
    }

    internal sealed class GlobalsWorldPlugin : IWorldPlugin {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void RuntimeInitialize() {
            WorldExtensions.AddWorldPlugin(new GlobalsWorldPlugin());
        }
        
        public void Initialize(World world) {
            var sg = world.CreateSystemsGroup();
            sg.AddSystem(new ECS.ProcessEventsSystem());
            world.AddPluginSystemsGroup(sg);
        }
        
        public void Deinitialize(World world) {
            
        }
    }
}