using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace FullInspector.Serializers.Formatter {
    public class GradientSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            throw new NotSupportedException("Sorry, BinaryFormatter currently doesn't support Gradients");

            //var gradient = (Gradient)obj;
            //info.AddValue("alphaKeys", gradient.alphaKeys);
            //info.AddValue("colorKeys", gradient.colorKeys);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector) {
            throw new NotSupportedException("Sorry, BinaryFormatter currently doesn't support Gradients");

            //var alphaKeys = (GradientAlphaKey[])info.GetValue("alphaKeys", typeof(GradientAlphaKey[]));
            //var colorKeys = (GradientColorKey[])info.GetValue("colorKeys", typeof(GradientColorKey[]));

            //return new Gradient {
            //    alphaKeys = alphaKeys,
            //    colorKeys = colorKeys
            //};
        }
    }

    public class GradientAlphaKeySurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var key = (GradientAlphaKey)obj;
            info.AddValue("alpha", key.alpha);
            info.AddValue("time", key.time);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            var alpha = info.GetSingle("alpha");
            var time = info.GetSingle("time");

            return new GradientAlphaKey {
                alpha = alpha,
                time = time
            };
        }
    }

    public class GradientColorKeySurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var key = (GradientColorKey)obj;
            info.AddValue("color", key.color);
            info.AddValue("time", key.time);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            var color = (Color)info.GetValue("color", typeof(Color));
            var time = info.GetSingle("time");

            return new GradientColorKey {
                color = color,
                time = time
            };
        }
    }

    internal class GradientWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            surrogates.AddSurrogate(typeof(Gradient), context, new GradientSurrogate());
            surrogates.AddSurrogate(typeof(GradientAlphaKey), context, new GradientAlphaKeySurrogate());
            surrogates.AddSurrogate(typeof(GradientColorKey), context, new GradientColorKeySurrogate());
        }
    }
}