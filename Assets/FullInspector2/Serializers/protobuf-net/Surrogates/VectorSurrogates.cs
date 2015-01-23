using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    [ProtoContract]
    public class Vector2Surrogate {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        public static explicit operator Vector2(Vector2Surrogate surrogate) {
            return new Vector2(surrogate.x, surrogate.y);
        }

        public static explicit operator Vector2Surrogate(Vector2 vec) {
            return new Vector2Surrogate() {
                x = vec.x,
                y = vec.y
            };
        }
    }

    [ProtoContract]
    public class Vector3Surrogate {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        [ProtoMember(3)]
        public float z;

        public static explicit operator Vector3(Vector3Surrogate surrogate) {
            return new Vector3(surrogate.x, surrogate.y, surrogate.z);
        }

        public static explicit operator Vector3Surrogate(Vector3 vec) {
            return new Vector3Surrogate() {
                x = vec.x,
                y = vec.y,
                z = vec.z,
            };
        }
    }

    [ProtoContract]
    public class Vector4Surrogate {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        [ProtoMember(3)]
        public float z;

        [ProtoMember(4)]
        public float w;

        public static explicit operator Vector4(Vector4Surrogate surrogate) {
            return new Vector4(surrogate.x, surrogate.y, surrogate.z, surrogate.w);
        }

        public static explicit operator Vector4Surrogate(Vector4 vec) {
            return new Vector4Surrogate() {
                x = vec.x,
                y = vec.y,
                z = vec.z,
                w = vec.w
            };
        }
    }


    public class VectorModelWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            SetSurrogate<Vector2, Vector2Surrogate>(model);
            SetSurrogate<Vector3, Vector3Surrogate>(model);
            SetSurrogate<Vector4, Vector4Surrogate>(model);
        }
    }
}