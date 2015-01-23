using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace FullInspector.Internal {
    /// <summary>
    /// Some reflection utilities that are only available in the editor.
    /// </summary>
    public class fiEditorReflectionUtility {
        /// <summary>
        /// Returns a guess of all user-defined assemblies that are available in the editor, but not
        /// necessarily in the runtime.
        /// </summary>
        public static IEnumerable<Assembly> GetEditorAssemblies() {
            var assemblies = from assembly in AppDomain.CurrentDomain.GetAssemblies()

                             where fiRuntimeReflectionUtility.IsScriptOrCoreRuntimeAssembly(assembly.GetName().Name)
                             where ContainsAssemblyReference(assembly, typeof(EditorWindow).Assembly)

                             select assembly;

            return assemblies;
        }

        /// <summary>
        /// Returns true if the given assembly is referencing the given referenced assembly.
        /// </summary>
        public static bool ContainsAssemblyReference(Assembly assembly, Assembly referenced) {
            string editorAssemblyName = referenced.GetName().Name;
            foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies()) {
                if (referencedAssembly.Name == editorAssemblyName) {
                    return true;
                }
            }
            return false;
        }
    }
}