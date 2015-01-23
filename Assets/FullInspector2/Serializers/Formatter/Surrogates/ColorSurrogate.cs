using System.Runtime.Serialization;
using UnityEngine;

namespace FullInspector.Serializers.Formatter {
    internal class ColorSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var color = (Color)obj;
            info.AddValue("r", color.r);
            info.AddValue("g", color.g);
            info.AddValue("b", color.b);
            info.AddValue("a", color.a);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            float r = info.GetSingle("r");
            float g = info.GetSingle("g");
            float b = info.GetSingle("b");
            float a = info.GetSingle("a");

            return new Color(r, g, b, a);
        }
    }

    internal class ColorWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            surrogates.AddSurrogate(typeof(Color), context, new ColorSurrogate());
        }
    }
}