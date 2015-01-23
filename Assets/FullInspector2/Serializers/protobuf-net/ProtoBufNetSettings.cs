using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    /// <summary>
    /// This type contains settings for protobuf-net.
    /// </summary>
    public static class ProtoBufNetSettings {
        /// <summary>
        /// The type name of the precompiled serializer. The serializer will be compiled into a DLL
        /// of the same name.
        /// </summary>
        public const string PrecompiledSerializerTypeName = "ProtoBufNetPrecompiledSeriaizer";

        /// <summary>
        /// Should the precompiled DLL be automatically recompiled whenever a code compilation is
        /// detected?
        /// </summary>
        public const bool AutomaticDllRecompilation = true;
    }
}