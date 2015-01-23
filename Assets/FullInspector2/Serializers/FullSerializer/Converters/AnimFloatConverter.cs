#if UNITY_EDITOR

using FullSerializer;
using System;
using UnityEditor.AnimatedValues;

namespace FullInspector.Serializers.FullSerializer {
    public class AnimFloatConverter : fsConverter {
        public override bool CanProcess(Type type) {
            return type == typeof(AnimFloat);
        }

        public override fsFailure TrySerialize(object instance, out fsData serialized, Type storageType) {
            var anim = (AnimFloat)instance;
            serialized = new fsData(anim.target);
            return fsFailure.Success;
        }

        public override fsFailure TryDeserialize(fsData data, ref object instance, Type storageType) {
            instance = new AnimFloat(data.AsFloat);
            return fsFailure.Success;
        }

        public override bool RequestCycleSupport(Type storageType) {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType) {
            return false;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return storageType;
        }
    }
}

#endif