using System;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Draws the regular property editor but with a disabled GUI. With the current implementation
    /// this is not compatible with other attribute editors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDisabledAttribute : Attribute {
    }
}