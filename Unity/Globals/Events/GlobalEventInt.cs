namespace Morpeh.Globals {
    using System.Globalization;
    using Scellecs.Morpeh;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Events/Event Int")]
    public class GlobalEventInt : BaseGlobalEvent<int> {
        public override string Serialize(int data) => data.ToString(CultureInfo.InvariantCulture);

        public override int Deserialize(string serializedData) => int.Parse(serializedData, CultureInfo.InvariantCulture);
    }
}