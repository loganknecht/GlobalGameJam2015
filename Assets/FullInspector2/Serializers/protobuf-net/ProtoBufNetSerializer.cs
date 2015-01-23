#if UNITY_IOS || UNITY_WEBPLAYER
#define PROTOBUF_USE_PRECOMPILER
#endif

using FullInspector.Internal;
using FullInspector.Serializers.ProtoBufNet;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace FullInspector {
    public class ProtoBufNetSerializer : BaseSerializer {
        static ProtoBufNetSerializer() {
#if PROTOBUF_USE_PRECOMPILER
            // If we are using a precompiler, then we want to locate the type model class that we
            // are using for serialization. We do this via reflection and not statically so that we
            // don't generate compiler errors complaining about a missing type.
            Type serializer = TypeCache.FindType(ProtoBufNetSettings.PrecompiledSerializerTypeName,
                ProtoBufNetSettings.PrecompiledSerializerTypeName, /*willThrow:*/false);
            if (serializer == null) {
                Debug.LogError(string.Format("Unable to load precompiled protobuf-net serializer " +
                    "(searched for type {0}); have you compiled it using " +
                    "\"Window/Full Inspector/Compile protobuf-net Serialization DLL\"?",
                    ProtoBufNetSettings.PrecompiledSerializerTypeName));
            }

            else {
                _serializer = (TypeModel)Activator.CreateInstance(serializer);
            }
#else
            var serializer = TypeModelCreator.CreateModel();
            serializer.Compile();
            _serializer = serializer;
#endif
        }

        /// <summary>
        /// The serializer that we use for loading objects.
        /// </summary>
        private static TypeModel _serializer;

        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            if (value == null) return "null";

            if (_serializer == null) {
                throw new InvalidOperationException("No protobuf-net serializer is loaded");
            }

            UnityObjectReferenceHack.ActiveOperator = serializationOperator;

            using (var stream = new MemoryStream()) {
                _serializer.Serialize(stream, value, new SerializationContext() {
                    Context = serializationOperator
                });

                UnityObjectReferenceHack.ActiveOperator = null;

                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            if (serializedState == "null") return null;

            if (_serializer == null) {
                throw new InvalidOperationException("No protobuf-net serializer is loaded");
            }

            UnityObjectReferenceHack.ActiveOperator = serializationOperator;

            byte[] bytes = Convert.FromBase64String(serializedState);
            using (var stream = new MemoryStream(bytes)) {
                var deserialized = _serializer.Deserialize(stream, null, GetStorageType(storageType),
                    new SerializationContext() {
                        Context = serializationOperator
                    });

                UnityObjectReferenceHack.ActiveOperator = null;

                return deserialized;
            }
        }
    }
}