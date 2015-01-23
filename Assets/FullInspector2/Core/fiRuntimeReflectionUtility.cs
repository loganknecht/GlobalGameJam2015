using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Some reflection utilities that can be AOT compiled (and are therefore available at runtime).
    /// </summary>
    public class fiRuntimeReflectionUtility {
        /// <summary>
        /// Invokes the given static method on the given type.
        /// </summary>
        /// <param name="type">The type to search for the method.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The parameters to invoke the method with.</param>
        public static object InvokeStaticMethod(Type type, string methodName, object[] parameters) {
            try {
                return type.GetFlattenedMethod(methodName).Invoke(null, parameters);
            }
            catch { }
            return null;
        }
        public static object InvokeStaticMethod(string typeName, string methodName, object[] parameters) {
            return InvokeStaticMethod(TypeCache.FindType(typeName), methodName, parameters);
        }

        /// <summary>
        /// Invokes the given method on the given type.
        /// </summary>
        /// <param name="type">The type to find the method to invoke from.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="thisInstance">The "this" object in the method.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        public static void InvokeMethod(Type type, string methodName, object thisInstance, object[] parameters) {
            try {
                type.GetFlattenedMethod(methodName).Invoke(thisInstance, parameters);
            }
            catch { }
        }

        /// <summary>
        /// Returns a list of object instances from types in the assembly that implement the given
        /// type. This only constructs objects which have default constructors.
        /// </summary>
        public static IEnumerable<TInterface> GetAssemblyInstances<TInterface>() {
            return from assembly in GetRuntimeAssemblies()
                   from type in assembly.GetTypes()

                   where typeof(TInterface).IsAssignableFrom(type)
                   where type.GetConstructor(fsPortableReflection.EmptyTypes) != null

                   select (TInterface)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Returns all types that derive from UnityEngine.Object that are usable during runtime.
        /// </summary>
        public static IEnumerable<Type> GetUnityObjectTypes() {
            return from assembly in GetRuntimeAssemblies()

                   // GetExportedTypes() doesn't work for dynamic modules, so we jut use GetTypes()
                   // instead and manually filter for public
                   from type in assembly.GetTypes()
                   where type.Resolve().IsVisible

                   // Cannot be an open generic type
                   where type.Resolve().IsGenericTypeDefinition == false

                   where typeof(UnityObject).IsAssignableFrom(type)

                   select type;
        }

        /// <summary>
        /// Return a guess of all assemblies that can be used at runtime.
        /// </summary>
        public static IEnumerable<Assembly> GetRuntimeAssemblies() {
#if !UNITY_EDITOR && UNITY_METRO
            yield return typeof(Assembly).GetTypeInfo().Assembly;
#else
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()

                   let name = assembly.GetName().Name

                   // Unity exposes lots of assemblies that won't contain any behaviors that will
                   // contain a MonoBehaviour or UnityObject reference... so we ignore them to speed
                   // up reflection processing

                   where IsScriptOrCoreRuntimeAssembly(name) == true

                   // In the editor, even Assembly-CSharp, etc contain a reference to UnityEditor,
                   // so we can't strip the assembly that way
                   where name.Contains("-Editor") == false

                   select assembly;
#endif
        }

        /// <summary>
        /// Returns true if the given assembly is likely to contain user-scripts or it is a core
        /// runtime assembly (ie, UnityEngine).
        /// </summary>
        /// <param name="name">The unqualified name of the assembly.</param>
        public static bool IsScriptOrCoreRuntimeAssembly(string name) {
            var nonScript = new string[] {
                "UnityScript.Lang",
                "Boo.Lang.Parser",
                "Boo.Lang",
                "Boo.Lang.Compiler",
                "System.ComponentModel.DataAnnotations",
                "System.Xml.Linq",
                "ICSharpCode.NRefactory",
                "UnityScript",
                "Mono.Cecil",
                "nunit.framework",
                "UnityEditor.Android.Extensions",
                "UnityEditor.BB10.Extensions",
                "UnityEditor.Metro.Extensions",
                "UnityEditor.WP8.Extensions",
                "UnityEditor.iOS.Extensions",
                "UnityEditor.Graphs",
                "protobuf-net",
                "Newtonsoft.Json",
                "AssetStoreToolsExtra",
                "AssetStoreTools",
                "Unity.PackageManager",
                "Mono.Security",
                "System.Xml",
                "System.Configuration",
                "System",
                "Unity.IvyParser",
                "System.Core",
                "Unity.DataContract",
                "I18N.West",
                "I18N",
                "Unity.Locator",
                "UnityEditor",
                "mscorlib",
                "nunit.core",
                "nunit.core.interfaces",
                "Mono.Cecil.Mdb",
                "NSubstitute"
            };
            return nonScript.Contains(name) == false;
        }

        /// <summary>
        /// Returns all types in the current AppDomain that derive from the given baseType and are a
        /// class that is not an open generic type.
        /// </summary>
        public static IEnumerable<Type> AllSimpleTypesDerivingFrom(Type baseType) {
            return from assembly in GetRuntimeAssemblies()
                   from type in assembly.GetTypes()
                   where baseType.IsAssignableFrom(type)
                   where type.Resolve().IsClass
                   where type.Resolve().IsGenericTypeDefinition == false
                   select type;
        }
    }
}