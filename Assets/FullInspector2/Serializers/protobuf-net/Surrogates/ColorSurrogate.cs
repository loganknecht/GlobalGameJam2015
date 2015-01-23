using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    [ProtoContract]
    public class ColorSurrogate {
        [ProtoMember(1)]
        public float r;

        [ProtoMember(2)]
        public float g;

        [ProtoMember(3)]
        public float b;

        [ProtoMember(4)]
        public float a;

        public static explicit operator Color(ColorSurrogate surrogate) {
            return new Color(surrogate.r, surrogate.g, surrogate.b, surrogate.a);
        }

        public static explicit operator ColorSurrogate(Color color) {
            return new ColorSurrogate() {
                r = color.r,
                g = color.g,
                b = color.b,
                a = color.a
            };
        }
    }

    public class ColorModelWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            SetSurrogate<Color, ColorSurrogate>(model);
        }
    }
}