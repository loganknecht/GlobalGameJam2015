#if UNITY_EDITOR

using FullSerializer;
using System;
using UnityEditor.AnimatedValues;

namespace FullInspector.Serializers.FullSerializer {
    public class AnimBoolConverter : fsConverter {
        public override bool CanProcess(Type type) {
            return type == typeof(AnimBool);
        }

        public override fsFailure TrySerialize(object instance, out fsData serialized, Type storageType) {
            var anim = (AnimBool)instance;
            serialized = new fsData(anim.target);
            return fsFailure.Success;
        }

        public override fsFailure TryDeserialize(fsData data, ref object instance, Type storageType) {
            instance = new AnimBool(data.AsBool);
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