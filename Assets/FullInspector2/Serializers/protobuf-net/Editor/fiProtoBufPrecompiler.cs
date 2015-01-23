using FullInspector.Internal;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Process = System.Diagnostics.Process;

namespace FullInspector.Serializers.ProtoBufNet {
    public class fiProtoBufPrecompiler {
        private static Process _decompilationProcess;

        private static void UpdateProgress(string outputPath, string protobufAssemblyPath,
            float progress) {

            if (_decompilationProcess == null) {
                EditorUtility.ClearProgressBar();
                return;
            }

            else if (_decompilationProcess.HasExited) {
                string err = _decompilationProcess.StandardError.ReadToEnd();

                // An error has occurred. Report it.
                if (string.IsNullOrEmpty(err) == false) {
                    Debug.LogWarning("Error when running decompilation process:\n" + err);
                }

                // No error
                else {
                    Debug.Log("The protobuf-net serializer decompilation has finished. Please " +
                        "wait for Unity to finish recompilation. The decompiled C# file is " +
                        "viewable at " + outputPath);

                    // Reimport the new serializer
                    AssetDatabase.Refresh();
                }

                EditorUtility.ClearProgressBar();

                // Regardless of the error status, delete the precompiled serializer assembly
                try {
                    File.Delete(protobufAssemblyPath);
                }
                catch { }
            }
            else {
                EditorUtility.DisplayProgressBar("Running Decompiler", "Decompiling the protobuf-net precompiled serializer assembly into C#", progress);
                EditorApplication.delayCall += () => UpdateProgress(outputPath, protobufAssemblyPath, progress + 0.005f);
            }
        }

        private static string PrepareForArguments(List<string> strings) {
            var output = new StringBuilder();

            foreach (var str in strings) {
                output.Append('"');
                output.Append(str);
                output.Append('"');
                output.Append(' ');
            }

            return output.ToString();
        }

        /// <summary>
        /// Runs the decompiler. This is an async process.
        /// </summary>
        /// <param name="outputPath">The path to output the decompiled C# file.</param>
        /// <param name="assemblyPath">The path of the assembly to decompile.</param>
        /// <param name="searchDirectories">Directories that include DLL files that will be needed
        /// for reference resolution during decompilation.</param>
        private static void RunDecompiler(string outputPath, string assemblyPath, string[] searchDirectories) {
            string decompilerPath = fiProtoBufFileUtility.CreateDecompilerExePath();

            var output = new StringBuilder();
            output.AppendLine("Running Full Inspector C# Decompiler for protobuf-net. Click to see more information.");
            output.AppendLine("\tDecompiler Path: " + decompilerPath);
            output.AppendLine("\tOutput Path: " + outputPath);
            output.AppendLine("\tInput Assembly Path: " + assemblyPath);
            foreach (var searchDirectory in searchDirectories) {
                output.AppendLine("\tAssembly Search Directory: " + searchDirectory);
            }
            Debug.Log(output);

            List<string> argList = new List<string>() { outputPath, assemblyPath };
            argList.AddRange(searchDirectories);
            string args = PrepareForArguments(argList);

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo() {
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,

                UseShellExecute = false,
                RedirectStandardError = true,

                WorkingDirectory = Directory.GetCurrentDirectory()
            };

#if UNITY_EDITOR
#if UNITY_EDITOR_WIN
            // windows platform
            startInfo.FileName = decompilerPath;
            startInfo.Arguments = args;
#elif UNITY_EDITOR_OSX
            // osx platform
            // On OSX, we need to invoke the decompiler using mono, not just as a raw executable
            startInfo.FileName = "mono";
            startInfo.Arguments = decompilerPath + " " + args;
#else
#error Unknown editor platform
#endif
#endif
            try {
                _decompilationProcess = Process.Start(startInfo);
            }
            catch (Exception e) {
                string msg = "Failed to start decompilation process.";
#if UNITY_EDITOR_OSX
                msg += " Is mono installed and available from the command line?";
#endif
                Debug.Log(msg + " Exception is:\n" + e);
            }

            EditorApplication.delayCall += () => UpdateProgress(outputPath, assemblyPath, 0);
        }

        public static void PrecompileSerializer(string outputPath) {
            if (EditorApplication.isCompiling) {
                Debug.LogWarning("The precompiled serializer canot be created while Unity is " +
                    "compiling code. Please try again once compilation has finished.");
                return;
            }
            if (_decompilationProcess != null) {
                Debug.LogWarning("A prior precompiled serializer process is still running.");
                return;
            }

            string TypeName = ProtoBufNetSettings.PrecompiledSerializerTypeName;
            string DllPath = TypeName + ".dll";

            var options = new RuntimeTypeModel.CompilerOptions {
                TypeName = TypeName,
                OutputPath = DllPath,
                ImageRuntimeVersion = Assembly.GetExecutingAssembly().ImageRuntimeVersion,
                MetaDataVersion = 0x20000, // use .NET 2 onwards
                Accessibility = RuntimeTypeModel.Accessibility.Public
            };

            // Create the precompiled serializer
            var model = TypeModelCreator.CreateModel();
            try {
                model.Compile(options);

                var output = new StringBuilder();
                output.AppendLine("Created protobuf-net serialization DLL (at " +
                    Path.GetFullPath(DllPath) + "). It contains serialization data for the " +
                    "following types:");
                foreach (var modelType in model.GetTypes()) {
                    output.Append('\t');
                    output.AppendLine(modelType.ToString());
                }
                Debug.Log(output.ToString());
            }
            catch (Exception) {
                Debug.LogError("Make sure to compile to protobuf-net DLL while the editor is " +
                    "not in AOT mode");
                throw;
            }

            // Run the decompiler
            var searchPaths = new List<string>() {
                Path.GetDirectoryName(typeof(CommonBaseBehavior).Assembly.Location), // Scripts (Assembly-CSharp and the like)
                Path.GetDirectoryName(typeof(UnityEngine.Object).Assembly.Location), // UnityEngine.dll
            };
            searchPaths.AddRange(from string directory in Directory.GetDirectories("Assets" + Path.DirectorySeparatorChar, "*", SearchOption.AllDirectories)
                                 where (directory.Contains("/.") || directory.Contains("\\.")) == false
                                 where Directory.GetFiles(directory, "*.dll").Count() > 0
                                 select Path.GetFullPath(directory));

            DllPath = Path.GetFullPath(DllPath);
            outputPath = Path.GetFullPath(outputPath);

            RunDecompiler(outputPath, DllPath, searchPaths.ToArray());
        }
    }
}