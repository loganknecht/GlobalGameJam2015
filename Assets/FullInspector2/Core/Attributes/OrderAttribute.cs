using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Set the display order of an field or property of an object. A field or property without an
    /// order defaults to order double.MaxValue. Each inheritance level receives its own order
    /// group.
    /// </summary>
    [Obsolete("Use InspectorOrderAttribute instead", true)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OrderAttribute : Attribute {
        /// <summary>
        /// The ordering of this member relative to other ordered fields/properties.
        /// </summary>
        public double Order;

        /// <summary>
        /// Set the order.
        /// </summary>
        /// <param name="order">The order in which to display this field or property. A field or
        /// property without an OrderAttribute defaults to order 0.</param>
        public OrderAttribute(double order) {
            Order = order;
        }
    }
}