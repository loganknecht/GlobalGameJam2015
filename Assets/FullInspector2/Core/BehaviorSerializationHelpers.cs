using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Implements the core serialization API that can be used for wrapping Unity serialization.
    /// </summary>
    public static class BehaviorSerializationHelpers {
        /// <summary>
        /// Serializes the current state of the given object.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use for the serialization
        /// process.</typeparam>
        /// <param name="obj">The object that should be serialized.</param>
        public static void SaveState<TSerializer>(ISerializedObject obj)
            where TSerializer : BaseSerializer {

            // short-circuit for null serializer
            if (typeof(TSerializer) == typeof(NullSerializer)) return;

            // ensure the parameter derives from UnityObject
            UnityObject unityObj = obj as UnityObject;
            if (unityObj == null) {
                throw new InvalidOperationException("SerializeObject requires the obj parameter " +
                    "to derive from UnityEngine.Object");
            }

            var callbacks = obj as ISerializationCallbacks;
            if (callbacks != null) callbacks.OnBeforeSerialize();

            // fetch the selected serializer
            var serializer = fiSingletons.Get<TSerializer>();

            // setup the serialization operator
            var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = new List<UnityObject>();

            // get the properties that we will be serializing
            var properties = InspectedType.Get(obj.GetType()).GetProperties(InspectedMemberFilters.FullInspectorSerializedProperties);

            var serializedKeys = new List<string>();
            var serializedValues = new List<string>();

            for (int i = 0; i < properties.Count; ++i) {
                InspectedProperty property = properties[i];
                object currentValue = property.Read(obj);

                try {
                    if (currentValue == null) {
                        serializedKeys.Add(property.Name);
                        serializedValues.Add(null);
                    }
                    else {
                        var serializedState = serializer.Serialize(property.MemberInfo, currentValue, serializationOperator);
                        serializedKeys.Add(property.Name);
                        serializedValues.Add(serializedState);
                    }
                }
                catch (Exception e) {
                    Debug.LogError("Exception caught when serializing property <" +
                        property.Name + "> in <" + obj + "> with value " + currentValue + "\n" +
                        e);
                }
            }

            // Write the updated data out to the object.
            obj.SerializedStateKeys = serializedKeys;
            obj.SerializedStateValues = serializedValues;
            obj.SerializedObjectReferences = serializationOperator.SerializedObjects;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(unityObj);
#endif
        }

        /// <summary>
        /// Deserializes an object that has been serialized.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer that was used to serialize the
        /// object.</typeparam>
        /// <param name="obj">The object that will be restored from its serialized state.</param>
        public static void RestoreState<TSerializer>(ISerializedObject obj)
            where TSerializer : BaseSerializer {

            // short-circuit for null serializer
            if (typeof(TSerializer) == typeof(NullSerializer)) return;

            // ensure the parameter derives from UnityObject
            if (obj is UnityObject == false) {
                throw new InvalidOperationException("RestoreState requires the obj parameter " +
                    "to derive from UnityEngine.Object");
            }

            // ensure references are initialized
            if (obj.SerializedStateKeys == null) {
                obj.SerializedStateKeys = new List<string>();
            }
            if (obj.SerializedStateValues == null) {
                obj.SerializedStateValues = new List<string>();
            }
            if (obj.SerializedObjectReferences == null) {
                obj.SerializedObjectReferences = new List<UnityObject>();
            }

            // try to verify that no data corruption occurred
            if (obj.SerializedStateKeys.Count != obj.SerializedStateValues.Count) {
                if (fiSettings.EmitWarnings) {
                    Debug.LogWarning("Serialized key count does not equal value count; possible " +
                        "data corruption / bad manual edit?", (UnityObject)obj);
                }
            }

            // there is nothing to deserialize
            if (obj.SerializedStateKeys.Count == 0) {
                if (fiSettings.AutomaticReferenceInstantation) {
                    InstantiateReferences(obj);
                }
                return;
            }

            // fetch the selected serializer
            var serializer = fiSingletons.Get<TSerializer>();

            // setup the serialization operator setup the serialization operator
            var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = obj.SerializedObjectReferences;

            // get the properties that we will be serializing
            var inspectedType = InspectedType.Get(obj.GetType());

            for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                var name = obj.SerializedStateKeys[i];
                var state = obj.SerializedStateValues[i];

                var property = inspectedType.GetPropertyByName(name);
                if (property == null) {
                    if (fiSettings.EmitWarnings) {
                        Debug.LogWarning("Unable to find serialized property with name=" + name +
                            " on type " + obj.GetType(), (UnityObject)obj);
                    }
                    continue;
                }

                object restoredValue = null;

                if (string.IsNullOrEmpty(state) == false) {
                    try {
                        restoredValue = serializer.Deserialize(property.MemberInfo, state,
                            serializationOperator);
                    }
                    catch (Exception e) {
                        Debug.LogError("Exception caught when deserializing property <" + name +
                            "> in <" + obj + ">\n" + e, (UnityObject)obj);
                    }
                }

                // sigh... CompareBaseObjectsExternal exception is thrown when we write a null
                // UnityObject reference in a non-primary thread using reflection
                if (ReferenceEquals(restoredValue, null) &&
                    typeof(UnityObject).IsAssignableFrom(property.StorageType)) {

                    continue;
                }

                property.Write(obj, restoredValue);
            }

            var callbacks = obj as ISerializationCallbacks;
            if (callbacks != null) callbacks.OnAfterDeserialize();
        }

        /// <summary>
        /// Instantiates all of the references in the given object.
        /// </summary>
        /// <param name="obj">The object to instantiate references in.</param>
        /// <param name="metadata">The (cached) metadata for the object.</param>
        private static void InstantiateReferences(object obj, InspectedType metadata = null) {
            if (metadata == null) {
                metadata = InspectedType.Get(obj.GetType());
            }

            // we don't want to do anything with collections
            if (metadata.IsCollection) {
                return;
            }

            var inspectedProperties = metadata.GetProperties(InspectedMemberFilters.InspectableMembers);
            for (int i = 0; i < inspectedProperties.Count; ++i) {
                var property = inspectedProperties[i];

                // this type is a reference, so we might need to instantiate it
                if (property.StorageType.Resolve().IsClass) {
                    // cannot allocate an instance for abstract types
                    if (property.StorageType.Resolve().IsAbstract) {
                        continue;
                    }
                    // check to see if the property already has a value; if it does, then skip it
                    object current = property.Read(obj);
                    if (current != null) {
                        continue;
                    }

                    // the property is null; we need to instantiate a new value for it
                    var propertyMetadata = InspectedType.Get(property.StorageType);

                    // the value cannot be created using the default constructor (ie, it has only
                    // one constructor that takes parameters); we cannot initialize an instance that
                    // is guaranteed to be in a valid state
                    if (propertyMetadata.HasDefaultConstructor == false) {
                        continue;
                    }

                    object instance = propertyMetadata.CreateInstance();
                    property.Write(obj, instance);

                    // recursively create instances
                    InstantiateReferences(instance, propertyMetadata);
                }
            }
        }
    }
}