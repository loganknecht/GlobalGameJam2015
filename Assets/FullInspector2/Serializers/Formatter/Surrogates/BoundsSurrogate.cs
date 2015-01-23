using System.Runtime.Serialization;
using UnityEngine;

namespace FullInspector.Serializers.Formatter {
    internal class BoundsSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var bounds = (Bounds)obj;
            info.AddValue("center", bounds.center);
            info.AddValue("size", bounds.size);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            var center = (Vector3)info.GetValue("center", typeof(Vector3));
            var size = (Vector3)info.GetValue("size", typeof(Vector3));

            return new Bounds(center, size);
        }
    }

    internal class BoundsWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            surrogates.AddSurrogate(typeof(Bounds), context, new BoundsSurrogate());
        }
    }
}