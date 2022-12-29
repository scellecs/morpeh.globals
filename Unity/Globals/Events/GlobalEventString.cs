namespace Scellecs.Morpeh.Globals.Events {
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Events/Event String")]
    public class GlobalEventString : BaseGlobalEvent<string> {
        public override string Serialize(string data) => data;

        public override string Deserialize(string serializedData) => serializedData;
    }
}