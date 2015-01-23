using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    [ProtoContract]
    public class GradientAlphaKeySurrogate {
        [ProtoMember(1)]
        public float Alpha;
        [ProtoMember(2)]
        public float Time;

        public static explicit operator GradientAlphaKey(GradientAlphaKeySurrogate surrogate) {
            return new GradientAlphaKey {
                alpha = surrogate.Alpha,
                time = surrogate.Time
            };
        }
        public static explicit operator GradientAlphaKeySurrogate(GradientAlphaKey gradient) {
            return new GradientAlphaKeySurrogate {
                Alpha = gradient.alpha,
                Time = gradient.time
            };
        }
    }

    [ProtoContract]
    public class GradientColorKeySurrogate {
        [ProtoMember(1)]
        public Color Color;
        [ProtoMember(2)]
        public float Time;

        public static explicit operator GradientColorKey(GradientColorKeySurrogate surrogate) {
            return new GradientColorKey {
                color = surrogate.Color,
                time = surrogate.Time
            };
        }
        public static explicit operator GradientColorKeySurrogate(GradientColorKey gradient) {
            return new GradientColorKeySurrogate {
                Color = gradient.color,
                Time = gradient.time
            };
        }
    }

    [ProtoContract]
    public class GradientSurrogate {
        [ProtoMember(1)]
        public GradientColorKey[] ColorKeys;

        [ProtoMember(2)]
        public GradientAlphaKey[] AlphaKeys;

        public static explicit operator Gradient(GradientSurrogate surrogate) {
            return new Gradient {
                alphaKeys = surrogate.AlphaKeys,
                colorKeys = surrogate.ColorKeys
            };
        }

        public static explicit operator GradientSurrogate(Gradient gradient) {
            if (gradient == null) return null;

            return new GradientSurrogate() {
                AlphaKeys = gradient.alphaKeys,
                ColorKeys = gradient.colorKeys
            };
        }
    }

    public class GradientModelWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            SetSurrogate<GradientAlphaKey, GradientAlphaKeySurrogate>(model);
            SetSurrogate<GradientColorKey, GradientColorKeySurrogate>(model);
            SetSurrogate<Gradient, GradientSurrogate>(model);
        }
    }
}