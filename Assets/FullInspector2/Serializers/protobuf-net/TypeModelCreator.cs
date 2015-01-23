using FullInspector.Internal;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FullInspector.Serializers.ProtoBufNet {
    /// <summary>
    /// Manages the dynamic construction of a protobuf-net TypeModel that is used for serialization.
    /// </summary>
    public static class TypeModelCreator {
        public static List<string> AssemblyIgnoreList = new List<string> {
            "ShaderForge"
        };

        private static bool IsAssemblyAllowed(Assembly assembly) {
            return AssemblyIgnoreList.Contains(assembly.GetName().Name) == false;
        }

        public static bool IsInIgnoredAssembly(Type type) {
            return IsAssemblyAllowed(type.Assembly) == false;
        }

        /// <summary>
        /// Types which have a [ProtoContract] attribute.
        /// </summary>
        private static IEnumerable<Type> GetContracts(RuntimeTypeModel model) {
            return from assembly in fiRuntimeReflectionUtility.GetRuntimeAssemblies()
                   where IsAssemblyAllowed(assembly)

                   from contractType in assembly.GetTypes()

                   where contractType.GetAttribute<ProtoContractAttribute>() != null

                   // Generic contract types are useless by themselves; they are only used when
                   // requested by some other type that is being serialized, which in that case they
                   // will be instantiated with generic type parameters
                   where contractType.IsGenericTypeDefinition == false

                   // Ignore types that have already been added to the model. For example, we
                   // ignore types with a surrogate
                   where ContainsType(model, contractType) == false

                   select contractType;
        }

        private static Type RunSanityCheck(Type type) {
            if ((type.IsPublic || type.IsNestedPublic) == false) {
                Throw("sanity check -- type is not public for type={0}", type);
            }

            if (type.IsGenericTypeDefinition) {
                Throw("sanity check -- type is a generic definition for type={0}", type);
            }

            return type;
        }

        public static RuntimeTypeModel CreateModel() {
            var model = TypeModel.Create();

            // We want protobuf-net to serialize default values. Sometimes, protobuf-net will skip
            // serialization when it really shouldn't.
            //
            // An example is a nullable struct; the nullable struct can contain a value, but if
            // that value is the default one then protobuf-net will skip serialization, resulting
            // in a null nullable type after deserialization.
            model.UseImplicitZeroDefaults = false;

            // custom model workers
            foreach (IProtoModelWorker worker in
                fiRuntimeReflectionUtility.GetAssemblyInstances<IProtoModelWorker>()) {
                worker.Work(model);
            }

            // regular old [ProtoContract] types
            foreach (Type contract in GetContracts(model)) {
                if (contract.IsVisible == false) {
                    Throw("A ProtoContract type needs to have public visibility for contract={0}",
                        contract);
                }

                model.Add(RunSanityCheck(contract), true);
            }

            // Fields and properties on UnityObject derived types, such as BaseBehavior, are not
            // annotated with serialization annotations. This means that for protobuf-net, if a
            // BaseBehavior contains a Dictionary{string,string} field, then that specific generic
            // instantiation may not be discovered, as the field does not serve as an anchor.
            //
            // In this loop, we go through all UnityObject derived types and ensure that every
            // serialized property is in the RuntimeTypeModel.
            foreach (var behaviorType in fiRuntimeReflectionUtility.GetUnityObjectTypes()) {
                // We only want UnityObject types are serializable by ProtoBufNet
                // TODO: support custom base classes
                if (typeof(BaseBehavior<ProtoBufNetSerializer>).IsAssignableFrom(behaviorType) == false) {
                    continue;
                }

                var serializedProperties = InspectedType.Get(behaviorType).GetProperties(InspectedMemberFilters.FullInspectorSerializedProperties);
                foreach (InspectedProperty property in serializedProperties) {
                    // If the property is generic and the model currently doesn't contain it, make
                    // sure we add it to the model.
                    if (property.StorageType.IsGenericType &&
                        ContainsType(model, property.StorageType) == false) {

                        model.Add(property.StorageType, true);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Returns true if the given RuntimeTypeModel contains a definition for the given type.
        /// </summary>
        private static bool ContainsType(RuntimeTypeModel model, Type type) {
            bool emit = !type.IsNullableType();
            bool fastCheck = model.IsDefined(type);

            // DEBUGGING: comment this if statement out if there are protobuf-net related issues
            if (emit) {
                return fastCheck;
            }

            foreach (MetaType metaType in model.GetTypes()) {
                if (metaType.Type == type) {
                    if (emit && fastCheck != true) UnityEngine.Debug.Log("Fast check failed for " + type.CSharpName() + " (nullabe? " + type.IsNullableType() + ")!");
                    return true;
                }
            }

            if (emit && fastCheck != false) UnityEngine.Debug.Log("Fast check failed for " + type.CSharpName() + " (nullabe? " + type.IsNullableType() + ")!");
            return false;
        }

        /// <summary>
        /// Throws an InvalidOperationException.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="parameters">The parameters in the format string.</param>
        private static void Throw(string format, params object[] parameters) {
            throw new InvalidOperationException(string.Format(format, parameters));
        }
    }
}