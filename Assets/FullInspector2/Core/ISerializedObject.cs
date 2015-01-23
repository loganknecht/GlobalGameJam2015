using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// The API that the editor code needs to interact with UnityObjects.
    /// </summary>
    public interface ISerializedObject {
        /// <summary>
        /// Restore the last saved state.
        /// </summary>
        void RestoreState();

        /// <summary>
        /// Save the current state.
        /// </summary>
        void SaveState();

        /// <summary>
        /// This list contains a set of object references that were encountered during the
        /// serialization process in this object graph. These need to persist through a Unity
        /// serialization cycle.
        /// </summary>
        List<UnityObject> SerializedObjectReferences {
            get;
            set;
        }

        /// <summary>
        /// The serialized state for this UnityObject - the key values. The actual value is in
        /// SerializedStateValues at the same index.
        /// </summary>
        List<string> SerializedStateKeys {
            get;
            set;
        }

        /// <summary>
        /// The serialized state for this UnityObject - the actual values. The key for this value is
        /// in SerializedStateKeys at the same index.
        /// </summary>
        List<string> SerializedStateValues {
            get;
            set;
        }
    }

}