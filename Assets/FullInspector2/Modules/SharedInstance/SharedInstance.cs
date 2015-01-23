namespace FullInspector {
    /// <summary>
    /// A SharedInstance{T} contains an instance of type T whose instance is shared across multiple MonoBehaviour instances.
    /// </summary>
    /// <typeparam name="TInstance">The object type to store.</typeparam>
    public class SharedInstance<TInstance> : BaseScriptableObject {
        /// <summary>
        /// The shared object instance.
        /// </summary>
        public TInstance Instance;
    }
}