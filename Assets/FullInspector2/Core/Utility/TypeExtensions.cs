// The MIT License (MIT)
//
// Copyright (c) 2013-2014 Jacob Dufault
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FullInspector.Internal {
    /// <summary>
    /// Extensions to the Type API.
    /// </summary>
    public static class TypeExtensions {
        /// <summary>
        /// Returns a pretty name for the type in the style of one that you'd see in C#.
        /// </summary>
        public static string CSharpName(this Type type) {
            // we special case some of the common type names
            if (type == typeof(void)) return "void";
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "string";

            // original code taken from http://stackoverflow.com/a/17376472
            string name = type.FullName;
            if (string.IsNullOrEmpty(type.Namespace) == false) {
                name = name.Substring(type.Namespace.Length + 1);
            }

            if (type.Resolve().IsGenericParameter) name = type.Name;

            if (type.Resolve().IsGenericType == false || name.IndexOf('`') < 0) {
                return name;
            }

            var sb = new StringBuilder();

            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                                            .Select(t => t.CSharpName())
                                            .ToArray()));
            sb.Append(">");

            return sb.ToString();
        }

        /// <summary>
        /// Returns true if the given type is nullable.
        /// </summary>
        /// <remarks>
        /// This *should* always return false if the type was fetched via GetType() according to the
        /// .NET docs at http://msdn.microsoft.com/en-us/library/ms366789.aspx.
        /// </remarks>
        public static bool IsNullableType(this Type type) {
            // TODO: consider generic structs that are nullable, but at the moment creating a
            //       generic nullable type causes an internal compiler error

            return
                type.Resolve().IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool CompareTypes(Type a, Type b) {
            // one of them is a definition, so make both of them a definition
            if ((a.Resolve().IsGenericType && b.Resolve().IsGenericType) &&
                (a.Resolve().IsGenericTypeDefinition || b.Resolve().IsGenericTypeDefinition)) {

                a = a.GetGenericTypeDefinition();
                b = b.GetGenericTypeDefinition();
            }

            return a == b;
        }

        /// <summary>
        /// Searches for a particular implementation of the given type inside of the type. This is
        /// particularly useful if the interface type is an open type, ie, typeof(IFace{}), because
        /// this method will then return IFace{} but with appropriate type parameters inserted.
        /// </summary>
        /// <param name="type">The base type to search for interface</param>
        /// <param name="parentType">The interface type to search for. Can be an open generic
        /// type.</param>
        /// <returns>The actual interface type that the type contains, or null if there is no
        /// implementation of the given interfaceType on type.</returns>
        public static bool HasParent(this Type type, Type parentType) {
            // a type does not have itself as a parent
            if (CompareTypes(type, parentType)) {
                return false;
            }

            // fast path: IsAssignableFrom returns true, so parentType is definitely a parent
            if (parentType.IsAssignableFrom(type)) {
                return true;
            }

            while (type != null) {
                if (CompareTypes(type, parentType)) {
                    return true;
                }

                foreach (var iface in type.GetInterfaces()) {
                    if (CompareTypes(iface, parentType)) {
                        return true;
                    }
                }

                type = type.Resolve().BaseType;
            }

            return false;
        }

        /// <summary>
        /// Searches for a particular implementation of the given interface type inside of the type.
        /// This is particularly useful if the interface type is an open type, ie, typeof(IFace{}),
        /// because this method will then return IFace{} but with appropriate type parameters
        /// inserted.
        /// </summary>
        /// <param name="type">The base type to search for interface</param>
        /// <param name="interfaceType">The interface type to search for. Can be an open generic
        /// type.</param>
        /// <returns>The actual interface type that the type contains, or null if there is no
        /// implementation of the given interfaceType on type.</returns>
        public static Type GetInterface(this Type type, Type interfaceType) {
            if (interfaceType.Resolve().IsGenericType && interfaceType.Resolve().IsGenericTypeDefinition == false) {
                throw new ArgumentException("GetInterface requires that if the interface " +
                    "type is generic, then it must be the generic type definition, not a " +
                    "specific generic type instantiation");
            };

            while (type != null) {
                foreach (var iface in type.GetInterfaces()) {
                    if (iface.Resolve().IsGenericType) {
                        if (interfaceType == iface.GetGenericTypeDefinition()) {
                            return iface;
                        }
                    }

                    else if (interfaceType == iface) {
                        return iface;
                    }
                }

                type = type.Resolve().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Returns true if the baseType is an implementation of the given interface type. The
        /// interface type can be generic.
        /// </summary>
        /// <param name="type">The type to search for an implementation of the given
        /// interface</param>
        /// <param name="interfaceType">The interface type to search for</param>
        /// <returns></returns>
        public static bool IsImplementationOf(this Type type, Type interfaceType) {
            if (interfaceType.Resolve().IsGenericType &&
                interfaceType.Resolve().IsGenericTypeDefinition == false) {
                
                throw new ArgumentException("IsImplementationOf requires that if the interface " +
                    "type is generic, then it must be the generic type definition, not a " +
                    "specific generic type instantiation");
            };

            if (type.Resolve().IsGenericType) {
                type = type.GetGenericTypeDefinition();
            }

            while (type != null) {
                foreach (var iface in type.GetInterfaces()) {
                    if (iface.Resolve().IsGenericType) {
                        if (interfaceType == iface.GetGenericTypeDefinition()) {
                            return true;
                        }
                    }

                    else if (interfaceType == iface) {
                        return true;
                    }
                }

                type = type.Resolve().BaseType;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the given attribute is defined on the given element.
        /// </summary>
        public static bool HasAttribute(this MemberInfo element, Type attributeType) {
            return GetAttribute(element, attributeType) != null;
        }

        /// <summary>
        /// Returns true if the given attribute is defined on the given element.
        /// </summary>
        public static bool HasAttribute<TAttribute>(this MemberInfo element) {
            return HasAttribute(element, typeof(TAttribute));
        }

        /// <summary>
        /// Fetches the given attribute from the given MemberInfo. This method applies caching
        /// and is allocation free (after caching has been performed).
        /// </summary>
        /// <param name="element">The MemberInfo the get the attribute from.</param>
        /// <param name="attributeType">The type of attribute to fetch.</param>
        /// <returns>The attribute or null.</returns>
        public static Attribute GetAttribute(this MemberInfo element, Type attributeType) {
            var query = new AttributeQuery {
                MemberInfo = element,
                AttributeType = attributeType
            };

            Attribute attribute;
            if (_cachedAttributeQueries.TryGetValue(query, out attribute) == false) {
                var attributes = element.GetCustomAttributes(attributeType, inherit: true);
                attribute = (Attribute)attributes.FirstOrDefault();
                _cachedAttributeQueries[query] = attribute;
            }

            return attribute;
        }

        /// <summary>
        /// Fetches the given attribute from the given MemberInfo.
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute to fetch.</typeparam>
        /// <param name="element">The MemberInfo to get the attribute from.</param>
        /// <returns>The attribute or null.</returns>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo element)
            where TAttribute : Attribute {

            return (TAttribute)GetAttribute(element, typeof(TAttribute));
        }
        private struct AttributeQuery {
            public MemberInfo MemberInfo;
            public Type AttributeType;
        }
        private static IDictionary<AttributeQuery, Attribute> _cachedAttributeQueries =
            new Dictionary<AttributeQuery, Attribute>(new AttributeQueryComparator());

        private class AttributeQueryComparator : IEqualityComparer<AttributeQuery> {
            public bool Equals(AttributeQuery x, AttributeQuery y) {
                return
                    x.MemberInfo == y.MemberInfo &&
                    x.AttributeType == y.AttributeType;
            }

            public int GetHashCode(AttributeQuery obj) {
                return
                    obj.MemberInfo.GetHashCode() +
                    (17 * obj.AttributeType.GetHashCode());
            }
        }
    }
}