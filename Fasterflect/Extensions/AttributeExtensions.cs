#region License
// Copyright © 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for locating and retrieving attributes.
	/// </summary>
	internal static class AttributeExtensions
	{
		#region Attribute Lookup (Single)
		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>The first attribute found on the source element.</returns>
		public static Attribute Attribute(this ICustomAttributeProvider provider)
		{
			return ReflectLookup.Attribute(provider);
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <paramref name="attributeType"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>The first attribute found on the source element.</returns>
		public static Attribute Attribute(this ICustomAttributeProvider provider, Type attributeType)
		{
			return ReflectLookup.Attribute(provider, attributeType);
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <typeparamref name="T"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>The first attribute found on the source element.</returns>
		public static T Attribute<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			return ReflectLookup.Attribute<T>(provider);
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <typeparamref name="T"/> associated with the 
		/// enumeration value given in the <paramref name="provider"/> parameter.
		/// </summary>
		/// <typeparam name="T">The attribute type to search for.</typeparam>
		/// <param name="provider">An enumeration value on which to search for the attribute.</param>
		/// <returns>The first attribute found on the source.</returns>
		public static T Attribute<T>(this Enum provider) where T : Attribute
		{
			return ReflectLookup.Attribute<T>(provider);
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <paramref name="attributeType"/> associated with the 
		/// enumeration value given in the <paramref name="provider"/> parameter.
		/// </summary>
		/// <param name="provider">An enumeration value on which to search for the attribute.</param>
		/// <param name="attributeType">The attribute type to search for.</param>
		/// <returns>The first attribute found on the source.</returns>
		public static Attribute Attribute(this Enum provider, Type attributeType)
		{
			return ReflectLookup.Attribute(provider, attributeType);
		}
		#endregion

		#region Attribute Lookup (Multiple)
		/// <summary>
		/// Gets the <see cref="System.Attribute"/>s associated with the <paramref name="provider"/>. The resulting
		/// list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
		/// to include.
		/// </summary>
		/// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
		public static IList<Attribute> Attributes(this ICustomAttributeProvider provider, params Type[] attributeTypes)
		{
			return ReflectLookup.Attributes(provider, attributeTypes);
		}

		/// <summary>
		/// Gets all <see cref="System.Attribute"/>s of type <typeparamref name="T"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
		public static IList<T> Attributes<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			return ReflectLookup.Attributes<T>(provider);
		}

		/// <summary>
		/// Gets the <see cref="System.Attribute"/>s associated with the enumeration given in <paramref name="provider"/>. 
		/// </summary>
		/// <typeparam name="T">The attribute type to search for.</typeparam>
		/// <param name="provider">An enumeration on which to search for attributes of the given type.</param>
		/// <returns>A list of the attributes found on the supplied source. This value will never be null.</returns>
		public static IList<T> Attributes<T>(this Enum provider) where T : Attribute
		{
			return ReflectLookup.Attributes<T>(provider);
		}

		/// <summary>
		/// Gets the <see cref="System.Attribute"/>s associated with the enumeration given in <paramref name="provider"/>. 
		/// The resulting list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
		/// to include.
		/// </summary>
		/// <returns>A list of the attributes found on the supplied source. This value will never be null.</returns>
		public static IList<Attribute> Attributes(this Enum provider, params Type[] attributeTypes)
		{
			return ReflectLookup.Attributes(provider, attributeTypes);
		}
		#endregion

		#region HasAttribute Lookup (Presence Detection)
		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of type <paramref name="attributeType"/>.
		/// </summary>
		/// <returns>True if the source element has the associated attribute, false otherwise.</returns>
		public static bool HasAttribute(this ICustomAttributeProvider provider, Type attributeType)
		{
			return ReflectLookup.HasAttribute(provider, attributeType);
		}

		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of type <typeparamref name="T"/>.
		/// </summary>
		/// <returns>True if the source element has the associated attribute, false otherwise.</returns>
		public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			return ReflectLookup.HasAttribute<T>(provider);
		}

		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of any of the types given in <paramref name="attributeTypes"/>.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="attributeTypes">The list of attribute types to look for. If this list is <see langword="null"/> or
		/// empty an <see cref="ArgumentException"/> will be thrown.</param>
		/// <returns>True if the source element has at least one of the specified attribute types, false otherwise.</returns>
		public static bool HasAnyAttribute(this ICustomAttributeProvider provider, params Type[] attributeTypes)
		{
			return ReflectLookup.HasAnyAttribute(provider, attributeTypes);
		}

		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of all of the types given in <paramref name="attributeTypes"/>.
		/// </summary>
		/// <returns>True if the source element has all of the specified attribute types, false otherwise.</returns>
		public static bool HasAllAttributes(this ICustomAttributeProvider provider, params Type[] attributeTypes)
		{
			return ReflectLookup.HasAllAttributes(provider, attributeTypes);
		}
		#endregion

		#region MembersWith Lookup
		/// <summary>
		/// Gets all public and non-public instance members on the given <paramref name="type"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="memberTypes">The <see cref="MemberTypes"/> to include in the search.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all fields and properties
		/// will be included in the result.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> MembersWith(this Type type, MemberTypes memberTypes,
													 params Type[] attributeTypes)
		{
			return ReflectLookup.MembersWith(type, memberTypes, attributeTypes);
		}

		/// <summary>
		/// Gets all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> 
		/// that match the specified <paramref name="bindingFlags"/> and are decorated with an
		/// <see cref="System.Attribute"/> of the given type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="memberTypes">The <see cref="MemberTypes"/> to include in the search.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> MembersWith<T>(this Type type, MemberTypes memberTypes, FasterflectFlags bindingFlags)
		{
			return ReflectLookup.MembersWith<T>(type, memberTypes, bindingFlags);
		}

		/// <summary>
		/// Gets all members on the given <paramref name="type"/> that match the specified 
		/// <paramref name="bindingFlags"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="memberTypes">The <see cref="MemberTypes"/> to include in the search.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all fields and properties
		/// matching the given <paramref name="bindingFlags"/> will be included in the result.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> MembersWith(this Type type, MemberTypes memberTypes, FasterflectFlags bindingFlags,
													 params Type[] attributeTypes)
		{
			return ReflectLookup.MembersWith(type, memberTypes, bindingFlags, attributeTypes);
		}
		#endregion

		#region FieldsAndPropertiesWith, FieldsWith, PropertiesWith
		/// <summary>
		/// Gets all public and non-public instance fields and properties on the given <paramref name="type"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all fields and properties
		/// will be included in the result.</param>
		/// <returns>A list of all matching fields and properties on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndPropertiesWith(this Type type, params Type[] attributeTypes)
		{
			return ReflectLookup.FieldsAndPropertiesWith(type, attributeTypes);
		}

		/// <summary>
		/// Gets all fields and properties on the given <paramref name="type"/> that match the specified 
		/// <paramref name="bindingFlags"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all fields and properties
		/// matching the given <paramref name="bindingFlags"/> will be included in the result.</param>
		/// <returns>A list of all matching fields and properties on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndPropertiesWith(this Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return ReflectLookup.FieldsAndPropertiesWith(type, bindingFlags, attributeTypes);
		}

		/// <summary>
		/// Gets all fields on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all fields matching the given 
		/// <paramref name="bindingFlags"/> will be included in the result.</param>
		/// <returns>A list of all matching fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> FieldsWith(this Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return ReflectLookup.FieldsWith(type, bindingFlags, attributeTypes);
		}

		/// <summary>
		/// Gets all properties on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all properties matching the given 
		/// <paramref name="bindingFlags"/> will be included in the result.</param>
		/// <returns>A list of all matching properties on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> PropertiesWith(this Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return ReflectLookup.PropertiesWith(type, bindingFlags, attributeTypes);
		}
		#endregion

		#region MethodsWith, ConstructorsWith
		/// <summary>
		/// Gets all methods on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all methods matching the given 
		/// <paramref name="bindingFlags"/> will be included in the result.</param>
		/// <returns>A list of all matching methods on the type. This value will never be null.</returns>
		public static IList<MethodInfo> MethodsWith(this Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Method, bindingFlags, attributeTypes).Cast<MethodInfo>().ToList();
		}

		/// <summary>
		/// Gets all constructors on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// The resulting list of members can optionally be filtered by supplying a list of 
		/// <paramref name="attributeTypes"/>, in which case only members decorated with at least one of
		/// these will be included.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination 
		/// used to define the search behavior and result filtering.</param>
		/// <param name="attributeTypes">The optional list of attribute types with which members should
		/// be decorated. If this parameter is <see langword="null"/> or empty then all constructors matching the given 
		/// <paramref name="bindingFlags"/> will be included in the result.</param>
		/// <returns>A list of all matching constructors on the type. This value will never be null.</returns>
		public static IList<ConstructorInfo> ConstructorsWith(this Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Constructor, bindingFlags, attributeTypes).Cast<ConstructorInfo>().ToList();
		}
		#endregion

		#region MembersAndAttributes Lookup
		/// <summary>
		/// Gets a dictionary with all public and non-public instance members on the given <paramref name="type"/> 
		/// and their associated attributes. Only members of the given <paramref name="memberTypes"/> will
		/// be included in the result.
		/// The list of attributes associated with each member can optionally be filtered by supplying a list of
		/// <paramref name="attributeTypes"/>, in which case only members with at least one of these will be
		/// included in the result.
		/// </summary>
		/// <returns>An dictionary mapping all matching members to their associated attributes. This value
		/// will never be null. The attribute list associated with each member in the dictionary will likewise
		/// never be null.</returns>
		public static IDictionary<MemberInfo, List<Attribute>> MembersAndAttributes(this Type type,
																					 MemberTypes memberTypes,
																					 params Type[] attributeTypes)
		{
			return ReflectLookup.MembersAndAttributes(type, memberTypes, attributeTypes);
		}

		/// <summary>
		/// Gets a dictionary with all members on the given <paramref name="type"/> and their associated attributes.
		/// Only members of the given <paramref name="memberTypes"/> and matching <paramref name="bindingFlags"/> will
		/// be included in the result.
		/// The list of attributes associated with each member can optionally be filtered by supplying a list of
		/// <paramref name="attributeTypes"/>, in which case only members with at least one of these will be
		/// included in the result.
		/// </summary>
		/// <returns>An dictionary mapping all matching members to their associated attributes. This value
		/// will never be null. The attribute list associated with each member in the dictionary will likewise
		/// never be null.</returns>
		public static IDictionary<MemberInfo, List<Attribute>> MembersAndAttributes(this Type type,
																					 MemberTypes memberTypes,
																					 FasterflectFlags bindingFlags,
																					 params Type[] attributeTypes)
		{
			return ReflectLookup.MembersAndAttributes(type, memberTypes, bindingFlags, attributeTypes);
		}
		#endregion
	}
}