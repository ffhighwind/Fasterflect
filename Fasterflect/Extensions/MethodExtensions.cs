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
using System.Reflection;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for locating, inspecting and invoking methods.
	/// </summary>
	internal static partial class MethodExtensions
	{
		#region Method Invocation
		/// <summary>
		/// Creates a delegate which can invoke the method <paramref name="name"/> with arguments matching
		/// <paramref name="parameterTypes"/> on the given <paramref name="type"/>.
		/// Leave <paramref name="parameterTypes"/> empty if the method has no arguments.
		/// </summary>
		public static MethodInvoker DelegateForCallMethod(this Type type, string name, params Type[] parameterTypes)
		{
			return Reflect.Method(type, name, parameterTypes);
		}

		/// <summary>
		/// Create a delegate to invoke a generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="DelegateForCallMethod(Type,string,Type[])"/>
		public static MethodInvoker DelegateForCallMethod(this Type type, string name, Type[] genericTypes, params Type[] parameterTypes)
		{
			return Reflect.Method(type, name, genericTypes, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can invoke the method <paramref name="name"/> with arguments matching
		/// <paramref name="parameterTypes"/> and matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.
		/// Leave <paramref name="parameterTypes"/> empty if the method has no arguments.
		/// </summary>
		public static MethodInvoker DelegateForCallMethod(this Type type, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return Reflect.Method(type, name, bindingFlags, parameterTypes);
		}

		/// <summary>
		/// Create a delegate to invoke a generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="DelegateForCallMethod(Type,string,FasterflectFlags,Type[])"/>
		public static MethodInvoker DelegateForCallMethod(this Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags,
			params Type[] parameterTypes)
		{
			return Reflect.Method(type, genericTypes, name, bindingFlags, parameterTypes);
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
		public static MethodInfo Method(this Type type, string name)
		{
			return ReflectLookup.Method(type, name);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string)"/>
		public static MethodInfo Method(this Type type, Type[] genericTypes, string name)
		{
			return ReflectLookup.Method(type, genericTypes, name);
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
		public static MethodInfo Method(this Type type, string name, Type[] parameterTypes)
		{
			return ReflectLookup.Method(type, name, parameterTypes);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,Type[])"/>
		public static MethodInfo Method(this Type type, Type[] genericTypes, string name, Type[] parameterTypes)
		{
			return ReflectLookup.Method(type, genericTypes, name, parameterTypes);
		}

		/// <summary>
		/// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		/// default behavior is to check for an exact, case-sensitive match. Pass <see cref="FasterflectFlags.PartialNameMatch"/> 
		/// to locate by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(this Type type, string name, FasterflectFlags bindingFlags)
		{
			return ReflectLookup.Method(type, name, bindingFlags);
		}

		/// <summary>
		/// Gets a generic method.  See the overload with same arguments exception for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,FasterflectFlags)"/>
		public static MethodInfo Method(this Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags)
		{
			return ReflectLookup.Method(type, genericTypes, name, bindingFlags);
		}

		/// <summary>
		/// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="type"/> where the parameter types correspond in order with the
		/// supplied <paramref name="parameterTypes"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		/// default behavior is to check for an exact, case-sensitive match. Pass <see cref="FasterflectFlags.PartialNameMatch"/> 
		/// to locate by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
		/// will be included in the result. The default behavior is to check only for assignment compatibility,
		/// but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(this Type type, string name, Type[] parameterTypes, FasterflectFlags bindingFlags)
		{
			return ReflectLookup.Method(type, name, parameterTypes, bindingFlags);
		}

		/// <summary>
		/// Gets the method with the given <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="type"/> where the parameter types correspond in order with the
		/// supplied <paramref name="parameterTypes"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="genericTypes">Type parameters if this is a generic method.</param>
		/// <param name="name">The name of the method to search for. This argument must be supplied. The 
		/// default behavior is to check for an exact, case-sensitive match. Pass <see cref="FasterflectFlags.PartialNameMatch"/> 
		/// to locate by substring and <see cref="FasterflectFlags.IgnoreCase"/> to ignore case.</param>
		/// <param name="parameterTypes">If this parameter is supplied then only methods with the same parameter signature
		/// will be included in the result. The default behavior is to check only for assignment compatibility,
		/// but this can be changed to exact matching by passing <see cref="FasterflectFlags.ExactBinding"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <returns>The specified method or null if no method was found. If there are multiple matches
		/// due to method overloading the first found match will be returned.</returns>
		public static MethodInfo Method(this Type type, Type[] genericTypes, string name, Type[] parameterTypes, FasterflectFlags bindingFlags)
		{
			return ReflectLookup.Method(type, genericTypes, name, parameterTypes, bindingFlags);
		}

		internal static MethodInfo MakeGeneric(this MethodInfo methodInfo, Type[] genericTypes)
		{
			if (methodInfo == null) {
				return null;
			}
			if (genericTypes == null ||
				genericTypes.Length == 0 ||
				genericTypes == Type.EmptyTypes) {
				return methodInfo;
			}
			return methodInfo.MakeGenericMethod(genericTypes);
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
		public static IList<MethodInfo> Methods(this Type type, params string[] names)
		{
			return ReflectLookup.Methods(type, names);
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
		public static IList<MethodInfo> Methods(this Type type, FasterflectFlags bindingFlags, params string[] names)
		{
			return ReflectLookup.Methods(type, bindingFlags, names);
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
		public static IList<MethodInfo> Methods(this Type type, Type[] parameterTypes, params string[] names)
		{
			return ReflectLookup.Methods(type, parameterTypes, names);
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
		public static IList<MethodInfo> Methods(this Type type, Type[] parameterTypes, FasterflectFlags bindingFlags, params string[] names)
		{
			return ReflectLookup.Methods(type, parameterTypes, bindingFlags, names);
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
		public static IList<MethodInfo> Methods(this Type type, Type[] genericTypes, Type[] parameterTypes, FasterflectFlags bindingFlags,
			params string[] names)
		{
			return ReflectLookup.Methods(type, genericTypes, parameterTypes, bindingFlags, names);
		}
		#endregion
	}

	/// <summary>
	/// Extension methods for locating, inspecting and invoking methods.
	/// </summary>
	internal static partial class MethodExtensions
	{
		#region Method Invocation (Internal)
		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// using <paramref name="parameters"/> as arguments. 
		/// Leave <paramref name="parameters"/> empty if the method has no arguments.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <see langword="null"/> is returned.</remarks>
		/// <remarks>
		/// All elements of <paramref name="parameters"/> must not be <see langword="null"/>.  Otherwise, 
		/// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
		/// any element is <see langword="null"/> or not, use the overload that accepts a <see cref="Type"/> array.
		/// </remarks>
		/// <seealso cref="CallMethod(object,string,System.Type[],object[])"/>
		internal static object CallMethod(this object obj, string name, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			Type[] parameterTypes = parameters.ToTypeArray();
			MethodInvoker method = DelegateForCallMethod(type, null, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			Type[] parameterTypes = parameters.ToTypeArray();
			MethodInvoker method = DelegateForCallMethod(type, genericTypes, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// using <paramref name="parameters"/> as arguments.
		/// Method parameter types are specified by <paramref name="parameterTypes"/>.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <see langword="null"/> is returned.</remarks>
		internal static object CallMethod(this object obj, string name, Type[] parameterTypes, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			MethodInvoker method = DelegateForCallMethod(type, null, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Type[],object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, Type[] parameterTypes, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			MethodInvoker method = DelegateForCallMethod(type, genericTypes, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/>
		/// matching <paramref name="bindingFlags"/> using <paramref name="parameters"/> as arguments.
		/// Leave <paramref name="parameters"/> empty if the method has no argument.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <see langword="null"/> is returned.</remarks>
		/// <remarks>
		/// All elements of <paramref name="parameters"/> must not be <see langword="null"/>.  Otherwise, 
		/// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
		/// any element is <see langword="null"/> or not, use the overload that accepts a <see cref="Type"/> array.
		/// </remarks>
		/// <seealso cref="CallMethod(object,string,System.Type[],Fasterflect.FasterflectFlags,object[])"/>
		internal static object CallMethod(this object obj, string name, FasterflectFlags bindingFlags, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			Type[] parameterTypes = parameters.ToTypeArray();
			MethodInvoker method = DelegateForCallMethod(type, null, name, bindingFlags, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,FasterflectFlags,object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, FasterflectFlags bindingFlags, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			Type[] parameterTypes = parameters.ToTypeArray();
			MethodInvoker method = DelegateForCallMethod(type, genericTypes, name, bindingFlags, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invokes a method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// matching <paramref name="bindingFlags"/> using <paramref name="parameters"/> as arguments.
		/// Method parameter types are specified by <paramref name="parameterTypes"/>.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <see langword="null"/> is returned.</remarks>
		internal static object CallMethod(this object obj, string name, Type[] parameterTypes, FasterflectFlags bindingFlags, params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			MethodInvoker method = DelegateForCallMethod(type, null, name, bindingFlags, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Type[],FasterflectFlags,object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, Type[] parameterTypes, FasterflectFlags bindingFlags,
			params object[] parameters)
		{
			Type type = obj.GetTypeAdjusted();
			MethodInvoker method = DelegateForCallMethod(type, genericTypes, name, bindingFlags, parameterTypes);
			object value = method(obj, parameters);
			return value;
		}
		#endregion
	}
}