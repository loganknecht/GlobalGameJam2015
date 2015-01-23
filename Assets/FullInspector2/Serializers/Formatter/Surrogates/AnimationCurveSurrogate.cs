using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace FullInspector.Serializers.Formatter {
    internal class KeyframeSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            var keyframe = (Keyframe)obj;

            info.AddValue("time", keyframe.time);
            info.AddValue("value", keyframe.value);
            info.AddValue("tangentMode", keyframe.tangentMode);
            info.AddValue("inTangent", keyframe.inTangent);
            info.AddValue("outTangent", keyframe.outTangent);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            float time = info.GetSingle("time");
            float value = info.GetSingle("value");
            int tangentMode = info.GetInt32("tangentMode");
            float inTangent = info.GetSingle("inTangent");
            float outTangent = info.GetSingle("outTangent");

            var frame = new Keyframe(time, value, inTangent, outTangent);
            frame.tangentMode = tangentMode;

            return frame;
        }
    }

    internal class AnimationCurveSurrogate : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            // TODO: fix the bugs... might be in SerializationInfo?
            Debug.LogError("AnimationCurve is not currently supported with FormatterSerializer");

            var curve = (AnimationCurve)obj;
            info.AddValue("keys", curve.keys, typeof(Keyframe[]));
            info.AddValue("preWrapMode", curve.preWrapMode, typeof(WrapMode));
            info.AddValue("postWrapMode", curve.postWrapMode, typeof(WrapMode));
        }

        private T SlowFind<T>(SerializationInfo info, string name) {
            foreach (var i in info) {
                if (i.Name == name) {
                    return (T)i.Value;
                }
            }

            throw new InvalidOperationException("Could not find value " + name);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
            //Debug.Log("SetObjectData for " + obj);
            //foreach (var i in info) {
            //    Debug.Log(i.Name + " => " + i.Value + " of type " + i.ObjectType);
            //}

            Keyframe[] keys = (Keyframe[])info.GetValue("keys", typeof(Keyframe[]));
            WrapMode preWrapMode = (WrapMode)info.GetValue("preWrapMode", typeof(WrapMode));
            WrapMode postWrapMode = SlowFind<WrapMode>(info, "postWrapMode");
            //(WrapMode)info.GetValue("postWrapMode ", typeof(WrapMode));

            //Debug.Log("done");

            var curve = new AnimationCurve(keys);
            curve.preWrapMode = preWrapMode;
            curve.postWrapMode = postWrapMode;
            return curve;
        }
    }

    internal class AnimationCurveWorker : IFormatterWorker {
        public void Work(DictionarySurrogateSelector surrogates, StreamingContext context) {
            surrogates.AddSurrogate(typeof(Keyframe), context, new KeyframeSurrogate());
            surrogates.AddSurrogate(typeof(AnimationCurve), context, new AnimationCurveSurrogate());
        }
    }
}