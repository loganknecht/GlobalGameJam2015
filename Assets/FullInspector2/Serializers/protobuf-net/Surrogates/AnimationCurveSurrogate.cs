using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    [ProtoContract]
    public class KeyframeSurrogate {
        [ProtoMember(1)]
        public float time;

        [ProtoMember(2)]
        public float value;

        [ProtoMember(3)]
        public int tangentMode;

        [ProtoMember(4)]
        public float inTangent;

        [ProtoMember(5)]
        public float outTangent;

        public static explicit operator Keyframe(KeyframeSurrogate surrogate) {
            var frame = new Keyframe(surrogate.time, surrogate.value, surrogate.inTangent,
                surrogate.outTangent);
            frame.tangentMode = surrogate.tangentMode;
            return frame;
        }

        public static explicit operator KeyframeSurrogate(Keyframe frame) {
            return new KeyframeSurrogate() {
                time = frame.time,
                value = frame.value,
                tangentMode = frame.tangentMode,
                inTangent = frame.inTangent,
                outTangent = frame.outTangent
            };
        }
    }

    [ProtoContract]
    public class AnimationCurveSurrogate {
        [ProtoMember(1)]
        public Keyframe[] keys;

        [ProtoMember(2)]
        public WrapMode preWrapMode;

        [ProtoMember(3)]
        public WrapMode postWrapMode;

        public static explicit operator AnimationCurve(AnimationCurveSurrogate surrogate) {
            if (surrogate == null) return null;

            var curve = new AnimationCurve(surrogate.keys);
            curve.preWrapMode = surrogate.preWrapMode;
            curve.postWrapMode = surrogate.postWrapMode;
            return curve;
        }

        public static explicit operator AnimationCurveSurrogate(AnimationCurve curve) {
            if (curve == null) return null;

            return new AnimationCurveSurrogate() {
                keys = curve.keys,
                postWrapMode = curve.postWrapMode,
                preWrapMode = curve.preWrapMode
            };
        }
    }

    public class AnimationKeyframeWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            SetSurrogate<AnimationCurve, AnimationCurveSurrogate>(model);
            SetSurrogate<Keyframe, KeyframeSurrogate>(model);
        }
    }
}