using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FullInspector.Serializers.Formatter {
    public interface IFormatterWorker {
        void Work(DictionarySurrogateSelector surrogates, StreamingContext context);
    }
}