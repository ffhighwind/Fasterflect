#region License

// Copyright © 2010 Buu Nguyen, Morten Mertner
// Copyright © 2018 Wesley Hamilton
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
// The latest version of this file can be found at https://github.com/ffhighwind/fasterflect

#endregion

using Fasterflect.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect
{
	/// <summary>
	/// Helper class for looking up reflection based types such as <see cref="MemberInfo"/> and <see cref="ConstructorInfo"/>.
	/// </summary>
	public static class ReflectLookup
	{
		#region Constructor
		/// <summary>
		/// Gets the constructor corresponding to the supplied <paramref name="parameterTypes"/> on the
		/// given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="parameterTypes">The types of the constructor parameters in order.</param>
		/// <returns>The matching constructor or null if no match was found.</returns>
		public static ConstructorInfo Constructor(Type type, params Type[] parameterTypes)
		{
			return type.Constructor(FasterflectFlags.InstanceAnyDeclaredOnly, parameterTypes);
		}

		/// <summary>
		/// Gets the constructor matching the given <paramref name="bindingFlags"/> and corresponding to the 
		/// supplied <paramref name="parameterTypes"/> on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The search criteria to use when reflecting.</param>
		/// <param name="parameterTypes">The types of the constructor parameters in order.</param>
		/// <returns>The matching constructor or null if no match was found.</returns>
		public static ConstructorInfo Constructor(Type type, BindingFlags bindingFlags, params Type[] parameterTypes)
		{
			return type.GetConstructor(bindingFlags, null, parameterTypes, null);
		}

		/// <summary>
		/// Gets all public and non-public constructors (that are not abstract) on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <returns>A list of matching constructors. This value will never be null.</returns>
		public static IList<ConstructorInfo> Constructors(Type type)
		{
			return type.Constructors(FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets all constructors matching the given <paramref name="bindingFlags"/> (and that are not abstract)
		/// on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The search criteria to use when reflecting.</param>
		/// <returns>A list of matching constructors. This value will never be null.</returns>
		public static IList<ConstructorInfo> Constructors(Type type, BindingFlags bindingFlags)
		{
			return type.GetConstructors(bindingFlags);
		}
		#endregion

		#region Field Lookup (Single)
		/// <summary>
		/// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
		/// searches for public and non-public instance fields on both the type itself and all parent classes.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field(Type type, string name)
		{
			return Field(type, name, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. 
		/// Use the <paramref name="bindingFlags"/> parameter to define the scope of the search.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field(Type type, string name, FasterflectFlags bindingFlags)
		{
			// we need to check all fields to do partial name matches
			if (bindingFlags.IsAnySet(FasterflectFlags.PartialNameMatch | FasterflectFlags.TrimExplicitlyImplemented)) {
				return Fields(type, bindingFlags, name).FirstOrDefault();
			}

			FieldInfo result = type.GetField(name, bindingFlags);
			if (result == null && bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly)) {
				if (type.BaseType != typeof(object) && type.BaseType != null) {
					return type.BaseType.Field(name, bindingFlags);
				}
			}
			bool hasSpecialFlags = bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);
			if (hasSpecialFlags) {
				IList<FieldInfo> fields = new List<FieldInfo> { result };
				fields = fields.Filter(bindingFlags);
				return fields.Count > 0 ? fields[0] : null;
			}
			return result;
		}
		#endregion

		#region Field Lookup (Multiple)
		/// <summary>
		/// Gets all public and non-public instance fields on the given <paramref name="type"/>,
		/// including fields defined on base types.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. This method will check for an exact, 
		/// case-sensitive match.</param>
		/// <returns>A list of all instance fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> Fields(Type type, params string[] names)
		{
			return type.Fields(FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Gets all fields on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/>  to ignore case.</param>
		/// <returns>A list of all matching fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> Fields(Type type, FasterflectFlags bindingFlags, params string[] names)
		{
			if (type == null || type == typeof(object)) {
				return Constants.EmptyFieldInfoArray;
			}

			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			bool hasNames = names != null && names.Length > 0;
			bool hasSpecialFlags = bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);

			if (!recurse && !hasNames && !hasSpecialFlags) {
				return type.GetFields(bindingFlags) ?? Constants.EmptyFieldInfoArray;
			}

			IList<FieldInfo> fields = GetFields(type, bindingFlags);
			fields = hasSpecialFlags ? fields.Filter(bindingFlags) : fields;
			fields = hasNames ? fields.Filter(bindingFlags, names) : fields;
			return fields;
		}

		private static IList<FieldInfo> GetFields(Type type, FasterflectFlags bindingFlags)
		{
			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);

			if (!recurse) {
				return type.GetFields(bindingFlags) ?? Constants.EmptyFieldInfoArray;
			}

			bindingFlags |= FasterflectFlags.DeclaredOnly;
			bindingFlags &= ~BindingFlags.FlattenHierarchy;

			List<FieldInfo> fields = new List<FieldInfo>();
			fields.AddRange(type.GetFields(bindingFlags));
			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(object)) {
				fields.AddRange(baseType.GetFields(bindingFlags));
				baseType = baseType.BaseType;
			}
			return fields;
		}
		#endregion

		#region Property Lookup (Single)
		/// <summary>
		/// Gets the property identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
		/// searches for public and non-public instance properties on both the type itself and all parent classes.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property(Type type, string name)
		{
			return Property(type, name, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets the property identified by <paramref name="name"/> on the given <paramref name="type"/>. 
		/// Use the <paramref name="bindingFlags"/> parameter to define the scope of the search.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property(Type type, string name, FasterflectFlags bindingFlags)
		{
			// we need to check all properties to do partial name matches
			if (bindingFlags.IsAnySet(FasterflectFlags.PartialNameMatch | FasterflectFlags.TrimExplicitlyImplemented)) {
				return type.Properties(bindingFlags, name).FirstOrDefault();
			}

			PropertyInfo result = type.GetProperty(name, bindingFlags | FasterflectFlags.DeclaredOnly);
			if (result == null && bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly)) {
				if (type.BaseType != typeof(object) && type.BaseType != null) {
					return type.BaseType.Property(name, bindingFlags);
				}
			}
			bool hasSpecialFlags = bindingFlags.IsSet(FasterflectFlags.ExcludeExplicitlyImplemented);
			if (hasSpecialFlags) {
				IList<PropertyInfo> properties = new List<PropertyInfo> { result };
				properties = properties.Filter(bindingFlags);
				return properties.Count > 0 ? properties[0] : null;
			}
			return result;
		}
		#endregion

		#region Property Lookup (Multiple)
		/// <summary>
		/// Gets all public and non-public instance properties on the given <paramref name="type"/>,
		/// including properties defined on base types. The result can optionally be filtered by specifying
		/// a list of property names to include using the <paramref name="names"/> parameter.
		/// </summary>
		/// <returns>A list of matching instance properties on the type.</returns>
		/// <param name="type">The type whose public properties are to be retrieved.</param>
		/// <param name="names">A list of names of properties to be retrieved. If this is <see langword="null"/>, 
		/// all properties are returned.</param>
		/// <returns>A list of all public properties on the type filted by <paramref name="names"/>.
		/// This value will never be null.</returns>
		public static IList<PropertyInfo> Properties(Type type, params string[] names)
		{
			return Properties(type, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Gets all properties on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>,
		/// including properties defined on base types.
		/// </summary>
		/// <param name="type">The type whose public properties are to be retrieved.</param>
		/// <param name="names">A list of names of properties to be retrieved. If this is <see langword="null"/>, 
		/// all properties are returned.</param>
		/// <param name="bindingFlags"></param>
		/// <returns>A list of all matching properties on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> Properties(Type type, FasterflectFlags bindingFlags, params string[] names)
		{
			if (type == null || type == typeof(object)) {
				return Constants.EmptyPropertyInfoArray;
			}

			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			bool hasNames = names != null && names.Length > 0;
			bool hasSpecialFlags = bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);

			if (!recurse && !hasNames && !hasSpecialFlags) {
				return type.GetProperties(bindingFlags) ?? Constants.EmptyPropertyInfoArray;
			}

			IList<PropertyInfo> properties = GetProperties(type, bindingFlags);
			properties = hasSpecialFlags ? properties.Filter(bindingFlags) : properties;
			properties = hasNames ? properties.Filter(bindingFlags, names) : properties;
			return properties;
		}

		/// <summary>
		/// Gets all properties on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>,
		/// including properties defined on base types.
		/// </summary>
		/// <param name="type">The type whose public properties are to be retrieved.</param>
		/// <param name="bindingFlags"></param>
		/// <returns>A list of all public properties on the type. This value will never be null.</returns>
		private static IList<PropertyInfo> GetProperties(Type type, FasterflectFlags bindingFlags)
		{
			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);

			if (!recurse) {
				return type.GetProperties(bindingFlags) ?? Constants.EmptyPropertyInfoArray;
			}

			bindingFlags |= FasterflectFlags.DeclaredOnly;
			bindingFlags &= ~BindingFlags.FlattenHierarchy;

			List<PropertyInfo> properties = new List<PropertyInfo>();
			properties.AddRange(type.GetProperties(bindingFlags));
			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(object)) {
				properties.AddRange(baseType.GetProperties(bindingFlags));
				baseType = baseType.BaseType;
			}
			return properties;
		}
		#endregion

		#region Member Lookup (Single)
		/// <summary>
		/// Gets the member identified by <paramref name="name"/> on the given <paramref name="type"/>. This 
		/// method searches for public and non-public instance fields on both the type itself and all parent classes.
		/// </summary>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
		public static MemberInfo Member(Type type, string name)
		{
			return Members(type, MemberTypes.All, FasterflectFlags.InstanceAnyVisibility, name).FirstOrDefault();
		}

		/// <summary>
		/// Gets the member identified by <paramref name="name"/> on the given <paramref name="type"/>. Use 
		/// the <paramref name="bindingFlags"/> parameter to define the scope of the search.
		/// </summary>
		/// <returns>A single MemberInfo instance of the first found match or null if no match was found.</returns>
		public static MemberInfo Member(Type type, string name, FasterflectFlags bindingFlags, MemberTypes memberTypes = MemberTypes.All)
		{
			// we need to check all members to do partial name matches
			for(; ;) {
				if (bindingFlags.IsAnySet(FasterflectFlags.PartialNameMatch | FasterflectFlags.TrimExplicitlyImplemented)) {
					return type.Members(memberTypes, bindingFlags, name).FirstOrDefault();
				}
				IList<MemberInfo> result = type.GetMember(name, memberTypes, bindingFlags);
				if (result.Count > 0) {
					bool hasSpecialFlags = bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);
					if (!hasSpecialFlags)
						return result[0];
					result = result.Filter(bindingFlags);
					if (result.Count > 0)
						return result[0];
				}
				type = type.BaseType;
				if (bindingFlags.IsSet(FasterflectFlags.DeclaredOnly) || type == typeof(object) || type == null)
					break;
			}
			return null;
		}
		#endregion

		#region Member Lookup (FieldsAndProperties)
		/// <summary>
		/// Gets all public and non-public instance fields and properties on the given <paramref name="type"/>, 
		/// including members defined on base types.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndProperties(Type type)
		{
			return Members(type, MemberTypes.Field | MemberTypes.Property, FasterflectFlags.InstanceAnyVisibility, null);
		}

		/// <summary>
		/// Gets all public and non-public instance fields and properties on the given <paramref name="type"/> 
		/// that match the specified <paramref name="bindingFlags"/>, including members defined on base types.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> FieldsAndProperties(Type type, FasterflectFlags bindingFlags)
		{
			return Members(type, MemberTypes.Field | MemberTypes.Property, bindingFlags, null);
		}
		#endregion

		#region Member Lookup (Multiple)
		/// <summary>
		/// Gets all public and non-public instance members on the given <paramref name="type"/>.
		/// These are returned in random order.
		/// </summary>
		/// <returns>A list of all members on the type. This value will never be null.</returns>
		/// <param name="type">The type to reflect on.</param>
		/// <returns>A list of all members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members(Type type)
		{
			return Members(type, MemberTypes.All, FasterflectFlags.InstanceAnyVisibility, null);
		}

		/// <summary>
		/// Gets all public and non-public instance members on the given <paramref name="type"/> that 
		/// match the specified <paramref name="bindingFlags"/>. These are returned in random order.
		/// </summary>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members(Type type, FasterflectFlags bindingFlags)
		{
			return Members(type, MemberTypes.All, bindingFlags, null);
		}

		/// <summary>
		/// Gets all public and non-public instance members of the given <paramref name="memberTypes"/> on the 
		/// given <paramref name="type"/>, optionally filtered by the supplied <paramref name="names"/> list.
		/// These are returned in random order.
		/// </summary>
		/// <param name="memberTypes">The <see cref="MemberTypes"/> to include in the result.</param>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members(Type type, MemberTypes memberTypes, params string[] names)
		{
			return Members(type, memberTypes, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Gets all members of the given <paramref name="memberTypes"/> on the given <paramref name="type"/> that 
		/// match the specified <paramref name="bindingFlags"/>, optionally filtered by the supplied <paramref name="names"/>
		/// list (in accordance with the given <paramref name="bindingFlags"/>). These are returned in random order.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="memberTypes">The <see cref="MemberTypes"/> to include in the result.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static IList<MemberInfo> Members(Type type, MemberTypes memberTypes, FasterflectFlags bindingFlags, params string[] names)
		{
			if (type == null || type == typeof(object)) {
				return Constants.EmptyMemberInfoArray;
			}

			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			bool hasNames = names != null && names.Length > 0;
			bool hasSpecialFlags = bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);

			if (!recurse && !hasNames && !hasSpecialFlags) {
				return type.FindMembers(memberTypes, bindingFlags, null, null);
			}

			IList<MemberInfo> members = GetMembers(type, memberTypes, bindingFlags);
			members = hasSpecialFlags ? members.Filter(bindingFlags) : members;
			members = hasNames ? members.Filter(bindingFlags, names) : members;
			return members;
		}

		private static IList<MemberInfo> GetMembers(Type type, MemberTypes memberTypes, FasterflectFlags bindingFlags)
		{
			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			if (!recurse) {
				return type.FindMembers(memberTypes, bindingFlags, null, null);
			}

			bindingFlags |= BindingFlags.DeclaredOnly;
			bindingFlags &= ~BindingFlags.FlattenHierarchy;

			List<MemberInfo> members = new List<MemberInfo>();
			members.AddRange(type.FindMembers(memberTypes, bindingFlags, null, null));
			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(object)) {
				members.AddRange(baseType.FindMembers(memberTypes, bindingFlags, null, null));
				baseType = baseType.BaseType;
			}
			return members;
		}

		/// <summary>
		/// Gets all members of the given <paramref name="type"/> filtered by the supplied <paramref name="names"/> list.
		/// These are returned in the same order and a <see cref="MissingMemberException"/> is thrown if any member is missing.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="names">The optional list of names against which to filter the result.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static MemberInfo[] MembersExact(Type type, params string[] names)
		{
			return MembersExact(type, FasterflectFlags.StaticInstanceAnyVisibility, MemberTypes.All, names);
		}

		/// <summary>
		/// Gets all members of the given <paramref name="type"/> filtered by the supplied <paramref name="names"/> list. 
		/// These are returned in the same order and a <see cref="MissingMemberException"/> is thrown if any member is missing.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static MemberInfo[] MembersExact(Type type, FasterflectFlags bindingFlags, params string[] names)
		{
			return MembersExact(type, bindingFlags, MemberTypes.All, names);
		}

		/// <summary>
		/// Gets all members of the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>,
		/// filtered by the supplied <paramref name="names"/> list. These are returned in the same order and a <see cref="MissingMemberException"/> 
		/// is thrown if any member is missing.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="memberTypes">The <see cref="MemberTypes"/> to include in the result.</param>
		/// <param name="names">The optional list of names against which to filter the result.</param>
		/// <returns>A list of all matching members on the type. This value will never be null.</returns>
		public static MemberInfo[] MembersExact(Type type, FasterflectFlags bindingFlags, MemberTypes memberTypes, params string[] names)
		{
			MemberInfo[] members = new MemberInfo[names.Length];
			for (int i = 0, count = names.Length; i < count; ++i) {
				string name = names[i];
				members[i] = Member(type, name, bindingFlags, memberTypes) ?? throw new MissingMemberException(type.FullName, name);
			}
			return members;
		}
		#endregion

		#region Method Lookup (Single)
		/// <summary>
		/// Gets the public or non-public instance method with the given <paramref name="name"/> on the
		/// given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		/// default behavior is to check for an exact, case-sensitive match. Pass <see cref="FasterflectFlags.PartialNameMatch"/> 
		/// to locate by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(Type type, string name)
		{
			return Method(type, name, null, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string)"/>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name)
		{
			return Method(type, genericTypes, name, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets the public or non-public instance method with the given <paramref name="name"/> on the 
		/// given <paramref name="type"/> where the parameter types correspond in order with the
		/// supplied <paramref name="parameterTypes"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		/// default behavior is to check for an exact, case-sensitive match.</param>
		/// <param name="parameterTypes">If this parameter is not null then only methods with the same 
		/// parameter signature will be included in the result.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(Type type, string name, Type[] parameterTypes)
		{
			return Method(type, name, parameterTypes, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,Type[])"/>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name, Type[] parameterTypes)
		{
			return Method(type, genericTypes, name, parameterTypes, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		/// default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
		/// to include explicitly implemented interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate
		/// by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Method(type, name, null, bindingFlags);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,FasterflectFlags)"/>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags)
		{
			return Method(type, genericTypes, name, null, bindingFlags);
		}

		/// <summary>
		/// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="type"/> where the parameter types correspond in order with the
		/// supplied <paramref name="parameterTypes"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		///   default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
		///   to include explicitly implemented interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate
		///   by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
		///   will be included in the result. The default behavior is to check only for assignment compatibility,
		///   but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		///   the search behavior and result filtering.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(Type type, string name, Type[] parameterTypes, FasterflectFlags bindingFlags)
		{
			return Method(type, null, name, parameterTypes, bindingFlags);
		}

		/// <summary>
		/// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="type"/> where the parameter types correspond in order with the
		/// supplied <paramref name="parameterTypes"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="genericTypes">Type parameters if this is a generic method.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		///   default behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.ExplicitNameMatch"/> 
		///   to include explicitly implemented interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate
		///   by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
		///   will be included in the result. The default behavior is to check only for assignment compatibility,
		///   but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		///   the search behavior and result filtering.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name, Type[] parameterTypes, FasterflectFlags bindingFlags)
		{
			bool hasTypes = parameterTypes != null;
			bool hasGenericTypes = genericTypes != null && genericTypes.Length > 0;
			// we need to check all methods to do partial name matches or complex parameter binding
			bool processAll = bindingFlags.IsAnySet(FasterflectFlags.PartialNameMatch | FasterflectFlags.TrimExplicitlyImplemented);
			processAll |= hasTypes && bindingFlags.IsSet(FasterflectFlags.IgnoreParameterModifiers);
			processAll |= hasGenericTypes;
			if (processAll) {
				return type.Methods(genericTypes, parameterTypes, bindingFlags, name).FirstOrDefault().MakeGeneric(genericTypes);
			}

			MethodInfo result = hasTypes
				? type.GetMethod(name, bindingFlags, null, parameterTypes, null)
				: type.GetMethod(name, bindingFlags);
			if (result == null && bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly)) {
				if (type.BaseType != typeof(object) && type.BaseType != null) {
					return type.BaseType.Method(name, parameterTypes, bindingFlags).MakeGeneric(genericTypes);
				}
			}
			bool hasSpecialFlags =
				bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);
			if (hasSpecialFlags) {
				IList<MethodInfo> methods = new List<MethodInfo> { result }.Filter(bindingFlags);
				return (methods.Count > 0 ? methods[0] : null).MakeGeneric(genericTypes);
			}
			return result.MakeGeneric(genericTypes);
		}
		#endregion

		#region Method Lookup (Multiple)
		/// <summary>
		/// Gets all public and non-public instance methods on the given <paramref name="type"/> that match the 
		/// given <paramref name="names"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching methods. This value will never be null.</returns>
		public static IList<MethodInfo> Methods(Type type, params string[] names)
		{
			return type.Methods(null, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Gets all public and non-public instance methods on the given <paramref name="type"/> that match the 
		/// given <paramref name="names"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching methods. This value will never be null.</returns>
		public static IList<MethodInfo> Methods(Type type, FasterflectFlags bindingFlags, params string[] names)
		{
			return type.Methods(null, bindingFlags, names);
		}

		/// <summary>
		/// Gets all public and non-public instance methods on the given <paramref name="type"/> that match the given 
		///  <paramref name="names"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter 
		/// signature will be included in the result.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match.</param>
		/// <returns>A list of all matching methods. This value will never be null.</returns>
		public static IList<MethodInfo> Methods(Type type, Type[] parameterTypes, params string[] names)
		{
			return type.Methods(parameterTypes, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Gets all methods on the given <paramref name="type"/> that match the given lookup criteria.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
		/// will be included in the result. The default behavior is to check only for assignment compatibility,
		/// but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching methods. This value will never be null.</returns>
		public static IList<MethodInfo> Methods(Type type, Type[] parameterTypes, FasterflectFlags bindingFlags, params string[] names)
		{
			return type.Methods(null, parameterTypes, bindingFlags, names);
		}

		/// <summary>
		/// Gets all methods on the given <paramref name="type"/> that match the given lookup criteria.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="genericTypes">If this parameter is supplied then only methods with the same generic parameter 
		/// signature will be included in the result. The default behavior is to check only for assignment compatibility,
		/// but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
		/// will be included in the result. The default behavior is to check only for assignment compatibility,
		/// but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <see langword="null"/> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see cref="FasterflectFlags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see cref="FasterflectFlags.PartialNameMatch"/> to locate by substring, and 
		/// <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching methods. This value will never be null.</returns>
		public static IList<MethodInfo> Methods(Type type, Type[] genericTypes, Type[] parameterTypes, FasterflectFlags bindingFlags,
			params string[] names)
		{
			if (type == null || type == typeof(object)) {
				return Constants.EmptyMethodInfoArray;
			}
			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			bool hasNames = names != null && names.Length > 0;
			bool hasTypes = parameterTypes != null;
			bool hasGenericTypes = genericTypes != null && genericTypes.Length > 0;
			bool hasSpecialFlags =
				bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);

			if (!recurse && !hasNames && !hasTypes && !hasSpecialFlags) {
				return type.GetMethods(bindingFlags) ?? Constants.EmptyMethodInfoArray;
			}

			IList<MethodInfo> methods = GetMethods(type, bindingFlags);
			methods = hasNames ? methods.Filter(bindingFlags, names) : methods;
			methods = hasGenericTypes ? methods.Filter(genericTypes) : methods;
			methods = hasTypes ? methods.Filter(bindingFlags, parameterTypes) : methods;
			methods = hasSpecialFlags ? methods.Filter(bindingFlags) : methods;
			return methods;
		}

		private static IList<MethodInfo> GetMethods(Type type, FasterflectFlags bindingFlags)
		{
			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);

			if (!recurse) {
				return type.GetMethods(bindingFlags) ?? Constants.EmptyMethodInfoArray;
			}

			bindingFlags |= FasterflectFlags.DeclaredOnly;
			bindingFlags &= ~BindingFlags.FlattenHierarchy;

			List<MethodInfo> methods = new List<MethodInfo>();
			methods.AddRange(type.GetMethods(bindingFlags));
			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(object)) {
				methods.AddRange(baseType.GetMethods(bindingFlags));
				baseType = baseType.BaseType;
			}
			return methods;
		}
		#endregion

		#region Attribute Lookup (Single)
		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>The first attribute found on the source element.</returns>
		public static Attribute Attribute(ICustomAttributeProvider provider)
		{
			return provider.Attributes().FirstOrDefault();
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <paramref name="attributeType"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>The first attribute found on the source element.</returns>
		public static Attribute Attribute(ICustomAttributeProvider provider, Type attributeType)
		{
			return provider.Attributes(attributeType).FirstOrDefault();
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <typeparamref name="T"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>The first attribute found on the source element.</returns>
		public static T Attribute<T>(ICustomAttributeProvider provider) where T : Attribute
		{
			return provider.Attributes<T>().FirstOrDefault();
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <typeparamref name="T"/> associated with the 
		/// enumeration value given in the <paramref name="provider"/> parameter.
		/// </summary>
		/// <typeparam name="T">The attribute type to search for.</typeparam>
		/// <param name="provider">An enumeration value on which to search for the attribute.</param>
		/// <returns>The first attribute found on the source.</returns>
		public static T Attribute<T>(Enum provider) where T : Attribute
		{
			return provider.Attribute(typeof(T)) as T;
		}

		/// <summary>
		/// Gets the first <see cref="System.Attribute"/> of type <paramref name="attributeType"/> associated with the 
		/// enumeration value given in the <paramref name="provider"/> parameter.
		/// </summary>
		/// <param name="provider">An enumeration value on which to search for the attribute.</param>
		/// <param name="attributeType">The attribute type to search for.</param>
		/// <returns>The first attribute found on the source.</returns>
		public static Attribute Attribute(Enum provider, Type attributeType)
		{
			Type type = provider.GetType();
			MemberInfo info = type.Member(provider.ToString(), FasterflectFlags.StaticAnyVisibility | FasterflectFlags.DeclaredOnly);
			return info.Attribute(attributeType);
		}
		#endregion

		#region Attribute Lookup (Multiple)
		/// <summary>
		/// Gets the <see cref="System.Attribute"/>s associated with the <paramref name="provider"/>. The resulting
		/// list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
		/// to include.
		/// </summary>
		/// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
		public static IList<Attribute> Attributes(ICustomAttributeProvider provider, params Type[] attributeTypes)
		{
			bool hasTypes = attributeTypes != null && attributeTypes.Length > 0;
			return provider.GetCustomAttributes(true).Cast<Attribute>()
				.Where(attr => !hasTypes ||
					   attributeTypes.Any(at => {
						   Type type = attr.GetType();
						   return at == type || at.IsSubclassOf(type);
					   })).ToList();
		}

		/// <summary>
		/// Gets all <see cref="System.Attribute"/>s of type <typeparamref name="T"/> associated with the <paramref name="provider"/>.
		/// </summary>
		/// <returns>A list of the attributes found on the source element. This value will never be null.</returns>
		public static IList<T> Attributes<T>(ICustomAttributeProvider provider) where T : Attribute
		{
			return provider.GetCustomAttributes(typeof(T), true).Cast<T>().ToList();
		}

		/// <summary>
		/// Gets the <see cref="System.Attribute"/>s associated with the enumeration given in <paramref name="provider"/>. 
		/// </summary>
		/// <typeparam name="T">The attribute type to search for.</typeparam>
		/// <param name="provider">An enumeration on which to search for attributes of the given type.</param>
		/// <returns>A list of the attributes found on the supplied source. This value will never be null.</returns>
		public static IList<T> Attributes<T>(Enum provider) where T : Attribute
		{
			return provider.Attributes(typeof(T)).Cast<T>().ToList();
		}

		/// <summary>
		/// Gets the <see cref="System.Attribute"/>s associated with the enumeration given in <paramref name="provider"/>. 
		/// The resulting list of attributes can optionally be filtered by suppliying a list of <paramref name="attributeTypes"/>
		/// to include.
		/// </summary>
		/// <returns>A list of the attributes found on the supplied source. This value will never be null.</returns>
		public static IList<Attribute> Attributes(Enum provider, params Type[] attributeTypes)
		{
			Type type = provider.GetType();
			MemberInfo info = type.Member(provider.ToString(), FasterflectFlags.StaticAnyVisibility | FasterflectFlags.DeclaredOnly);
			return info.Attributes(attributeTypes);
		}
		#endregion

		#region HasAttribute Lookup (Presence Detection)
		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of type <paramref name="attributeType"/>.
		/// </summary>
		/// <returns>True if the source element has the associated attribute, false otherwise.</returns>
		public static bool HasAttribute(ICustomAttributeProvider provider, Type attributeType)
		{
			return provider.Attribute(attributeType) != null;
		}

		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of type <typeparamref name="T"/>.
		/// </summary>
		/// <returns>True if the source element has the associated attribute, false otherwise.</returns>
		public static bool HasAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
		{
			return provider.HasAttribute(typeof(T));
		}

		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of any of the types given in <paramref name="attributeTypes"/>.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="attributeTypes">The list of attribute types to look for. If this list is <see langword="null"/> or
		/// empty an <see cref="ArgumentException"/> will be thrown.</param>
		/// <returns>True if the source element has at least one of the specified attribute types, false otherwise.</returns>
		public static bool HasAnyAttribute(ICustomAttributeProvider provider, params Type[] attributeTypes)
		{
			return provider.Attributes(attributeTypes).Count() > 0;
		}

		/// <summary>
		/// Determines whether the <paramref name="provider"/> element has an associated <see cref="System.Attribute"/>
		/// of all of the types given in <paramref name="attributeTypes"/>.
		/// </summary>
		/// <returns>True if the source element has all of the specified attribute types, false otherwise.</returns>
		public static bool HasAllAttributes(ICustomAttributeProvider provider, params Type[] attributeTypes)
		{
			bool hasTypes = attributeTypes != null && attributeTypes.Length > 0;
			return !hasTypes || attributeTypes.All(at => provider.HasAttribute(at));
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
		public static IList<MemberInfo> MembersWith(Type type, MemberTypes memberTypes, params Type[] attributeTypes)
		{
			return type.MembersWith(memberTypes, FasterflectFlags.InstanceAnyVisibility, attributeTypes);
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
		public static IList<MemberInfo> MembersWith<T>(Type type, MemberTypes memberTypes, FasterflectFlags bindingFlags)
		{
			return type.MembersWith(memberTypes, bindingFlags, typeof(T));
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
		public static IList<MemberInfo> MembersWith(Type type, MemberTypes memberTypes, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			bool hasTypes = attributeTypes != null && attributeTypes.Length > 0;
			IEnumerable<MemberInfo> query = from m in type.Members(memberTypes, bindingFlags)
											where !hasTypes || m.HasAnyAttribute(attributeTypes)
											select m;
			return query.ToList();
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
		public static IList<MemberInfo> FieldsAndPropertiesWith(Type type, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Field | MemberTypes.Property, attributeTypes);
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
		public static IList<MemberInfo> FieldsAndPropertiesWith(Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Field | MemberTypes.Property, bindingFlags, attributeTypes);
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
		public static IList<FieldInfo> FieldsWith(Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Field, bindingFlags, attributeTypes).Cast<FieldInfo>().ToList();
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
		public static IList<PropertyInfo> PropertiesWith(Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
		{
			return type.MembersWith(MemberTypes.Property, bindingFlags, attributeTypes).Cast<PropertyInfo>().ToList();
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
		public static IList<MethodInfo> MethodsWith(Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
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
		public static IList<ConstructorInfo> ConstructorsWith(Type type, FasterflectFlags bindingFlags, params Type[] attributeTypes)
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
		public static IDictionary<MemberInfo, List<Attribute>> MembersAndAttributes(Type type, MemberTypes memberTypes, params Type[] attributeTypes)
		{
			return type.MembersAndAttributes(memberTypes, FasterflectFlags.InstanceAnyVisibility, attributeTypes);
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
		public static IDictionary<MemberInfo, List<Attribute>> MembersAndAttributes(Type type,
																					 MemberTypes memberTypes,
																					 FasterflectFlags bindingFlags,
																					 params Type[] attributeTypes)
		{
			var members = from m in type.Members(memberTypes, bindingFlags)
						  let a = m.Attributes(attributeTypes)
						  where a.Count() > 0
						  select new { Member = m, Attributes = a.ToList() };
			return members.ToDictionary(m => m.Member, m => m.Attributes);
		}
		#endregion
	}
}
