using FullInspector.Internal;
using System;
using System.Runtime.Serialization;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Serializers.Formatter {
    internal class UnityObjectSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var serializationOperator = (ISerializationOperator)context.Context;

            UnityObject unityObject = (UnityObject)obj;
            int id = serializationOperator.StoreObjectReference(unityObject);
            info.AddValue("id", id);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector) {

            var serializationOperator = (ISerializationOperator)context.Context;

            int id = info.GetInt32("id");
            return serializationOperator.RetrieveObjectReference(id);
        }
    }

    internal class UnityObjectBinaryFormatterWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            var unityObjectSurrogate = new UnityObjectSurrogate();
            foreach (Type unityObjectType in fiRuntimeReflectionUtility.GetUnityObjectTypes()) {
                surrogates.AddSurrogate(unityObjectType, context, unityObjectSurrogate);
            }
        }
    }
}