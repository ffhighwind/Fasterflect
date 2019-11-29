#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
// Copyright 2018 Wesley Hamilton
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

using Fasterflect.Emitter;
using Fasterflect.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Fasterflect
{
	/// <summary>
	/// Helper class for looking up <see cref="MemberInfo"/> and <see cref="ConstructorInfo"/> types.
	/// </summary>
	public static class ReflectLookup
	{
		#region ConstructorInfo
		/// <summary>
		/// Gets the constructor corresponding to the supplied <paramref name="parameterTypes"/> on the
		/// given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="parameterTypes">The types of the constructor parameters in order.</param>
		/// <returns>The matching constructor or null if no match was found.</returns>
		public static ConstructorInfo Constructor(Type type, params Type[] parameterTypes)
		{
			return type.Constructor(FasterflectFlags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Gets the constructor matching the given <paramref name="bindingFlags"/> and corresponding to the 
		/// supplied <paramref name="parameterTypes"/> on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type to reflect on.</param>
		/// <param name="bindingFlags">The search criteria to use when reflecting.</param>
		/// <param name="parameterTypes">The types of the constructor parameters in order.</param>
		/// <returns>The matching constructor or null if no match was found.</returns>
		public static ConstructorInfo Constructor(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
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
		public static IList<ConstructorInfo> Constructors(Type type, FasterflectFlags bindingFlags)
		{
			return type.GetConstructors(bindingFlags); //.Where(ci => !ci.IsAbstract).ToList();
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
			return type.Field(name, FasterflectFlags.InstanceAnyVisibility);
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
				return type.Fields(bindingFlags, name).FirstOrDefault();
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
				return new FieldInfo[0];
			}

			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			bool hasNames = names != null && names.Length > 0;
			bool hasSpecialFlags = bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);

			if (!recurse && !hasNames && !hasSpecialFlags) {
				return type.GetFields(bindingFlags) ?? new FieldInfo[0];
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
				return type.GetFields(bindingFlags) ?? new FieldInfo[0];
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
			return type.Property(name, FasterflectFlags.InstanceAnyVisibility);
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
			return type.Properties(FasterflectFlags.InstanceAnyVisibility, names);
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
			return type.Method(name, null, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string)"/>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name)
		{
			return type.Method(genericTypes, name, FasterflectFlags.InstanceAnyVisibility);
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
			return type.Method(name, parameterTypes, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,Type[])"/>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name, Type[] parameterTypes)
		{
			return type.Method(genericTypes, name, parameterTypes, FasterflectFlags.InstanceAnyVisibility);
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
			return type.Method(name, null, bindingFlags);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,FasterflectFlags)"/>
		public static MethodInfo Method(Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags)
		{
			return type.Method(genericTypes, name, null, bindingFlags);
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
			return type.Method(null, name, parameterTypes, bindingFlags);
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
				return new MethodInfo[0];
			}
			bool recurse = bindingFlags.IsNotSet(FasterflectFlags.DeclaredOnly);
			bool hasNames = names != null && names.Length > 0;
			bool hasTypes = parameterTypes != null;
			bool hasGenericTypes = genericTypes != null && genericTypes.Length > 0;
			bool hasSpecialFlags =
				bindingFlags.IsAnySet(FasterflectFlags.ExcludeBackingMembers | FasterflectFlags.ExcludeExplicitlyImplemented | FasterflectFlags.ExcludeHiddenMembers);

			if (!recurse && !hasNames && !hasTypes && !hasSpecialFlags) {
				return type.GetMethods(bindingFlags) ?? new MethodInfo[0];
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
				return type.GetMethods(bindingFlags) ?? new MethodInfo[0];
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
	}
}
