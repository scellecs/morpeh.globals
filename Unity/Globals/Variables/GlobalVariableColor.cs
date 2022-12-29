namespace Scellecs.Morpeh.Globals.Variables {
    using System;
    using Unity.IL2CPP.CompilerServices;
    using UnityEngine;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Variables/Variable Color")]
    public class GlobalVariableColor : BaseGlobalVariable<Color> {
        public override DataWrapper Wrapper {
            get => new ColorWrapper {value = this.value};
            set => this.Value = ((ColorWrapper) value).value;
        }
        
        public override Color Deserialize(string serializedData) => JsonUtility.FromJson<ColorWrapper>(serializedData).value;

        public override string Serialize(Color data) => JsonUtility.ToJson(new ColorWrapper {value = data});

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        [Serializable]
        private class ColorWrapper : DataWrapper {
            public Color value;

            public override string ToString() => this.value.ToString();
        }
    }
}