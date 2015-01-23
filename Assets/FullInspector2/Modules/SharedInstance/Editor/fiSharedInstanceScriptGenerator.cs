using FullInspector.Internal;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.SharedInstance {
    /// <summary>
    /// Generates derived types for SharedInstance{T} that do not have a generic parameter.
    /// </summary>
    public static class fiSharedInstanceScriptGenerator {
        public static void GenerateScript(Type instanceType) {
            // the name of the class, ie, SharedInstanceSystemInt32
            string className = instanceType.CSharpName();
            if (instanceType.Namespace != null && instanceType.Namespace != "System") {
                className = RemoveAll(instanceType.Namespace, '.') + className;
            }
            className = "SharedInstance_" + className;

            // the name of the type, ie, System.Int32
            string typeName = instanceType.CSharpName();
            if (instanceType.Namespace != null && instanceType.Namespace != "System") {
                typeName = instanceType.Namespace + "." + typeName;
            }

            Emit(className, typeName);
        }


        private static void Emit(string className, string typeName) {
            String directory = fiUtility.CombinePaths(fiSettings.RootDirectory, "Modules/SharedInstance/GeneratedScripts/");
            Directory.CreateDirectory(directory);
            String path = fiUtility.CombinePaths(directory, className + ".cs");
            if (File.Exists(path)) return;

            string script = "";
            script += "// This is an automatically generated script that is used to remove the generic " + Environment.NewLine;
            script += "// parameter from SharedInstance<T> so that Unity can properly serialize it." + Environment.NewLine;
            script += Environment.NewLine;
            script += "using System;" + Environment.NewLine;
            script += Environment.NewLine;
            script += "namespace FullInspector.Generated.SharedInstance {" + Environment.NewLine;
            script += "    public class " + className + " : SharedInstance<" + typeName + "> {}" + Environment.NewLine;
            script += "}" + Environment.NewLine;

            Debug.Log("Writing derived SharedInstance<" +  typeName + "> type (" + className + ") to " + path + "; click to see script below." +
                Environment.NewLine + Environment.NewLine + script);
            File.WriteAllText(path, script);
            AssetDatabase.Refresh();
        }

        private static string RemoveAll(string str, char c) {
            return str.Split(c).Aggregate((a, b) => a + b);
        }
    }
}