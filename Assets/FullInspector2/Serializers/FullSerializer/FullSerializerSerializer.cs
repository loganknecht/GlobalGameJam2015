using FullInspector.Serializers.FullSerializer;
using FullSerializer;
using System.Reflection;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Implements Full Inspector integration with Full Serializer, a .NET serializer that just
    /// works. Use Unity style annotations (such as [SerializeField]) to serialize your types.
    /// </summary>
    public class FullSerializerSerializer : BaseSerializer {
        private static fsSerializer _serializer;

        static FullSerializerSerializer() {
            _serializer = new fsSerializer();
            _serializer.AddConverter(new UnityObjectConverter());

#if UNITY_EDITOR
            _serializer.AddConverter(new AnimBoolConverter());
            _serializer.AddConverter(new AnimFloatConverter());
#endif
        }

        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            _serializer.Context.Set(serializationOperator);

            fsData data;
            var fail = _serializer.TrySerialize(GetStorageType(storageType), value, out data);
            if (EmitFailWarning(fail)) return null;

            return fsJsonPrinter.CompressedJson(data);
        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            fsFailure fail;

            fsData data;
            fail = fsJsonParser.Parse(serializedState, out data);
            if (EmitFailWarning(fail)) return null;

            _serializer.Context.Set(serializationOperator);

            object deserialized = null;
            fail = _serializer.TryDeserialize(data, GetStorageType(storageType), ref deserialized);
            if (EmitFailWarning(fail)) return null;

            return deserialized;
        }

        private static bool EmitFailWarning(fsFailure fail) {
            if (fail.Failed) {
                if (fiSettings.EmitWarnings) {
                    Debug.LogWarning(fail.FailureReason);
                }
                return true;
            }

            return false;
        }
    }
}