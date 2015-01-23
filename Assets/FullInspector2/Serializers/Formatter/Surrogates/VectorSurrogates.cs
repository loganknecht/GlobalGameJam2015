using System.Runtime.Serialization;
using UnityEngine;

namespace FullInspector.Serializers.Formatter {
    internal class Vector2Surrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var vec = (Vector2)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            return new Vector2(info.GetSingle("x"), info.GetSingle("y"));
        }
    }
    internal class Vector3Surrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var vec = (Vector3)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("z", vec.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            return new Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
        }
    }
    internal class Vector4Surrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var vec = (Vector4)obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("z", vec.z);
            info.AddValue("w", vec.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            return new Vector4(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"), info.GetSingle("w"));
        }
    }

    internal class VectorBinaryFormatterWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            surrogates.AddSurrogate(typeof(Vector2), context, new Vector2Surrogate());
            surrogates.AddSurrogate(typeof(Vector3), context, new Vector3Surrogate());
            surrogates.AddSurrogate(typeof(Vector4), context, new Vector4Surrogate());
        }
    }
}