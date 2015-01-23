using ProtoBuf;
using System;

namespace FullInspector.Serializers.ProtoBufNet {
    public class ProtoBufNetMetadata : fiISerializerMetadata {
        public Guid SerializerGuid {
            get {
                return new Guid("c74b6a96-c4b7-42e8-bfca-2fb8df8d263f");
            }
        }

        public Type SerializerType {
            get {
                return typeof(ProtoBufNetSerializer);
            }
        }

        public Type[] SerializationOptInAnnotationTypes {
            get {
                return new Type[] { 
                    typeof(ProtoMemberAttribute),
                };
            }
        }

        public Type[] SerializationOptOutAnnotationTypes {
            get {
                return new Type[] {
                    typeof(ProtoIgnoreAttribute)
                };
            }
        }
    }
}
