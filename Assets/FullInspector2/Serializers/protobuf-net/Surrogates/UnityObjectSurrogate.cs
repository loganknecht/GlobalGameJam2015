using FullInspector.Internal;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Serializers.ProtoBufNet {
    // HACK:
    // It seems like [OnDeserialize] and [OnSerializing] are broken for surrogates in
    // protobuf-net. The data simply does not get serialized or deserialized. So we have to
    // hack in access to the streaming context inside of the explicit conversion operators so
    // that serialization will work properly.
    internal static class UnityObjectReferenceHack {
        public static ISerializationOperator ActiveOperator;
    }

    [ProtoContract]
    public class UnityObjectSurrogate<TObject> where TObject : UnityObject {
        [ProtoMember(1)]
        public int StorageId;

        // The code below is how serialization really should happen, but this doesn't work. I have
        // no idea why -- if you know why, please submit a bug report!
        /*
        public UnityObject Reference;

        [OnDeserializing]
        public void OnDeserialized(StreamingContext context) {
            var ops = (ISerializationOperator)context.Context;
            Reference = ops.RetrieveObjectReference(StorageId);
        }

        [OnSerializing]
        public void OnSerialized(StreamingContext context) {
            var ops = (ISerializationOperator)context.Context;
            StorageId = ops.StoreObjectReference(Reference);
        }

        public static explicit operator TObject(UnityObjectSurrogate<TObject> surrogate) {
            return (TObject)surrogate.Reference;
        }

        public static explicit operator UnityObjectSurrogate<TObject>(TObject reference) {
            return new UnityObjectSurrogate<TObject>() {
                Reference = reference
            };
        }
        */

        // Here is the hacky serialization method that requires globals but actually works

        public static explicit operator TObject(UnityObjectSurrogate<TObject> surrogate) {
            return (TObject)UnityObjectReferenceHack.ActiveOperator.RetrieveObjectReference(surrogate.StorageId);
        }

        public static explicit operator UnityObjectSurrogate<TObject>(TObject reference) {
            return new UnityObjectSurrogate<TObject>() {
                StorageId = UnityObjectReferenceHack.ActiveOperator.StoreObjectReference(reference)
            };
        }

    }

    // iOS and WebPlayer have AOT compilers; MakeGenericType doesn't work so we remove it from
    // the compilation process.
#if !UNITY_IOS && !UNITY_WEBPLAYER
    public class UnityObjectModelWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            foreach (Type unityObjectType in fiRuntimeReflectionUtility.GetUnityObjectTypes()) {
                if (TypeModelCreator.IsInIgnoredAssembly(unityObjectType)) {
                    continue;
                }

                var surrogateType = typeof(UnityObjectSurrogate<>).MakeGenericType(unityObjectType);
                SetSurrogate(model, unityObjectType, surrogateType);
            }
        }
    }
#endif
}