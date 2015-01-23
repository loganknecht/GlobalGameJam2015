using UnityEditor;

namespace FullInspector.Serializers.ProtoBufNet {
    public class fiProtoBufNetMenus {
        [MenuItem("Window/Full Inspector/Developer/Create protobuf-net precompiled serializer (AOT platforms, beta)")]
        public static void PrecompileSerializer() {
            string OutputPath = "Assets/" + ProtoBufNetSettings.PrecompiledSerializerTypeName + ".cs";
            fiProtoBufPrecompiler.PrecompileSerializer(OutputPath);
        }
    }
}