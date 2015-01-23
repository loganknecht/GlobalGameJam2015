using ProtoBuf.Meta;
using System;

namespace FullInspector.Serializers.ProtoBufNet {
    public interface IProtoModelWorker {
        void Work(RuntimeTypeModel model);
    }

    public abstract class ProtoModelWorker : IProtoModelWorker {
        protected void SetSurrogate<TModel, TSurrogate>(RuntimeTypeModel model) {
            SetSurrogate(model, typeof(TModel), typeof(TSurrogate));
        }

        protected void SetSurrogate(RuntimeTypeModel model, Type modelType, Type surrogateType) {
            model.Add(modelType, false).SetSurrogate(surrogateType);
        }

        public abstract void Work(RuntimeTypeModel model);
    }
}