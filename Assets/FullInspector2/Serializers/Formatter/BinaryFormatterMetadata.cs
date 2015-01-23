using System;

namespace FullInspector.Serializers.Formatter {
    public class BinaryFormatterMetadata : fiISerializerMetadata {
        public Guid SerializerGuid {
            get { return new Guid("f3b35090-2cf3-4f89-845d-3743be7571fa"); }
        }

        public Type SerializerType {
            get { return typeof(BinaryFormatterSerializer); }
        }

        public Type[] SerializationOptInAnnotationTypes {
            get { return new Type[] { }; }
        }

        public Type[] SerializationOptOutAnnotationTypes {
            get { return new Type[] { }; }
        }
    }
}