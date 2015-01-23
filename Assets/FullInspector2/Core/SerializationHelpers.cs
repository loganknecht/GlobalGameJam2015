using FullInspector.Internal;
using FullSerializer.Internal;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    public static class SerializationHelpers {
        /// <summary>
        /// Deserialize a value from the given content.
        /// </summary>
        /// <typeparam name="T">The type of value to deserialize.</typeparam>
        /// <typeparam name="TSerializer">The serializer to use.</typeparam>
        /// <param name="content">The serialized content to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeFromContent<T, TSerializer>(string content)
            where TSerializer : BaseSerializer {

            var serializer = fiSingletons.Get<TSerializer>();
            var notSupportedOperator = fiSingletons.Get<NotSupportedSerializationOperator>();

            return (T)serializer.Deserialize(fsPortableReflection.AsMemberInfo(typeof(T)), content, notSupportedOperator);
        }

        /// <summary>
        /// Serialize the given value to a string. The given value cannot contain any references
        /// that derive from UnityEngine.Object.
        /// </summary>
        /// <typeparam name="T">The type of value to serialize.</typeparam>
        /// <typeparam name="TSerializer">The serializer to use.</typeparam>
        /// <param name="value">The actual value to serialize.</param>
        /// <returns>The serialized value state.</returns>
        public static string SerializeToContent<T, TSerializer>(T value)
            where TSerializer : BaseSerializer {

            var serializer = fiSingletons.Get<TSerializer>();
            var notSupportedOperator = fiSingletons.Get<NotSupportedSerializationOperator>();

            return serializer.Serialize(fsPortableReflection.AsMemberInfo(typeof(T)), value, notSupportedOperator);
        }

        /// <summary>
        /// Clones the given object using the selected serializer. In essence, this just runs the
        /// object through the serialization process and then deserializes it.
        /// </summary>
        /// <typeparam name="T">The type of object to clone.</typeparam>
        /// <typeparam name="TSerializer">The serializer to do the cloning with.</typeparam>
        /// <param name="obj">The object to clone.</param>
        /// <returns>A duplicate of the given object.</returns>
        public static T Clone<T, TSerializer>(T obj)
            where TSerializer : BaseSerializer {

            var serializer = fiSingletons.Get<TSerializer>();
            var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = new List<UnityObject>();

            string serialized = serializer.Serialize(fsPortableReflection.AsMemberInfo(typeof(T)), obj, serializationOperator);
            object deserialized = serializer.Deserialize(fsPortableReflection.AsMemberInfo(typeof(T)), serialized, serializationOperator);

            serializationOperator.SerializedObjects = null;

            return (T)deserialized;
        }
    }
}