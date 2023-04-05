namespace Scellecs.Morpeh.Globals {
    using System;
    using JetBrains.Annotations;
    using Scellecs.Morpeh;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;
    
#if UNITY_EDITOR
    using UnityEditor;
    using Scellecs.Morpeh.Editor;
    using Sirenix.OdinInspector;
#endif

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public abstract class BaseSingleton : ScriptableObject {
        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        protected int internalEntityID = -1;

        protected Entity internalEntity;

#if UNITY_EDITOR
        [PropertyOrder(100)]
        [ShowInInspector]
        [InlineProperty]
        [HideReferenceObjectPicker]
        [HideLabel]
        [Title("","Debug Info", HorizontalLine = true)]
        [Space]
        private EntityViewer entityViewer = new EntityViewer();
#endif

        [CanBeNull]
        private protected Entity InternalEntity {
            get {
#if UNITY_EDITOR 
                if (this.internalEntityID > -1 && this.internalEntity == null) {
                    this.internalEntity = World.Default.entities[this.internalEntityID];
                }
#endif
                return this.internalEntity;
            }
        }

        [NotNull]
        public Entity Entity {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return default;
                }

                this.CheckIsInitialized();
#endif
                return this.InternalEntity;
            }
        }

        protected virtual void OnEnable() {
            this.internalEntity = null;
#if UNITY_EDITOR
            this.entityViewer = new EntityViewer {getter = () => this.internalEntity};
#endif
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += this.OnEditorApplicationOnplayModeStateChanged;
            if (Application.isPlaying) {
#endif
                this.CheckIsInitialized();
#if UNITY_EDITOR
            }
#endif
        }

#if UNITY_EDITOR
        protected virtual void OnEditorApplicationOnplayModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode) {
                this.internalEntityID = -1;
                this.internalEntity   = null;
            }
        }
#endif
        protected virtual bool CheckIsInitialized() {
            if (this.internalEntityID < 0) {
                var world = World.Default;
                var cache = world.GetStash<SingletonMarker>();
                
                this.internalEntity = world.CreateEntity();
                this.internalEntityID = this.internalEntity.ID.id;
                cache.Add(this.internalEntity);
                
                return true;
            }

            return false;
        }

        public virtual void Dispose() {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= this.OnEditorApplicationOnplayModeStateChanged;
#endif
            if (this.internalEntityID != -1) {
                var entity = this.InternalEntity;
                if (entity != null && !entity.isDisposed) {
                    World.Default.RemoveEntity(entity);
                }

                this.internalEntityID = -1;
                this.internalEntity   = null;
            }
        }


        protected virtual void OnDestroy() {
            this.Dispose();
        }

        [Serializable]
        private struct SingletonMarker : IComponent {
        }
    }
}