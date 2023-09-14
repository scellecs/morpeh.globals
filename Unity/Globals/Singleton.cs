namespace Scellecs.Morpeh.Globals {
    using System;
    using Scellecs.Morpeh;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;
    
#if UNITY_EDITOR
    using Sirenix.OdinInspector;
    using UnityEditor;
#endif

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Singleton")]
    public class Singleton : BaseSingleton {
        [Space]
        [SerializeReference]
#if UNITY_EDITOR
        [HideIf(nameof(ShowSerializedComponents))]
#endif
        public IComponent[] serializedComponents = Array.Empty<IComponent>();

#if UNITY_EDITOR
        private bool ShowSerializedComponents => this.internalEntityID > -1;
#endif

        protected override bool CheckIsInitialized() {
            var check = base.CheckIsInitialized();
            if (check) {
                foreach (var component in this.serializedComponents) {
                    var type = component.GetType();
                    if (CommonTypeIdentifier.typeAssociation.TryGetValue(type, out var definition)) {
                        definition.entitySetComponentBoxed(this.internalEntity, component);
                    }
                    else {
                        Debug.LogError(
                            $"[MORPEH] For using {type.Name} in a Singleton you must warmup it or IL2CPP will strip it from the build.\nCall <b>TypeIdentifier<{type.Name}>.Warmup();</b> before access this Singleton.");
                    }
                }
            }
            return check;
        }
    }
}