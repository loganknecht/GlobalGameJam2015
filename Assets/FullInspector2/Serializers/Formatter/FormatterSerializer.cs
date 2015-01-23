using FullInspector.Internal;
using FullInspector.Serializers.Formatter;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FullInspector {
    /// <summary>
    /// Predefine the BinaryFormatterSerializer for convenience. This is just a FormatterSerialize
    /// using a BinaryFormatter.
    /// </summary>
    public class BinaryFormatterSerializer : FormatterSerializer<BinaryFormatter> { }

    /// <summary>
    /// Provides Full Inspector integration for IFormatter serializer types. Typically the
    /// IFormatter instance will be BinaryFormatter.
    /// </summary>
    /// <typeparam name="TFormatter">The type of IFormatter to use. Typically this will be
    /// BinaryFormatter.</typeparam>
    public class FormatterSerializer<TFormatter> : BaseSerializer
        where TFormatter : IFormatter, new() {

        private static TFormatter _formatter;

        static FormatterSerializer() {
            _formatter = new TFormatter();
            _formatter.Binder = new RobustSerializationBinder();

            var surrogates = new DictionarySurrogateSelector();
            _formatter.SurrogateSelector = surrogates;

            var context = new StreamingContext(StreamingContextStates.All);

            foreach (var worker in fiRuntimeReflectionUtility.GetAssemblyInstances<IFormatterWorker>()) {
                worker.Work(surrogates, context);
            }

            // force BinaryFormatter to go through reflection, which allows it to work on iOS
#if UNITY_IPHONE
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        }

        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            if (value == null) return "null";

            _formatter.Context = new StreamingContext(StreamingContextStates.All, serializationOperator);

            using (var stream = new MemoryStream()) {
                _formatter.Serialize(stream, value);
                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
            }

        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            if (serializedState == "null") return null;

            _formatter.Context = new StreamingContext(StreamingContextStates.All, serializationOperator);

            byte[] bytes = Convert.FromBase64String(serializedState);
            using (var stream = new MemoryStream(bytes)) {
                return _formatter.Deserialize(stream);
            }
        }
    }
}