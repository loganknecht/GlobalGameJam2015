namespace FullInspector {
    /// <summary>
    /// Extend this interface on any BaseBehavior or BaseScriptableObject type to receive callbacks
    /// for when Full Inspector runs serialization. All callbacks will occur on the main Unity
    /// thread, so API calls are fine.
    /// </summary>
    /// <remarks>
    /// These functions will *not* get invoked if the type does not extend BaseScriptableObject
    /// or BaseBehavior! Use the serializer-specific callbacks for that.
    /// </remarks>
    public interface ISerializationCallbacks {
        /// <summary>
        /// Called right before FI runs serialization.
        /// </summary>
        void OnBeforeSerialize();

        /// <summary>
        /// Called right after FI runs deserialization.
        /// </summary>
        void OnAfterDeserialize();
    }
}