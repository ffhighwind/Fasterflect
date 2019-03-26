#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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
using Fasterflect.Emitter;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for locating and accessing fields.
	/// </summary>
	public static partial class FieldExtensions
	{
		#region Field Access
		/// <summary>
		/// Creates a delegate which can set the value of the field specified by <paramref name="name"/> on 
		/// the given <paramref name="type"/>.
		/// </summary>
		public static MemberSetter DelegateForSetFieldValue(this Type type, string name)
		{
			return Reflect.FieldSetter(type, name);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the field specified by <paramref name="name"/> on 
		/// the given <paramref name="type"/>.
		/// </summary>
		public static MemberGetter DelegateForGetFieldValue(this Type type, string name)
		{
			return Reflect.FieldGetter(type, name);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the field specified by <paramref name="name"/> and
		/// matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.
		/// </summary>
		public static MemberSetter DelegateForSetFieldValue(this Type type, string name, FasterflectFlags bindingFlags)
		{
			return Reflect.FieldSetter(type, name, bindingFlags);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the field specified by <paramref name="name"/> and
		/// matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.
		/// </summary>
		public static MemberGetter DelegateForGetFieldValue(this Type type, string name, FasterflectFlags bindingFlags)
		{
			return Reflect.FieldGetter(type, name, bindingFlags);
		}
		#endregion

		#region Field Lookup (Single)
		/// <summary>
		/// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
		/// searches for public and non-public instance fields on both the type itself and all parent classes.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field(this Type type, string name)
		{
			return Reflect.Lookup.Field(type, name);
		}

		/// <summary>
		/// Gets the field identified by <paramref name="name"/> on the given <paramref name="type"/>. 
		/// Use the <paramref name="bindingFlags"/> parameter to define the scope of the search.
		/// </summary>
		/// <returns>A single FieldInfo instance of the first found match or null if no match was found.</returns>
		public static FieldInfo Field(this Type type, string name, FasterflectFlags bindingFlags)
		{
			return Reflect.Lookup.Field(type, name, bindingFlags);
		}
		#endregion

		#region Field Lookup (Multiple)
		/// <summary>
		/// Gets all public and non-public instance fields on the given <paramref name="type"/>,
		/// including fields defined on base types.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. This method will check for an exact, 
		/// case-sensitive match.</param>
		/// <returns>A list of all instance fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> Fields(this Type type, params string[] names)
		{
			return Reflect.Lookup.Fields(type, names);
		}

		/// <summary>
		/// Gets all fields on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type on which to reflect.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> combination used to define
		/// the search behavior and result filtering.</param>
		/// <param name="names">The optional list of names against which to filter the result. If this parameter is
		/// <c>null</c> or empty no name filtering will be applied. The default behavior is to check for an exact, 
		/// case-sensitive match. Pass <see href="Flags.ExcludeExplicitlyImplemented"/> to exclude explicitly implemented 
		/// interface members, <see href="Flags.PartialNameMatch"/> to locate by substring, and 
		/// <see href="Flags.IgnoreCase"/> to ignore case.</param>
		/// <returns>A list of all matching fields on the type. This value will never be null.</returns>
		public static IList<FieldInfo> Fields(this Type type, FasterflectFlags bindingFlags, params string[] names)
		{
			return Reflect.Lookup.Fields(type, bindingFlags, names);
		}
		#endregion
	}
}