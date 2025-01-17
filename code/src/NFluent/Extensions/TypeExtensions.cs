﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TypeExtensions.cs" company="NFluent">
//   Copyright 2020 by Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NFluent.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TypeExtensions
    {
        private const BindingFlags BindingFlagsAll =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type[] SignedTypesOrder =
            {typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)};

        private static readonly Type[] UnsignedTypesOrder =
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint),
            typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)
        };

        /// <summary>
        /// Checks if a type has at least one attribute of a give type.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <param name="attribute">Attribute type to check for.</param>
        /// <returns>True if <paramref name="type"/> cref="type"/> has a least one attribute of type <paramref name="attribute"/>, false otherwise.</returns>
        public static bool TypeHasAttribute(this Type type, Type attribute) =>
            type.GetTypeInfo().GetCustomAttributes(false)
                .Any(customAttribute => customAttribute.GetType() == attribute);
                
        /// <summary>
        /// Checks if an anonymous type.
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if <see paramref="type"/> is anonymous.</returns>
        public static bool TypeIsAnonymous(this Type type) => type.Name.Contains("Anonymous") && type.TypeHasAttribute(typeof(CompilerGeneratedAttribute));

        /// <summary>
        /// Checks if a type possesses at least a field or a property.
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <returns>true if the type as at least one field or property</returns>
        public static bool TypeHasMember(this Type type) => !type.GetTypeInfo().IsEnum && (type.GetFields(BindingFlagsAll).Any() || type.GetProperties(BindingFlagsAll).Any());

        /// <summary>
        /// Returns true if the provided type implements IEnumerable, disregarding well known enumeration (string).
        /// </summary>
        /// <param name="type">type to assess</param>
        /// <param name="evenWellKnown">treat well known enumerations (string) as enumeration as well</param>
        /// <returns>true if <see paramref="type" /> should treated as an enumeration.</returns>
        public static bool IsAnEnumeration(this Type type, bool evenWellKnown) => type.GetInterfaces().Contains(typeof(IEnumerable)) && (evenWellKnown || type != typeof(string));

        /// <summary>
        /// Type is an ISet implementation.
        /// </summary>
        /// <param name="type">type to assess</param>
        /// <returns>true if type is an ISet implementation.</returns>
        public static bool IsAList(this Type type) => type.GetInterfaces().Any(t => t == typeof(IList));

        /// <summary>
        /// Returns true if the type is a generic type
        /// </summary>
        /// <param name="type">type to asses</param>
        /// <returns>true if <see paramref="type" /> is a generic type.</returns>
        public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

        /// <summary>
        /// Checks if a type is numerical (i.e: int, double, short, uint...).
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>true if the type is a numerical type.</returns>
        public static bool IsNumerical(this Type type) => UnsignedTypesOrder.Contains(type);

        /// <summary>
        /// Finds an implicit conversion that works for both types.
        /// </summary>
        /// <param name="type">First type</param>
        /// <param name="otherType">Other types</param>
        /// <returns>A type for implicit conversion. Null if any of the types is non numerical.</returns>
        public static Type FindCommonNumericalType(this Type type, Type otherType)
        {
            var typeIsSigned = SignedTypesOrder.Contains(type);
            var otherIsSigned = SignedTypesOrder.Contains(otherType);
            var index = Array.IndexOf(UnsignedTypesOrder, type);
            var otherIndex = Array.IndexOf(UnsignedTypesOrder, otherType);
            if (index < 0 || otherIndex < 0)
            {
                return null;
            }

            // if we mix signed and unsigned, we want a signed type as a result and we need to identify the signed type large enough to hold the unsigned type
            if (typeIsSigned && !otherIsSigned)
            {
                otherIndex++;
            }

            if (!typeIsSigned && otherIsSigned)
            {
                index++;
            }

            // return the largest type of both
            return UnsignedTypesOrder[Math.Max(index, otherIndex)];
        }
    }
}