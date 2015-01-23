using System.IO;

namespace FullInspector.Serializers.ProtoBufNet {
    public static class fiProtoBufFileUtility {
        public static string CreateDecompilerExePath() {
            string tempDir = GetTempDirectory();
            Directory.CreateDirectory(tempDir);

            CopyDirectory(Path.Combine(fiSettings.RootDirectory, "Serializers/protobuf-net/Decompiler/Executables"), tempDir);
            return Path.Combine(tempDir, "Decompiler.exe");
        }

        private static string GetTempDirectory() {
            string dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            while (File.Exists(dir) || Directory.Exists(dir)) {
                dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }

            return dir;
        }

        private static void CopyDirectory(string srcDir, string destDir) {
            foreach (string srcFile in Directory.GetFiles(srcDir)) {
                if (srcFile.EndsWith(".meta")) continue;

                string destFile = Path.Combine(destDir, Path.GetFileName(srcFile));
                if (destFile.EndsWith("_fakeextension")) {
                    destFile = destFile.Substring(0, destFile.Length - "_fakeextension".Length);
                }

                File.Copy(srcFile, destFile);
            }
        }
    }
}