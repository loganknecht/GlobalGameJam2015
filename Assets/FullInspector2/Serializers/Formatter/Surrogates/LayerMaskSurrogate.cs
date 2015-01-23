using System.Runtime.Serialization;
using UnityEngine;

namespace FullInspector.Serializers.Formatter {
    public class LayerMaskSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var mask = (LayerMask)obj;
            info.AddValue("value", mask.value);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector) {

            int value = info.GetInt32("value");
            return new LayerMask() {
                value = value
            };
        }
    }

    internal class LayerMaskWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            surrogates.AddSurrogate(typeof(LayerMask), context, new LayerMaskSurrogate());
        }
    }
}