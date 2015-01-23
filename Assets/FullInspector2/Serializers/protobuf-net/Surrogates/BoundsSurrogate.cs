using ProtoBuf;
using ProtoBuf.Meta;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    [ProtoContract]
    public class BoundsSurrogate {
        [ProtoMember(1)]
        public Vector3 center;

        [ProtoMember(2)]
        public Vector3 size;

        public static explicit operator Bounds(BoundsSurrogate surrogate) {
            return new Bounds(surrogate.center, surrogate.size);
        }

        public static explicit operator BoundsSurrogate(Bounds bounds) {
            return new BoundsSurrogate() {
                center = bounds.center,
                size = bounds.size
            };
        }
    }

    public class BoundsModelWorker : ProtoModelWorker {
        public override void Work(RuntimeTypeModel model) {
            SetSurrogate<Bounds, BoundsSurrogate>(model);
        }
    }
}