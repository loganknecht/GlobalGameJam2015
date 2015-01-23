using FullInspector.Internal;
using System;
using System.Runtime.Serialization;

namespace FullInspector.Serializers.Formatter {
    /// <summary>
    /// A more robust deserialization binder that will match types even if they don't match the
    /// assembly name.
    /// </summary>
    internal class RobustSerializationBinder : SerializationBinder {
        public override Type BindToType(string assemblyName, string typeName) {
            return TypeCache.FindType(typeName, assemblyName);
        }
    }
}